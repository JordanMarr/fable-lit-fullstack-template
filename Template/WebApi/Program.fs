module WebApi.Program

open Saturn
open Giraffe
open Shared
open WebApi
open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Http
open System
open Microsoft.Extensions.Logging
open Shared
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Serilog
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Builder

let rec private unwrapEx (ex: Exception) =
    if not (isNull ex.InnerException) then 
        printfn "Parent Ex: %s" ex.Message
        printfn "Unwrapping inner exception..."
        unwrapEx ex.InnerException
    else ex

let fableRemotingErrorHandler (ex: Exception) (ri: RouteInfo<HttpContext>) = 
    let logger = ri.httpContext.GetLogger()
    logger.LogError(sprintf "Error at %s on method %s" ri.path ri.methodName)
    // Decide whether or not you want to propagate the error to the client
    let ex = unwrapEx ex
    logger.LogError(sprintf "Error: %s" ex.Message)
    Propagate "An error occurred while processing the request."
    
let remotingWebApi =
    Remoting.createApi()
    |> Remoting.fromContext (fun (ctx: HttpContext) -> ctx.GetService<ApiBuilder.ServerApi>().Build())
    |> Remoting.withRouteBuilder Api.routerPaths
    |> Remoting.withErrorHandler fableRemotingErrorHandler
    |> Remoting.buildHttpHandler

let webApp = 
    choose [ 
        remotingWebApi
        GET >=> routeCi "/api/ping"   >=> text "pong"
        GET >=> htmlView Views.devMode
    ]

let serviceConfig (services: IServiceCollection) =
    services
      .AddSingleton<ApiBuilder.ServerApi>()
      .AddLogging()

let configureWebHost (builder: IWebHostBuilder) =
    builder.ConfigureAppConfiguration (fun context config ->
        config
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json" , true)
            .AddEnvironmentVariables()
            |> ignore
    )

let configureCors (builder: CorsPolicyBuilder) =
    builder.WithOrigins([| "http://localhost:3000" |])
           .AllowAnyMethod()
           .AllowAnyHeader()
           |> ignore

let application = application {
    use_router webApp
    use_static "wwwroot"
    use_cors "CORS_policy" configureCors
    use_gzip
    service_config serviceConfig
    webhost_config configureWebHost
}

run application
