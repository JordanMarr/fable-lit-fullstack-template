module WebLit.ListCatFactsPage

open Shared
open Elmish
open Lit
open Lit.Elmish
open Shared.Api
open Fable.Core.JsInterop
open LitRouter
open Fable.Lit.Dsl
open Fable.Lit.Dsl.Shoelace
open Ctrls.Bootstrap

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

    html {
        slBreadcrumb {
            style "margin: 10px;"
            slBreadcrumbItem {
                onClick (fun _ -> Router.navigatePath("/"))
                "Home"
            }
            slBreadcrumbItem { "Cat Facts" }
        }

        slButtonGroup {
            style "margin-top: 20px; padding: 5px 0;"

            slButton {
                onClick (fun _ -> dispatch PrevPage)
                bsIcon "chevron-left" "white" "14px"
                "Previous"
            }

            slButton { $"Page: {model.PageNumber}" }

            slDropdown {
                slButton {
                    slot' "trigger"
                    caret true
                    $"Limit: {model.PageSize}"
                }
                slMenu {
                    id "menu-page"
                    slMenuItem {
                        id "menu-item-page"
                        el "fluent-slider" {
                            prop "min" 1
                            prop "max" 20
                            prop "value" model.PageSize
                            style "width: 200px; height: 70px;"
                            on "change" (fun e -> dispatch (SetPageSize e.target?valueAsNumber))
                            on "blur" (fun _ -> dispatch RefreshFacts)

                            el "fluent-slider-label" { attr "position" "1"; "1" }
                            el "fluent-slider-label" { attr "position" "10"; "10" }
                            el "fluent-slider-label" { attr "position" "20"; "20" }
                        }
                    }
                }
            }

            slButton {
                onClick (fun _ -> dispatch NextPage)
                "Next"
                bsIcon "chevron-right" "white" "14px"
            }
        }

        table {
            style "max-height: 300px; overflow-y: auto; margin-top: 10px"
            thead {
                tr {
                    th { style "width: 80px"; nothing }
                    th { style "width: 800px"; "Cat Fact" }
                }
            }
            tbody {
                match model.CatFacts with
                | [||] -> 
                    tr {
                        td { nothing }
                        td { attr "colspan" "2"; "Fetching cat facts..." }
                    }
                | catFacts ->
                    for catFact in catFacts do
                        tr {
                            td {
                                slButton {
                                    sizeSmall
                                    onClick (fun _ -> Router.navigatePath($"/cat-fact/{catFact.Fact}"))
                                    "View"
                                }
                            }
                            td { catFact.Fact }
                        }
            }
        }
    }
    |> Renderer.render