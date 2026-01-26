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

    // Dialog state - use typed DialogRef for imperative show/hide
    let dialog = Dialog.createRef()
    let confirmCount, setConfirmCount = Hook.useState 0

    // Drawer state
    let drawer = Drawer.createRef()

    // Tooltip state (for imperative demo)
    let tooltip = Tooltip.createRef()

    // Details state
    let details = Details.createRef()

    // Animation state
    let animation = Animation.createRef()

    // Button click counter
    let clickCount, setClickCount = Hook.useState 0

    view {
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
                template {
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
                template {
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
                template {
                    div {
                        style "display: flex; flex-direction: column; gap: 15px;"

                        slButton {
                            variantPrimary
                            onClick (fun _ -> Dialog.show dialog)
                            slIcon { slot' "prefix"; iconName "box-arrow-up-right" }
                            "Open Dialog"
                        }

                        p { $"Confirmed {confirmCount} times" }

                        slDialog {
                            Dialog.bind dialog
                            label' "Confirmation"

                            p { "Are you sure you want to proceed with this action?" }

                            div {
                                slot' "footer"
                                style "display: flex; gap: 10px; justify-content: flex-end;"

                                slButton {
                                    variantDefault
                                    onClick (fun _ -> Dialog.hide dialog)
                                    "Cancel"
                                }
                                slButton {
                                    variantPrimary
                                    onClick (fun _ ->
                                        setConfirmCount (confirmCount + 1)
                                        Dialog.hide dialog
                                    )
                                    "Confirm"
                                }
                            }
                        }
                    }
                }
            )

            // Drawer Demo
            card "Drawer with slDrawer" (
                template {
                    div {
                        style "display: flex; flex-direction: column; gap: 15px;"

                        slButton {
                            variantPrimary
                            onClick (fun _ -> Drawer.show drawer)
                            slIcon { slot' "prefix"; iconName "layout-sidebar" }
                            "Open Drawer"
                        }

                        slDrawer {
                            Drawer.bind drawer
                            label' "Settings Panel"

                            div {
                                style "display: flex; flex-direction: column; gap: 15px;"

                                slInput {
                                    label' "Setting 1"
                                    placeholder' "Enter value"
                                }

                                slSwitch { "Enable notifications" }

                                slSwitch { "Auto-save" }
                            }

                            div {
                                slot' "footer"
                                style "display: flex; gap: 10px; justify-content: flex-end;"

                                slButton {
                                    variantDefault
                                    onClick (fun _ -> Drawer.hide drawer)
                                    "Close"
                                }
                                slButton {
                                    variantPrimary
                                    onClick (fun _ -> Drawer.hide drawer)
                                    "Save"
                                }
                            }
                        }
                    }
                }
            )

            // Tooltip Demo
            card "Tooltip with slTooltip" (
                template {
                    div {
                        style "display: flex; flex-direction: column; gap: 15px;"

                        div {
                            style "display: flex; gap: 10px; flex-wrap: wrap;"

                            slTooltip {
                                content' "This is a helpful tooltip!"
                                slButton { variantPrimary; "Hover me" }
                            }

                            slTooltip {
                                content' "Tooltip on the right"
                                placement "right"
                                slButton { variantSuccess; "Right tooltip" }
                            }

                            slTooltip {
                                content' "Tooltip below"
                                placement "bottom"
                                slButton { variantNeutral; "Bottom tooltip" }
                            }
                        }

                        div {
                            style "display: flex; gap: 10px; align-items: center;"

                            slTooltip {
                                Tooltip.bind tooltip
                                content' "Controlled via ref!"
                                trigger' "manual"
                                slBadge { variantWarning; "Manual tooltip target" }
                            }

                            slButton {
                                sizeSmall
                                onClick (fun _ -> Tooltip.show tooltip)
                                "Show"
                            }
                            slButton {
                                sizeSmall
                                onClick (fun _ -> Tooltip.hide tooltip)
                                "Hide"
                            }
                        }
                    }
                }
            )

            // Details Demo
            card "Details with slDetails" (
                template {
                    div {
                        style "display: flex; flex-direction: column; gap: 15px;"

                        slDetails {
                            summary' "Click to expand"
                            "This content is hidden by default and revealed when you click the summary."
                        }

                        slDetails {
                            summary' "Another section"
                            open' true
                            "This section starts open. You can collapse it by clicking."
                        }

                        slDetails {
                            Details.bind details
                            summary' "Controlled via ref"
                            "This details panel can be controlled programmatically."
                        }

                        div {
                            style "display: flex; gap: 10px;"
                            slButton {
                                sizeSmall
                                variantPrimary
                                onClick (fun _ -> Details.show details)
                                "Expand"
                            }
                            slButton {
                                sizeSmall
                                variantNeutral
                                onClick (fun _ -> Details.hide details)
                                "Collapse"
                            }
                        }
                    }
                }
            )

            // Animation Demo
            card "Animation with slAnimation" (
                template {
                    div {
                        style "display: flex; flex-direction: column; gap: 15px;"

                        slAnimation {
                            Animation.bind animation
                            name' "bounce"
                            duration 1000
                            iterations 1
                            slBadge { variantPrimary; pill true; "Bounce me!" }
                        }

                        div {
                            style "display: flex; gap: 10px;"
                            slButton {
                                variantPrimary
                                onClick (fun _ -> Animation.play animation)
                                slIcon { slot' "prefix"; iconName "play-fill" }
                                "Play Animation"
                            }
                            slButton {
                                variantNeutral
                                onClick (fun _ -> Animation.stop animation)
                                slIcon { slot' "prefix"; iconName "stop-fill" }
                                "Stop"
                            }
                        }
                    }
                }
            )

            // Button Variants Demo
            card "Button Variants & Sizes" (
                template {
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
                template {
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
                template {
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
