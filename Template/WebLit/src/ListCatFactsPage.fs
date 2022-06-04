module WebLit.ListCatFactsPage

open Shared
open Elmish
open Lit
open Lit.Elmish
open Shared.Api
open Fable.Core.JsInterop

type Model = 
    {
        CatFacts: Api.CatFact array
        PageSize: Api.PageSize
    }

type Msg = 
    | LoadCatFacts of Api.CatFact list
    | SetPageSize of PageSize
    | RefreshFacts
    | OnError of System.Exception

let init () = 
    let model = { CatFacts = [||]; PageSize = 10 }
    model, Cmd.OfAsync.either Server.api.GetCatFacts model.PageSize LoadCatFacts OnError

let update msg model = 
    match msg with
    | LoadCatFacts facts ->
        { model with CatFacts = facts |> List.toArray }, Cmd.none
    | SetPageSize size -> 
        { model with PageSize = size }, Cmd.none
    | RefreshFacts -> 
        model, Cmd.OfAsync.either Server.api.GetCatFacts model.PageSize LoadCatFacts OnError
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
                <sl-button size="small" @click={Ev (fun e -> Utils.Grapnel.navigate($"/cat-fact/{catFact.Fact}"))}>View</sl-button>
            </td>
            <td>{catFact.Fact}</td>
        </tr>
        """

    html $"""
        <sl-breadcrumb style="margin: 10px;">
            <sl-breadcrumb-item>Cat Facts</sl-breadcrumb-item>
        </sl-breadcrumb>

        <sl-card>
            <div slot="header">
                Page Size {model.PageSize}
            </div>
            <fluent-slider 
                .min={5}
                .max={10}
                .value={model.PageSize}
                style="width: 200px"
                @change={Ev (fun e -> dispatch (SetPageSize e.target?valueAsNumber))}>
            </fluent-slider>
            <sl-button size="small" @click={Ev (fun e -> dispatch RefreshFacts)}>Refresh</sl-button>
        </sl-card>

        <table style="max-height: 300px; overflow-y: auto; margin-top: 10px">
            <thead>
                <tr>
                    <th style="width: 80px"></th>
                    <th style="width: 800px">Cat Fact</th>
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