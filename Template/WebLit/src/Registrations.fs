module WebLit.Registrations

open Fable.Core
open Fable.Core.JsInterop
open Fable.Core.DynamicExtensions

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
        let fluentDataGridRow: unit -> unit = unbox (fluentModule.Item "fluentDataGridRow")
        let fluentDataGridCell: unit -> unit = unbox (fluentModule.Item "fluentDataGridCell")
        let fluentDataGrid: unit -> unit = unbox (fluentModule.Item "fluentDataGrid")

        provideFluentDesignSystem()?register(
            fluentSlider(), 
            fluentSliderLabel(),
            fluentDataGridRow(),
            fluentDataGridCell(),
            fluentDataGrid()
        )

    }
    |> Promise.start

let private registerShoelace() = 
    [| 
        importDynamic "@shoelace-style/shoelace/dist/themes/dark.css"
        importDynamic "@shoelace-style/shoelace/dist/components/button/button.js"
        importDynamic "@shoelace-style/shoelace/dist/components/card/card.js"
        importDynamic "@shoelace-style/shoelace/dist/components/breadcrumb/breadcrumb.js"
        importDynamic "@shoelace-style/shoelace/dist/components/breadcrumb-item/breadcrumb-item.js" 
        importDynamic "@shoelace-style/shoelace/dist/components/button-group/button-group.js"
        importDynamic "@shoelace-style/shoelace/dist/components/dropdown/dropdown.js"
        importDynamic "@shoelace-style/shoelace/dist/components/menu/menu.js"
        importDynamic "@shoelace-style/shoelace/dist/components/menu-item/menu-item.js"
        importDynamic "@shoelace-style/shoelace/dist/components/input/input.js"
        importDynamic "@shoelace-style/shoelace/dist/components/alert/alert.js"
    |]
    |> allSettled
    |> Promise.start

/// Imports and registers components
let registerComponents () = 
    registerFluentUI()
    registerShoelace()

