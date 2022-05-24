module WebLit.App

open Fable.Core
open Fable.Core.JsInterop
open Shared
open Elmish
open Lit
open Lit.Elmish

// Import FluentUI
let allComponents: obj = importMember "@fluentui/web-components"
let provideFluentDesignSystem: unit -> obj = importMember "@fluentui/web-components"
provideFluentDesignSystem()?register(allComponents)

type Model = 
    {
        CatFacts: Api.CatFact array
    }

type Msg = 
    | LoadCatFacts of Api.CatFact list
    | OnError of System.Exception

let init () = 
    let model = { CatFacts = [||] }
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

    let model, dispatch = Hook.useElmish(init, update)
    
    html $"""
        <h1>Cat Facts</h1>
        <fluent-data-grid style="max-height: 30em; overflow-y: auto;" .rowsData={model.CatFacts}>
        </fluent-data-grid>
        """
