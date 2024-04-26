module WebLit.ListCatFactsPage

open Shared
open Elmish
open Lit
open Lit.Elmish
open Shared.Api
open Fable.Core.JsInterop
open LitRouter
let private hmr = HMR.createToken()

type Model = 
    {
        CatFacts: Api.CatFact array
        PageSize: Api.PageSize
        PageNumber: Api.PageNumber
    }

type Msg = 
    | LoadCatFacts of Api.CatFact list
    | SetPageSize of PageSize
    | NextPage
    | PrevPage
    | RefreshFacts
    | OnError of System.Exception

let init () = 
    let model = { CatFacts = [||]; PageSize = 10; PageNumber = 1 }
    model, Cmd.OfAsync.either Server.api.GetCatFacts (model.PageSize, model.PageNumber) LoadCatFacts OnError

let update msg model = 
    match msg with
    | LoadCatFacts facts -> 
        { model with CatFacts = facts |> List.toArray }, Cmd.none
    | SetPageSize size -> 
        { model with PageSize = size }, Cmd.none
    | NextPage -> 
        { model with PageNumber = model.PageNumber + 1 }, Cmd.ofMsg RefreshFacts
    | PrevPage -> 
        match model.PageNumber - 1 with
        | page when page < 1 -> model, Cmd.none
        | page -> { model with PageNumber = page }, Cmd.ofMsg RefreshFacts
    | RefreshFacts -> 
        model, Cmd.OfAsync.either Server.api.GetCatFacts (model.PageSize, model.PageNumber) LoadCatFacts OnError
    | OnError ex ->
        Fable.Core.JS.console.error $"Error: {ex.Message}"
        model, Cmd.none

[<HookComponent>]
let Page() = 
    Hook.useHmr(hmr)
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
                <sl-button size="small" @click={fun _ -> Router.navigatePath($"/cat-fact/{catFact.Fact}")}>View</sl-button>
            </td>
            <td>{catFact.Fact}</td>
        </tr>
        """

    html $"""
        <sl-breadcrumb style="margin: 10px;">
            <sl-breadcrumb-item @click={fun () -> Router.navigatePath("/")}>Home</sl-breadcrumb-item>
            <sl-breadcrumb-item>Cat Facts</sl-breadcrumb-item>
        </sl-breadcrumb>

        <sl-button-group style="margin-top: 20px; padding: 5px 0;">
            <sl-button @click={Ev (fun e -> dispatch PrevPage)}>
                <bs-icon src="chevron-left" color="white" size="14px"></bs-icon>
                Previous
            </sl-button>
            <sl-button>
                Page: {model.PageNumber}
            </sl-button>            
            <sl-dropdown>
                <sl-button slot="trigger" caret>Limit: {model.PageSize}</sl-button>
                <sl-menu id="menu-page">
                    <sl-menu-item id="menu-item-page">
                        <fluent-slider 
                            .min={1}
                            .max={20}
                            .value={model.PageSize}
                            style="width: 200px; height: 70px;"
                            @change={Ev (fun e -> dispatch (SetPageSize e.target?valueAsNumber))}
                            @blur={Ev (fun e -> dispatch RefreshFacts)}>
                            <fluent-slider-label position="1">1</fluent-slider-label>
                            <fluent-slider-label position="10">10</fluent-slider-label>
                            <fluent-slider-label position="20">20</fluent-slider-label>
                        </fluent-slider>
                    </sl-menu-item>
                </sl-menu>
            </sl-dropdown>
            <sl-button @click={Ev (fun e -> dispatch NextPage)}>
                Next
                <bs-icon src="chevron-right" color="white" size="14px"></bs-icon>
            </sl-button>
        </sl-button-group>
            
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