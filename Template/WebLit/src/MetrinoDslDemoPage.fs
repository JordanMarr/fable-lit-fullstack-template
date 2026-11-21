module WebLit.MetrinoDslDemoPage

open Lit
open Fable.Lit.Dsl
open Fable.Lit.Dsl.Metrino

let private hmr = HMR.createToken()

/// Global toast notifications (the <metro-toast> host is created lazily on first Show).
let private toasts = new ToastHost()

/// Metro accent colors: name, accent, light, dark.
let private accents =
    [
        "cobalt",  "#0050ef", "#4a8aff", "#0035a8"
        "red",     "#e51400", "#ff4438", "#b40000"
        "orange",  "#fa6800", "#ff9a3c", "#c64900"
        "mango",   "#f09609", "#ffc04a", "#be7200"
        "lime",    "#a4c400", "#d5f200", "#749200"
        "green",   "#00a300", "#00d500", "#007200"
        "teal",    "#00aba9", "#00ddda", "#007a78"
        "purple",  "#a200ff", "#d04aff", "#7200ca"
        "magenta", "#d80073", "#ff2d9b", "#a60052"
    ]

/// Metrino components read CSS custom properties (which inherit through shadow DOM),
/// so setting them on a container themes everything inside it.
let private accentStyle (accentName: string) =
    let _, accent, light, dark =
        accents |> List.tryFind (fun (n, _, _, _) -> n = accentName) |> Option.defaultValue accents.Head
    $"--metro-accent: {accent}; --metro-accent-light: {light}; --metro-accent-dark: {dark};"

/// A section within a pivot item.
let private section (title: string) (content: Node) =
    div {
        style "display: flex; flex-direction: column; gap: 14px; min-width: 280px; flex: 1;"
        h3 { style "font-weight: 300; font-size: 24px; margin: 0;"; title }
        content
    }

[<HookComponent>]
let Page() =
    Hook.useHmr(hmr)

    // Accent color
    let accent, setAccent = Hook.useState "cobalt"
    let accentPicker = Hook.useMemo(fun () -> MetroRef.create())

    // Input state
    let name, setName = Hook.useState ""
    let notifications, setNotifications = Hook.useState true
    let agreed, setAgreed = Hook.useState false
    let volume, setVolume = Hook.useState 40.0
    let stars, setStars = Hook.useState 3

    // Tiles
    let unread, setUnread = Hook.useState 3
    let liveTile = Hook.useMemo(fun () -> MetroRef.create())

    // Dialog
    let dialog = Hook.useMemo(fun () -> MetroRef.create())
    let saveCount, setSaveCount = Hook.useState 0

    // Some components get their data imperatively, after they have rendered.
    Hook.useEffectOnce(fun () ->
        accentPicker |> MetroRef.setOptions (accents |> List.map (fun (n, _, _, _) -> n))
        liveTile |> MetroRef.setItems [
            box {| title = "Fable.Lit.Dsl"; message = "Typed F# templates for Lit" |}
            box {| title = "Metrino"; message = "Metro web components by Angel Munoz" |}
            box {| title = "Live tiles"; message = "Cycle through short updates" |}
        ]
    )

    view {
        div {
            // Metro look: black canvas, light typography, scoped accent tokens
            style $"""background: #000; color: #fff; padding: 24px; font-family: "Segoe UI", system-ui, sans-serif; {accentStyle accent}"""

            h1 { style "font-weight: 300; font-size: 42px; margin: 0 0 4px 0;"; "Fable.Lit.Dsl.Metrino Demo" }
            p {
                style "color: rgba(255, 255, 255, 0.6); margin: 0 0 20px 0;"
                "Windows Phone / Windows 8 Metro components from "
                a { href "https://github.com/AngelMunoz/metrino"; target "_blank"; "Metrino" }
                ", built with the F# DSL on top of Fable.Lit.Dsl."
            }

            div {
                style "max-width: 240px; margin-bottom: 12px;"
                metroComboBox {
                    MetroRef.bind accentPicker
                    placeholder' "Accent color"
                    value' accent
                    onSelectionChanged (fun e -> setAccent (detail<{| selectedValue: string |}> e).selectedValue)
                }
            }

            metroPivot {
                // ----------------------------------------------------------------
                metroPivotItem {
                    header' "inputs"

                    div {
                        style "display: flex; flex-wrap: wrap; gap: 40px; padding-top: 16px;"

                        section "Text & toggles" (
                            template {
                                metroTextBox {
                                    label' "Your name"
                                    placeholder' "Type to update the greeting"
                                    value' name
                                    onMetroInput (fun e -> setName (detail<{| value: string |}> e).value)
                                }

                                metroToggleSwitch {
                                    on' notifications
                                    onMetroChange (fun e -> setNotifications (detail<{| on: bool |}> e).on)
                                    "Notifications"
                                }

                                metroCheckBox {
                                    checked' agreed
                                    onMetroChange (fun e -> setAgreed (detail<{| ``checked``: bool |}> e).``checked``)
                                    "I agree to the terms"
                                }
                            }
                        )

                        section "Ranges" (
                            template {
                                div {
                                    $"Volume: {int volume}"
                                    metroSlider {
                                        min' 0
                                        max' 100
                                        step' 1
                                        value' volume
                                        onMetroChange (fun e -> setVolume (detail<{| value: float |}> e).value)
                                    }
                                }

                                div {
                                    $"Rating: {stars} / 5"
                                    metroRating {
                                        max' 5
                                        value' stars
                                        onMetroChange (fun e -> setStars (detail<{| value: int |}> e).value)
                                    }
                                }
                            }
                        )

                        section "Summary" (
                            template {
                                metroInfoBar {
                                    if agreed then severitySuccess else severityWarning
                                    title' (if name = "" then "Hello!" else $"Hello, {name}!")
                                    if agreed then "Thanks for agreeing to the terms." else "Please agree to the terms."
                                }
                                p {
                                    style "color: rgba(255, 255, 255, 0.6); margin: 0;"
                                    $"""Notifications are {if notifications then "on" else "off"}, volume is {int volume}, rated {stars} stars."""
                                }
                            }
                        )
                    }
                }

                // ----------------------------------------------------------------
                metroPivotItem {
                    header' "tiles"

                    div {
                        style "display: flex; flex-direction: column; gap: 16px; padding-top: 16px;"

                        metroTileGrid {
                            columns 4

                            metroIconicTile {
                                sizeMedium
                                icon "mail"
                                title' "Mail"
                                count unread
                            }

                            metroFlipTile {
                                sizeMedium
                                div { slot' "front"; style "padding: 12px;"; "Click to flip" }
                                div { slot' "back"; style "padding: 12px;"; "Hello from the back!" }
                            }

                            metroLiveTile {
                                MetroRef.bind liveTile
                                sizeWide
                                interval 3000
                            }

                            metroIconicTile {
                                sizeMedium
                                icon "calendar"
                                title' "Calendar"
                            }

                            metroIconicTile {
                                sizeSmall
                                icon "settings"
                            }

                            metroIconicTile {
                                sizeSmall
                                icon "music"
                            }
                        }

                        div {
                            style "display: flex; gap: 10px;"
                            metroButton {
                                accent
                                onClick (fun _ -> setUnread (unread + 1))
                                "New mail"
                            }
                            metroButton {
                                disabled' (unread = 0)
                                onClick (fun _ -> setUnread 0)
                                "Mark all read"
                            }
                        }
                    }
                }

                // ----------------------------------------------------------------
                metroPivotItem {
                    header' "feedback"

                    div {
                        style "display: flex; flex-wrap: wrap; gap: 40px; padding-top: 16px;"

                        section "Dialog & toasts" (
                            template {
                                div {
                                    style "display: flex; gap: 10px; flex-wrap: wrap;"
                                    metroButton {
                                        accent
                                        onClick (fun _ -> MetroRef.show dialog)
                                        "Open dialog"
                                    }
                                    metroButton {
                                        onClick (fun _ ->
                                            toasts.Show("Toasts are managed globally by ToastHost.", title = "Hello", severity = ToastSeverity.Informational)
                                            |> ignore
                                        )
                                        "Show toast"
                                    }
                                }
                                p { $"Saved {saveCount} times" }
                            }
                        )

                        section "Progress" (
                            template {
                                div {
                                    "Determinate (follows the volume slider)"
                                    metroProgressBar { value' volume; maximum 100.0 }
                                }
                                div {
                                    "Indeterminate"
                                    metroProgressBar { indeterminate true }
                                }
                                metroProgressRing { sizeLarge }
                            }
                        )

                        section "Info bars" (
                            template {
                                metroInfoBar { severityInformational; title' "Info"; "An informational message." }
                                metroInfoBar { severityWarning; title' "Warning"; closable true; "You can close this one." }
                                metroInfoBar { severityError; title' "Error"; "Something went wrong." }
                            }
                        )
                    }
                }
            }

            metroContentDialog {
                MetroRef.bind dialog
                title' "Save changes?"
                "Your settings will be applied."

                metroButton {
                    slot' "buttons"
                    accent
                    onClick (fun _ ->
                        setSaveCount (saveCount + 1)
                        toasts.Show("Your settings were saved.", title = "Saved", severity = ToastSeverity.Success) |> ignore
                        MetroRef.hide dialog
                    )
                    "Save"
                }
                metroButton {
                    slot' "buttons"
                    onClick (fun _ -> MetroRef.hide dialog)
                    "Cancel"
                }
            }
        }
    }
