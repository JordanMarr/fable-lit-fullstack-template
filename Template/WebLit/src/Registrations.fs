module WebLit.Registrations

open Fable.Core
open Fable.Core.JsInterop
open Fable.Core.DynamicExtensions
open Fable.Lit.Dsl.Shoelace
open Fable.Lit.Dsl.Metrino

[<AutoOpen>]
module PromiseExtensions =
    type PromiseResolution<'ResolutionValue> =
        {| status: string; value: 'ResolutionValue option; reason: exn option |}
    [<Emit("Promise.allSettled($0)")>]
    let inline allSettled<'T> (promises: JS.Promise<'T> array): JS.Promise<PromiseResolution<'T>> = jsNative

let registerFluentUI() =
    promise {
        let! fluentModule = importDynamic "@fluentui/web-components"
        let provideFluentDesignSystem: unit -> obj = unbox (fluentModule.Item "provideFluentDesignSystem")

        // Register all components
        //let allComponents: obj = fluentModule.Item "allComponents"
        //provideFluentDesignSystem()?register(allComponents)

        // Cherrypick components
        let fluentSlider: unit -> unit = unbox (fluentModule.Item "fluentSlider")
        let fluentSliderLabel: unit -> unit = unbox (fluentModule.Item "fluentSliderLabel")

        provideFluentDesignSystem()?register(
            fluentSlider(),
            fluentSliderLabel()
        )

    }
    |> Promise.start

/// Registers the Metrino components used by the Metrino DSL demo page.
/// (Use `Metrino.registerAll()` to register every component, or a category like `Metrino.registerInputs()`.)
let registerMetrino() =
    Metrino.Register.button()
    Metrino.Register.comboBox()
    Metrino.Register.textBox()
    Metrino.Register.toggleSwitch()
    Metrino.Register.checkBox()
    Metrino.Register.slider()
    Metrino.Register.rating()
    Metrino.Register.pivot()
    Metrino.Register.tileGrid()
    Metrino.Register.iconicTile()
    Metrino.Register.flipTile()
    Metrino.Register.liveTile()
    Metrino.Register.contentDialog()
    Metrino.Register.infoBar()
    Metrino.Register.progressBar()
    Metrino.Register.progressRing()

/// Imports and registers components
let registerComponents () =
    registerFluentUI()
    registerMetrino()
    Shoelace.setBasePath()
    [|
        importDynamic Shoelace.Asset.DarkTheme
        importDynamic Shoelace.Asset.Alert
        importDynamic Shoelace.Asset.Animation
        importDynamic Shoelace.Asset.Badge
        importDynamic Shoelace.Asset.Breadcrumb
        importDynamic Shoelace.Asset.BreadcrumbItem
        importDynamic Shoelace.Asset.Button
        importDynamic Shoelace.Asset.ButtonGroup
        importDynamic Shoelace.Asset.Card
        importDynamic Shoelace.Asset.Details
        importDynamic Shoelace.Asset.Dialog
        importDynamic Shoelace.Asset.Drawer
        importDynamic Shoelace.Asset.Dropdown
        importDynamic Shoelace.Asset.Icon
        importDynamic Shoelace.Asset.Input
        importDynamic Shoelace.Asset.Menu
        importDynamic Shoelace.Asset.MenuItem
        importDynamic Shoelace.Asset.Switch
        importDynamic Shoelace.Asset.Tag
        importDynamic Shoelace.Asset.Tooltip
    |]
    |> Shoelace.startImports
