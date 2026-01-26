module WebLit.Registrations

open Fable.Core
open Fable.Core.JsInterop
open Fable.Core.DynamicExtensions
open Fable.Lit.Dsl.Shoelace

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

/// Imports and registers components
let registerComponents () =
    registerFluentUI()
    Shoelace.setBasePath()
    [|
        importDynamic Shoelace.Asset.DarkTheme
        importDynamic Shoelace.Asset.Alert
        importDynamic Shoelace.Asset.Animation
        importDynamic Shoelace.Asset.Badge
        importDynamic Shoelace.Asset.Breadcrumb
        importDynamic Shoelace.Asset.BreadcrumbItem
        importDynamic Shoelace.Asset.Button
        importDynamic Shoelace.Asset.Card
        importDynamic Shoelace.Asset.Details
        importDynamic Shoelace.Asset.Dialog
        importDynamic Shoelace.Asset.Drawer
        importDynamic Shoelace.Asset.Icon
        importDynamic Shoelace.Asset.Input
        importDynamic Shoelace.Asset.Switch
        importDynamic Shoelace.Asset.Tag
        importDynamic Shoelace.Asset.Tooltip
        importDynamic Shoelace.Asset.Dropdown
    |]
    |> Shoelace.startImports
