module WebLit.ShoelaceDslDemoPage

open Fable.Core.JsInterop
open Lit
open LitStore
open Fable.Lit.Dsl
open Fable.Lit.Dsl.Shoelace

let private hmr = HMR.createToken()

/// A reusable card component using the Shoelace DSL.
let private card (title: string) (content: Node) =
    slCard {
        class' "card-header"
        div {
            slot' "header"
            strong { title }
        }
        content
    }

[<HookComponent>]
let Page() =
    Hook.useHmr(hmr)

    // Form state
    let username, setUsername = Hook.useState ""
    let email, setEmail = Hook.useState ""
    let formSubmitted, setFormSubmitted = Hook.useState false

    // Toggle state
    let featureEnabled, setFeatureEnabled = Hook.useState false

    // Dialog state - use ref for imperative show/hide
    // Shoelace dialogs must be opened via .show() and .hide(), not by setting the open property.
    let dialogRef = Hook.useRef<obj option>(None)
    let showDialogRef () = dialogRef.Value |> Option.iter (fun el -> el?show() |> ignore)
    let hideDialogRef () = dialogRef.Value |> Option.iter (fun el -> el?hide() |> ignore)
    let confirmCount, setConfirmCount = Hook.useState 0

    // Button click counter
    let clickCount, setClickCount = Hook.useState 0

    html {
        slBreadcrumb {
            style "margin: 10px;"
            slBreadcrumbItem { "Home" }
            slBreadcrumbItem { "Shoelace DSL Demo" }
        }

        h1 { "Fable.Lit.Dsl.Shoelace Demo" }
        p {
            class' "lead"
            "This page demonstrates the Shoelace-specific DSL layer built on top of Fable.Lit.Dsl."
        }

        div {
            style "display: grid; grid-template-columns: repeat(auto-fit, minmax(400px, 1fr)); gap: 20px; margin-top: 20px;"

            // Form Demo
            card "Form with slInput + slButton" (
                html {
                    div {
                        style "display: flex; flex-direction: column; gap: 15px;"

                        slInput {
                            label' "Username"
                            placeholder' "Enter your username"
                            value' username
                            clearable true
                            helpText "Your unique username"
                            onSlInput (fun ev -> setUsername(ev?target?value))
                        }

                        slInput {
                            label' "Email"
                            type'' "email"
                            placeholder' "Enter your email"
                            value' email
                            clearable true
                            helpText "We'll never share your email"
                            onSlInput (fun ev -> setEmail(ev?target?value))
                        }

                        div {
                            style "display: flex; gap: 10px;"
                            slButton {
                                variantPrimary
                                onClick (fun _ ->
                                    setFormSubmitted true
                                    printfn "Form submitted: %s, %s" username email
                                )
                                slIcon { slot' "prefix"; iconName "check2-circle" }
                                "Submit"
                            }
                            slButton {
                                variantDefault
                                outline true
                                onClick (fun _ ->
                                    setUsername ""
                                    setEmail ""
                                    setFormSubmitted false
                                )
                                slIcon { slot' "prefix"; iconName "x-circle" }
                                "Reset"
                            }
                        }

                        if formSubmitted && username <> "" then
                            slAlert {
                                variantSuccess
                                open' true
                                closable true
                                slIcon { slot' "icon"; iconName "check2-circle" }
                                $"Form submitted for user: {username}"
                            }
                    }
                }
            )

            // Toggle Demo
            card "Toggle with slSwitch" (
                html {
                    div {
                        style "display: flex; flex-direction: column; gap: 15px;"

                        slSwitch {
                            checked' featureEnabled
                            onSlChange (fun ev -> setFeatureEnabled(ev?target?``checked``))
                            "Enable dark mode"
                        }

                        slSwitch {
                            checked' (not featureEnabled)
                            disabled' true
                            "Disabled switch (inverted)"
                        }

                        if featureEnabled then
                            slAlert {
                                variantPrimary
                                open' true
                                slIcon { slot' "icon"; iconName "moon" }
                                "Dark mode is now enabled!"
                            }
                        else
                            slAlert {
                                variantNeutral
                                open' true
                                slIcon { slot' "icon"; iconName "sun" }
                                "Light mode is active."
                            }
                    }
                }
            )

            // Dialog Demo
            card "Dialog with slDialog" (
                html {
                    div {
                        style "display: flex; flex-direction: column; gap: 15px;"

                        slButton {
                            variantPrimary
                            onClick (fun _ -> showDialogRef())
                            slIcon { slot' "prefix"; iconName "box-arrow-up-right" }
                            "Open Dialog"
                        }

                        p { $"Confirmed {confirmCount} times" }

                        slDialog {
                            //bindRef (fun el -> dialogRef.Value <- Some el)
                            //bindRef (fun part ->
                            //    let el = part?element 
                            //    printfn $"Dialog element bound: {el}"
                            //    dialogRef.Value <- Some el 
                            //)
                            bindRef (fun el -> 
                                printfn $"Ref received: {el}"  // should read: "Ref received: [object HTMLSlDialogElement]"
                                dialogRef.Value <- Some el 
                            )
                            label' "Confirmation"
                            onSlRequestClose (fun _ -> hideDialogRef())

                            p { "Are you sure you want to proceed with this action?" }

                            div {
                                slot' "footer"
                                style "display: flex; gap: 10px; justify-content: flex-end;"

                                slButton {
                                    variantDefault
                                    onClick (fun _ -> hideDialogRef())
                                    "Cancel"
                                }
                                slButton {
                                    variantPrimary
                                    onClick (fun _ ->
                                        setConfirmCount (confirmCount + 1)
                                        hideDialogRef()
                                    )
                                    "Confirm"
                                }
                            }
                        }
                    }
                }
            )

            // Button Variants Demo
            card "Button Variants & Sizes" (
                html {
                    div {
                        style "display: flex; flex-direction: column; gap: 15px;"

                        // Variants
                        div {
                            style "display: flex; gap: 10px; flex-wrap: wrap;"
                            slButton { variantDefault; "Default" }
                            slButton { variantPrimary; "Primary" }
                            slButton { variantSuccess; "Success" }
                            slButton { variantNeutral; "Neutral" }
                            slButton { variantWarning; "Warning" }
                            slButton { variantDanger; "Danger" }
                        }

                        // Sizes
                        div {
                            style "display: flex; gap: 10px; align-items: center;"
                            slButton { variantPrimary; sizeSmall; "Small" }
                            slButton { variantPrimary; sizeMedium; "Medium" }
                            slButton { variantPrimary; sizeLarge; "Large" }
                        }

                        // Styles
                        div {
                            style "display: flex; gap: 10px; flex-wrap: wrap;"
                            slButton { variantPrimary; outline true; "Outline" }
                            slButton { variantPrimary; pill true; "Pill" }
                            slButton { variantPrimary; circle true; slIcon { iconName "gear" } }
                            slButton { variantPrimary; loading true; "Loading" }
                            slButton { variantPrimary; disabled' true; "Disabled" }
                        }
                    }
                }
            )

            // Badges Demo
            card "Badges & Tags" (
                html {
                    div {
                        style "display: flex; flex-direction: column; gap: 15px;"

                        // Badges
                        div {
                            style "display: flex; gap: 10px; flex-wrap: wrap;"
                            slBadge { variantPrimary; "Primary" }
                            slBadge { variantSuccess; "Success" }
                            slBadge { variantNeutral; "Neutral" }
                            slBadge { variantWarning; "Warning" }
                            slBadge { variantDanger; "Danger" }
                        }

                        // Pill badges
                        div {
                            style "display: flex; gap: 10px; flex-wrap: wrap;"
                            slBadge { variantPrimary; pill true; "Pill Primary" }
                            slBadge { variantSuccess; pill true; "Pill Success" }
                        }

                        // Tags
                        div {
                            style "display: flex; gap: 10px; flex-wrap: wrap;"
                            slTag { variantPrimary; "Primary Tag" }
                            slTag { variantSuccess; "Success Tag" }
                            slTag { variantNeutral; removable true; "Removable" }
                        }
                    }
                }
            )

            // Click Counter Demo
            card "Event Handling" (
                html {
                    div {
                        style "display: flex; flex-direction: column; gap: 15px;"

                        div {
                            style "display: flex; gap: 10px; align-items: center;"
                            slButton {
                                variantPrimary
                                onClick (fun _ -> setClickCount (clickCount + 1))
                                slIcon { slot' "prefix"; iconName "hand-index-thumb" }
                                "Click me!"
                            }
                            slBadge {
                                variantPrimary
                                pill true
                                $"{clickCount}"
                            }
                        }

                        slButton {
                            variantDanger
                            outline true
                            disabled' (clickCount = 0)
                            onClick (fun _ -> setClickCount 0)
                            slIcon { slot' "prefix"; iconName "arrow-counterclockwise" }
                            "Reset Counter"
                        }
                    }
                }
            )
        }
    }
    |> Renderer.render
