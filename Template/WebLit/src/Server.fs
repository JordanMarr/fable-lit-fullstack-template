module WebLit.Server

open System
open Fable.Remoting.Client
open Fable.Core

/// Takes path segments and combines them into a valid path
let combine (paths: string list) =
    paths
    |> List.map (fun path -> List.ofArray (path.Split('/')))
    |> List.concat
    |> List.filter (fun segment -> not (segment.Contains(".")))
    |> List.filter (String.IsNullOrWhiteSpace >> not)
    |> String.concat "/"
    |> sprintf "/%s"

/// When publishing to IIS, your application most likely runs inside a virtual path (i.e. localhost/SafeApp)
/// every request made to the server will have to account for this virtual path
/// so we get the virtual path from the location
/// `virtualPath` of `http://localhost/SafeApp` -> `/SafeApp/`
let virtualPath : string =
    JS.eval("window.location.pathname")

/// Normalized the path taking into account the virtual path of the server
let normalize (path: string) = 
    combine [ virtualPath; path ]

let normalizeRoutes typeName methodName =
    Shared.Api.routerPaths typeName methodName
    |> normalize
    
let api =
    Remoting.createApi()
    |> Remoting.withRouteBuilder normalizeRoutes
    |> Remoting.buildProxy<Shared.Api.IServerApi>