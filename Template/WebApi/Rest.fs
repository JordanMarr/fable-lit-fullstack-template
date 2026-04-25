module WebApi.Rest

open System.Net.Http
open System.Text.Json

let private jsonOptions =
    JsonSerializerOptions(PropertyNameCaseInsensitive = true)

/// Makes REST GET call and returns a 'Result.
let get<'Result> (client: HttpClient) (relativeUrl: string) =
    task {
        let! resp = client.GetAsync(relativeUrl)
        let! content = resp.Content.ReadAsStringAsync()
        return JsonSerializer.Deserialize<'Result>(content, jsonOptions)
    }
    |> Async.AwaitTask
