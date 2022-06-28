module WebApi.ApiBuilder

open Serilog
open Microsoft.Extensions.Configuration
open Shared
open Shared.Api
open System.Net.Http
open System
open Microsoft.AspNetCore.Http

/// An implementation of the Shared IServerApi protocol.
/// Can require ASP.NET injected dependencies in the constructor and uses the Build() function to return value of `IServerApi`.
type ServerApi(logger: ILogger, cfg: IConfiguration) =

    let getCatFactsClient () = new HttpClient(BaseAddress = Uri cfg["CatFactsBaseUrl"])

    /// Loads a page of CAT FACTS!
    let getCatFacts (pageSize: PageSize, pageNumber: PageNumber) = 
        async {
            use client = getCatFactsClient ()
            let! page = Rest.get<CatFactPage> client $"/facts?page={pageNumber}&limit={pageSize}"

            // Replace periods with safe ascii period because Grapnel router chokes on having periods in the url route
            return page.Data |> List.map (fun row -> { row with Fact = row.Fact.Replace('.', '․')})
        }

    /// Builds the Fable.Remoting Api with handlers
    member this.Build(ctx: HttpContext) : Shared.Api.IServerApi =
        {
            GetCatFacts = getCatFacts
        }
