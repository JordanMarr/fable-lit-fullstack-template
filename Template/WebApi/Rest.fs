module WebApi.Rest

open Microsoft.Extensions.Logging
open System.Net.Http
open Newtonsoft.Json
open System
open System.Net.Http.Headers

/// Makes REST GET call and return a 'Result.
let get<'Result> (client: HttpClient) (relativeUrl: string) = 
    task {
        let! resp = client.GetAsync(relativeUrl)
        let! content = resp.Content.ReadAsStringAsync()
        let result = JsonConvert.DeserializeObject<'Result>(content)
        return result
    }
    |> Async.AwaitTask
