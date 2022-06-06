module Shared.Api
open System

/// Defines how routes are generated on server and mapped from client
let routerPaths typeName method = sprintf "/api/%s" method

type CatFact = { Fact: string }
type CatFactPage = { Current_Page: int; Data: CatFact list }

type PageSize = int
type PageNumber = int

/// A type that specifies the communication protocol between client and server.
/// To learn more, read the docs at https://zaid-ajaj.github.io/Fable.Remoting/src/basics.html
type IServerApi = {
    GetCatFacts: PageSize * PageNumber -> Async<CatFact list>
}