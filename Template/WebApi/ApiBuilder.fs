module WebApi.ApiBuilder

open Microsoft.Extensions.Logging
open Microsoft.Extensions.Configuration
open Shared
open Shared.Api
open System.Net.Http
open System

/// An implementation of the Shared IServerApi protocol.
/// Can require ASP.NET injected dependencies in the constructor and uses the Build() function to return value of `IServerApi`.
type ServerApi(logger: ILogger<ServerApi>, cfg: IConfiguration) =

    let getCatFactsClient () = new HttpClient(BaseAddress = Uri cfg["CatFactsBaseUrl"])

    /// Loads a page of Forge projects.
    let getCatFacts () = 
        async {
            use client : HttpClient = getCatFactsClient ()
            return! Rest.get<Shared.Api.CatFact list> client "/facts"
        }

    /// Builds the Fable.Remoting Api with handlers
    member this.Build() : Shared.Api.IServerApi =
        {
            GetCatFacts = getCatFacts
        }
