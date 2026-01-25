module WebLit.DslDemoPage

open Fable.Core.JsInterop
open Lit
open LitStore
open Fable.Lit.Dsl

let private hmr = HMR.createToken()

/// A reusable card component using the DSL.
let private card (title: string) (content: Node) =
    el "sl-card" {
        class' "card-header"
        div {
            attr "slot" "header"
            strong { title }
        }
        content
    }

/// A badge component.
let private badge (variant: string) (text: string) =
    el "sl-badge" {
        attr "variant" variant
        text
    }

/// Demo of basic elements and text.
let private basicElementsDemo () =
    card "Basic Elements" (
        html {
            p { "This paragraph is rendered using the DSL." }
            p {
                "You can mix "
                strong { "bold" }
                ", "
                em { "italic" }
                ", and "
                code { "code" }
                " text."
            }
            hr { nothing }
            div {
                class' "demo-box"
                style "padding: 10px; background: #f0f0f0; border-radius: 4px;"
                "A styled div with inline styles."
            }
        }
    )

/// Demo of lists using for loops.
let private listDemo () =
    let items = [ "Whiskers"; "Mittens"; "Shadow"; "Luna"; "Oliver" ]

    card "Lists with For Loops" (
        html {
            p { "Cat names:" }
            ul {
                for item in items do
                    li {
                        el "sl-icon" { attr "name" "heart" }
                        $" {item}"
                    }
            }
        }
    )

/// Demo of conditionals.
let private conditionalDemo (isLoggedIn: bool) (username: string) =
    card "Conditional Rendering" (
        html {
            if isLoggedIn then
                p {
                    el "sl-icon" { attr "name" "person-fill" }
                    $" Welcome back, {username}!"
                }
                el "sl-button" {
                    attr "variant" "danger"
                    attr "outline" "true"
                    "Log Out"
                }
            else
                p { "Please log in to continue." }
                el "sl-button" {
                    attr "variant" "primary"
                    "Log In"
                }
        }
    )

/// Demo of custom elements (Shoelace components).
let private customElementsDemo () =
    card "Custom Elements (Shoelace)" (
        html {
            div {
                style "display: flex; gap: 10px; flex-wrap: wrap; margin-bottom: 15px;"
                badge "primary" "Primary"
                badge "success" "Success"
                badge "neutral" "Neutral"
                badge "warning" "Warning"
                badge "danger" "Danger"
            }

            el "sl-button-group" {
                attr "label" "Actions"
                el "sl-button" {
                    attr "variant" "primary"
                    el "sl-icon" { attr "slot" "prefix"; attr "name" "gear" }
                    "Settings"
                }
                el "sl-button" {
                    attr "variant" "default"
                    el "sl-icon" { attr "slot" "prefix"; attr "name" "envelope" }
                    "Messages"
                }
                el "sl-button" {
                    attr "variant" "default"
                    el "sl-icon" { attr "slot" "prefix"; attr "name" "bell" }
                    "Notifications"
                }
            }
        }
    )

/// Demo of interop with raw Lit templates.
let private interopDemo () =
    card "Lit Interop" (
        html {
            p { "DSL content above..." }

            // Embed a raw Lit template inside the DSL
            lit (Lit.html $"""
                <sl-alert variant="primary" open>
                    <sl-icon slot="icon" name="info-circle"></sl-icon>
                    This alert is rendered using a raw Lit template embedded in the DSL!
                </sl-alert>
            """)

            p { "...and DSL content below." }
        }
    )

/// Demo of a data table.
let private tableDemo () =
    let catFacts = [
        {| id = 1; fact = "Cats sleep 12-16 hours per day"; verified = true |}
        {| id = 2; fact = "A group of cats is called a clowder"; verified = true |}
        {| id = 3; fact = "Cats have over 20 vocalizations"; verified = false |}
        {| id = 4; fact = "The first cat in space was French"; verified = true |}
    ]

    card "Data Table" (
        html {
            table {
                style "width: 100%; border-collapse: collapse;"
                thead {
                    tr {
                        th { style "text-align: left; padding: 8px;"; "#" }
                        th { style "text-align: left; padding: 8px;"; "Fact" }
                        th { style "text-align: center; padding: 8px;"; "Verified" }
                    }
                }
                tbody {
                    for fact in catFacts do
                        tr {
                            td { style "padding: 8px;"; $"{fact.id}" }
                            td { style "padding: 8px;"; fact.fact }
                            td {
                                style "text-align: center; padding: 8px;"
                                if fact.verified then
                                    el "sl-icon" {
                                        attr "name" "check-circle-fill"
                                        style "color: green;"
                                    }
                                else
                                    el "sl-icon" {
                                        attr "name" "x-circle-fill"
                                        style "color: red;"
                                    }
                            }
                        }
                }
            }
        }
    )

/// Demo of nested components.
let private nestedDemo () =
    let alert (variant: string) (message: string) =
        el "sl-alert" {
            attr "variant" variant
            attr "open" "true"
            el "sl-icon" {
                attr "slot" "icon"
                attr "name" (
                    match variant with
                    | "success" -> "check2-circle"
                    | "warning" -> "exclamation-triangle"
                    | "danger" -> "exclamation-octagon"
                    | _ -> "info-circle"
                )
            }
            message
        }

    card "Nested Components" (
        html {
            div {
                style "display: flex; flex-direction: column; gap: 10px;"
                alert "success" "Operation completed successfully!"
                alert "warning" "Please review before continuing."
                alert "danger" "An error occurred."
            }
        }
    )

/// Demo of property bindings and event handlers.
let private propsAndEventsDemo
    (inputValue: string, setInputValue: string -> unit)
    (switchEnabled: bool, setSwitchEnabled: bool -> unit)
    (clickCount: int, setClickCount: int -> unit) =

    card "Property & Event Bindings" (
        html {
            // Input with property binding and event handler
            div {
                style "margin-bottom: 15px;"
                el "sl-input" {
                    attr "label" "Text Input (using .value property)"
                    prop "value" inputValue
                    onInput (fun ev -> setInputValue(ev.target?value))
                }
            }

            // Display the current input value
            p {
                "Current value: "
                strong { inputValue }
            }

            hr { nothing }

            // Switch with property binding
            div {
                style "margin-bottom: 15px;"
                el "sl-switch" {
                    prop "checked" switchEnabled
                    on "sl-change" (fun ev -> setSwitchEnabled(ev.target?``checked``))
                    "Enable feature"
                }
            }

            // Conditional content based on switch
            if switchEnabled then
                el "sl-alert" {
                    attr "variant" "success"
                    attr "open" "true"
                    el "sl-icon" { attr "slot" "icon"; attr "name" "check2-circle" }
                    "Feature is now enabled!"
                }

            hr { () }

            // Button with click event
            div {
                style "display: flex; gap: 10px; align-items: center;"
                el "sl-button" {
                    attr "variant" "primary"
                    onClick (fun _ -> setClickCount(clickCount + 1))
                    el "sl-icon" { attr "slot" "prefix"; attr "name" "hand-index-thumb" }
                    "Click me!"
                }
                span { $"Clicked {clickCount} times" }
            }
        }
    )

[<HookComponent>]
let Page() =
    Hook.useHmr(hmr)
    let ctx = Hook.useStore(AppContext.store)
    let isLoggedIn = not (System.String.IsNullOrWhiteSpace ctx.Username)

    // State for the props and events demo
    let inputValue, setInputValue = Hook.useState "Hello, DSL!"
    let switchEnabled, setSwitchEnabled = Hook.useState false
    let clickCount, setClickCount = Hook.useState 0

    // Render the entire page using the DSL
    html {
        el "sl-breadcrumb" {
            style "margin: 10px;"
            el "sl-breadcrumb-item" { "Home" }
            el "sl-breadcrumb-item" { "DSL Demo" }
        }

        h1 { "Fable.Lit.Dsl Demo" }
        p {
            class' "lead"
            "This page demonstrates the F# computation expression DSL for building Lit templates."
        }

        div {
            style "display: grid; grid-template-columns: repeat(auto-fit, minmax(400px, 1fr)); gap: 20px; margin-top: 20px;"

            basicElementsDemo ()
            listDemo ()
            conditionalDemo isLoggedIn ctx.Username
            customElementsDemo ()
            interopDemo ()
            tableDemo ()
            nestedDemo ()
            propsAndEventsDemo (inputValue, setInputValue) (switchEnabled, setSwitchEnabled) (clickCount, setClickCount)
        }
    }
    |> Renderer.render
