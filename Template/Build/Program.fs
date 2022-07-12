module Program

open System
open System.Diagnostics
open System.IO
open Fake.IO
open Fake.Core
open Fake.Core.TargetOperators


// Initialize FAKE context
Setup.context()

let path xs = Path.Combine(Array.ofList xs)
let solutionRoot = Files.findParent __SOURCE_DIRECTORY__ "Fullstack.sln";
let webApi = path [ solutionRoot; "WebApi" ]
let webLit =  path [ solutionRoot; "WebLit" ]
let webApiTests = path [ solutionRoot; "WebApi.Tests" ]
let webLitTests = path [ solutionRoot; "WebLit.Tests" ]
let webLitDist = path [ webLit; "dist" ]
let build = path [ solutionRoot; ".build" ]
let dist = path [ solutionRoot; build; "dist" ]
let clientOutput = path [ dist; "wwwroot" ]

Target.create "Clean" <| fun _ ->
    // sometimes files are locked by VS for a bit, retry again until they can be deleted
    Retry.retry 5 <| fun _ -> Shell.deleteDirs [
        build
        path [ webApi; "bin" ]
        path [ webApi; "obj" ]
        path [ webApiTests; "bin" ]
        path [ webApiTests; "obj" ]
        path [ webLit; "bin" ]
        path [ webLit; "obj" ]
        path [ webLit; ".fable" ]
        path [ webLitTests; "bin" ]
        path [ webLitTests; "obj" ]
        path [ webLitTests; ".fable" ]
    ]

Target.create "RestoreServer" <| fun _ ->
    Retry.retry 5 <| fun _ ->
        let exitCode = Shell.Exec(Tools.dotnet, "restore", webApi)
        if exitCode <> 0 then failwith "Could restore packages in the server project"

Target.create "Server" <| fun _ ->
    Retry.retry 5 <| fun _ ->
        let exitCode = Shell.Exec(Tools.dotnet, "build --configuration Release", webApi)
        if exitCode <> 0 then failwith "Could not build the server project"

Target.create "ServerTests" <| fun _ ->
    let exitCode = Shell.Exec(Tools.dotnet, "run --configuration Release", webApiTests)
    if exitCode <> 0 then failwith "Failed while while running server tests"

Target.create "RestoreClient" <| fun _ ->
    let exitCode = Shell.Exec(Tools.npm, "install", webLit)
    if exitCode <> 0 then failwith "failed to run `npm install` in the client directory"

Target.create "Client" <| fun _ ->
    let exitCode = Shell.Exec(Tools.npm, "run build", webLit)
    if exitCode <> 0 then failwith "Failed to build client"

Target.create "ClientTests" <| fun _ ->
    let exitCode = Shell.Exec(Tools.npm, "test", webLit)
    if exitCode <> 0 then failwith "Client tests did not pass"

Target.create "HeadlessBrowserTests" <| fun _ ->
    Shell.cleanDir webLitDist
    let exitCode = Shell.Exec(Tools.npm, "run build:test", webLit)
    if exitCode <> 0 then
        failwith "Failed to build tests project"
    else
        let testResults = Async.RunSynchronously(Puppeteer.runTests webLitDist)
        if testResults <> 0 then failwith "Some tests failed"

Target.create "LiveClientTests" <| fun _ ->
    let exitCode = Shell.Exec(Tools.npm, "run test:live", webLit)
    if exitCode <> 0 then failwith "Failed to run client tests"

Target.create "Pack" <| fun _ ->
    match Shell.Exec(Tools.dotnet, sprintf "publish --configuration Release --output %s" dist, webApi) with
    | 0 ->
        let exitCode = Shell.Exec(Tools.npm, "run build", webLit)
        if exitCode <> 0 then failwith "Failed to build client"
        Shell.copyDir clientOutput webLitDist (fun file -> true)
    | n ->
        failwith "Failed to publish server project"

Target.create "PackNoTests" <| fun _ ->
    match Shell.Exec(Tools.dotnet, sprintf "publish --configuration Release --output %s" dist, webApi) with
    | 0 ->
        match Shell.Exec(Tools.npm, "run build", webLit) with
        | 0 -> Shell.copyDir clientOutput webLitDist (fun file -> true)
        | _ -> failwith "Failed to build the client project"
    | _ ->
        failwith "Failed to build the server project"

Target.create "Restore" <| fun _ ->
    printfn "Restoring Server and Client"

Target.create "AzDeployWeb" <| fun _ -> 
    // Zip Web App
    let distZip = path [ solutionRoot; build; "dist.zip"]
    System.IO.Compression.ZipFile.CreateFromDirectory(dist, distZip)
    
    // Azure Web App Service Deployment
    let azAccountGuid = ""
    let azResourceGroup = ""
    let azWebAppName = ""    
    Shell.Exec(Tools.az, $"account set -s {azAccountGuid}") |> ignore
    Shell.Exec(Tools.az, $"webapp deploy --resource-group {azResourceGroup} --name {azWebAppName} --src-path {distZip}") |> ignore

let dependencies = [
    "Clean" ==> "RestoreServer" ==> "RestoreClient" ==> "Restore"
    "Clean" ==> "RestoreServer" ==> "Server" ==> "ServerTests"
    "Clean" ==> "RestoreClient" ==> "Client" ==> "ClientTests"
    "Clean" ==> "ServerTests" ==> "ClientTests" ==> "Pack"
    "Clean" ==> "Server" ==> "Client" ==> "PackNoTests"
    "PackNoTests" ==> "AzDeployWeb" // Deploy Web App to Azure
]

[<EntryPoint>]
let main (args: string[]) =
    Console.WriteLine(Swag.logo)
    try
        match args with
        | [| "RunDefaultOr" |] -> Target.runOrDefault "Default"
        | [| "RunDefaultOr"; target |] -> Target.runOrDefault target
        | [| singleArg |] -> Target.runOrDefault singleArg
        | manyArguments ->
            Console.Write("[Interactive Mode] Run build target: ")
            let target = Console.ReadLine()
            Target.runOrDefault target
        0
    with ex ->
        printfn "%A" ex
        1