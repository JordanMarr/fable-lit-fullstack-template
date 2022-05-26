module WebLit.App

open Shared
open Elmish
open Lit
open Utils
open Utils.Grapnel


registerComponents()
Grapnel.init()
let router = Router.Create()

type Page = 
    | Welcome
    | ListCatFacts
    | ViewCatFact of fact: string

type Model = 
    {
        CurrentPage: Page
        CatFacts: Api.CatFact array
    }

type Msg = 
    | LoadCatFacts of Api.CatFact list
    | OnError of System.Exception

let init () = 
    let model = { CurrentPage = Welcome; CatFacts = [||] }
    model, Cmd.OfAsync.either Server.api.GetCatFacts () LoadCatFacts OnError

let update msg model = 
    match msg with
    | LoadCatFacts facts ->
        { model with CatFacts = facts |> List.toArray }, Cmd.none
    | OnError ex ->
        Fable.Core.JS.console.error $"Error: {ex.Message}"
        model, Cmd.none

[<LitElement("my-app")>]
let MyApp() =
    let _ = LitElement.init(fun cfg -> cfg.useShadowDom <- false)
    let currentPage, setCurrentPage = Hook.useState Page.Welcome

    Hook.useEffectOnce(fun () -> 
        router.get("/", fun _ -> setCurrentPage Page.Welcome)
        router.get("/cat-facts", fun _ -> setCurrentPage Page.ListCatFacts)
        router.get("/cat-fact/:fact", fun (req: Req<{| fact: string |}>) -> setCurrentPage (Page.ViewCatFact req.``params``.fact))
    )
    
    html $"""
        <nav>
            <fluent-anchor appearance="hypertext" href="#" @click={fun _ -> router.navigate("/")}>Welcome</fluent-anchor>
            | 
            <fluent-anchor appearance="hypertext" href="#" @click={fun _ -> router.navigate("/cat-facts")}>View Cat Facts</fluent-anchor>
        </nav>
        <main style="margin: 20px;">
            {
                match currentPage with
                | Welcome -> WelcomePage.Page()
                | ListCatFacts -> ListCatFactsPage.Page()
                | ViewCatFact fact -> ViewCatFactPage.Page(fact)
            }
        </main>
        """
