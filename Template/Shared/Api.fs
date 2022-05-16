module Shared.Api
open System

/// Defines how routes are generated on server and mapped from client
let routerPaths typeName method = sprintf "/api/%s" method

type CatFact = {
    Text: string
    CreatedAt: DateTimeOffset
}


/// A type that specifies the communication protocol between client and server
/// to learn more, read the docs at https://zaid-ajaj.github.io/Fable.Remoting/src/basics.html
type IServerApi = {
    GetCatFacts: unit -> Async<CatFact list>
}