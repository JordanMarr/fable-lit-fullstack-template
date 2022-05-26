module WebLit.ListCatFactsPage

open Shared
open Elmish
open Lit
open Lit.Elmish
open Shared.Api

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

[<HookComponent>]
let Page() = 
    let model, dispatch = Hook.useElmish(init, update)

    let emptyRow () = 
        html $"""
        <tr>
            <td></td>
            <td colspan="2">Fetching cat facts...</td>
        </tr>
        """

    let renderRow (catFact: CatFact) = 
        html $"""
        <tr>
            <td>
                <fluent-button @click={Ev (fun e -> Utils.Grapnel.navigate($"/cat-fact/{catFact.Text}"))}>View</fluent-button>
            </td>
            <td>{catFact.Text}</td>
            <td>{catFact.CreatedAt.Date.ToLongDateString()}</td>
        </tr>
        """

    html $"""
        <fluent-breadcrumb style="margin: 10px;">
            <fluent-breadcrumb-item>Cat Facts</fluent-breadcrumb-item>
        </fluent-breadcrumb>

        <table style="max-height: 300px; overflow-y: auto; margin-top: 10px">
            <thead>
                <tr>
                    <th style="width: 80px"></th>
                    <th style="width: 800px">Cat Fact</th>
                    <th style="width: 200px">Created</th>
                </tr>
            </thead>
            <tbody>
                { 
                    match model.CatFacts with
                    | [||] -> [| emptyRow () |]
                    | catFacts -> catFacts |> Array.map renderRow
                }
            </tbody>
        </table>
        """