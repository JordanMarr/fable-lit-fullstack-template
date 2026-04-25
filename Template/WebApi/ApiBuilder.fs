module WebApi.ApiBuilder

open Serilog
open Microsoft.Extensions.Configuration
open Shared.Api
open System.Net.Http
open System

/// An implementation of the Shared IServerApi protocol.
/// Can require ASP.NET injected dependencies in the constructor.
type ServerApi(logger: ILogger, cfg: IConfiguration) =

    let getCatFactsClient () = new HttpClient(BaseAddress = Uri cfg["CatFactsBaseUrl"])
    
    interface IServerApi with

        member _.GetCatFacts(pageSize, pageNumber) = 
            async {
                use client = getCatFactsClient ()
                let! page = Rest.get<CatFactPage> client $"/facts?page={pageNumber}&limit={pageSize}"

                // Replace periods with safe ascii period because Grapnel router chokes on having periods in the url route
                return page.Data |> List.map (fun row -> { row with Fact = row.Fact.Replace('.', '․') })
            }
