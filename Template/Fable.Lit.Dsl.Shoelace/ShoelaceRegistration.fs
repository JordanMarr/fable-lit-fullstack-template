namespace Fable.Lit.Dsl.Shoelace

open Fable.Core
open Fable.Core.JsInterop

/// Typed Shoelace component registration API.
/// Provides an ergonomic way to register Shoelace components with proper typing.
[<AutoOpen>]
module ShoelaceRegistration =

    [<AutoOpen>]
    module private PromiseExtensions =
        type PromiseResolution<'ResolutionValue> =
            {| status: string; value: 'ResolutionValue option; reason: exn option |}
        [<Emit("Promise.allSettled($0)")>]
        let inline allSettled<'T> (promises: JS.Promise<'T> array): JS.Promise<PromiseResolution<'T>> = jsNative

    module Shoelace =
        /// Shoelace component types available for registration.
        type Component =
            | Button
            | Card
            | Breadcrumb
            | BreadcrumbItem
            | ButtonGroup
            | Dropdown
            | Menu
            | MenuItem
            | Input
            | Alert
            | Dialog
            | Drawer
            | Tooltip
            | Details
            | Animation
            | Switch
            | Icon
            | Badge
            | Tag

        /// Optional assets that can be included.
        type Asset =
            | DarkTheme

        let private componentImportPath = function
            | Button -> "@shoelace-style/shoelace/dist/components/button/button.js"
            | Card -> "@shoelace-style/shoelace/dist/components/card/card.js"
            | Breadcrumb -> "@shoelace-style/shoelace/dist/components/breadcrumb/breadcrumb.js"
            | BreadcrumbItem -> "@shoelace-style/shoelace/dist/components/breadcrumb-item/breadcrumb-item.js"
            | ButtonGroup -> "@shoelace-style/shoelace/dist/components/button-group/button-group.js"
            | Dropdown -> "@shoelace-style/shoelace/dist/components/dropdown/dropdown.js"
            | Menu -> "@shoelace-style/shoelace/dist/components/menu/menu.js"
            | MenuItem -> "@shoelace-style/shoelace/dist/components/menu-item/menu-item.js"
            | Input -> "@shoelace-style/shoelace/dist/components/input/input.js"
            | Alert -> "@shoelace-style/shoelace/dist/components/alert/alert.js"
            | Dialog -> "@shoelace-style/shoelace/dist/components/dialog/dialog.js"
            | Drawer -> "@shoelace-style/shoelace/dist/components/drawer/drawer.js"
            | Tooltip -> "@shoelace-style/shoelace/dist/components/tooltip/tooltip.js"
            | Details -> "@shoelace-style/shoelace/dist/components/details/details.js"
            | Animation -> "@shoelace-style/shoelace/dist/components/animation/animation.js"
            | Switch -> "@shoelace-style/shoelace/dist/components/switch/switch.js"
            | Icon -> "@shoelace-style/shoelace/dist/components/icon/icon.js"
            | Badge -> "@shoelace-style/shoelace/dist/components/badge/badge.js"
            | Tag -> "@shoelace-style/shoelace/dist/components/tag/tag.js"

        let private assetImportPath = function
            | DarkTheme -> "@shoelace-style/shoelace/dist/themes/dark.css"

        /// Predefined component profiles for common use cases.
        module Profile =
            let core = [ Button; Card; Input; Alert; Dropdown; Menu; MenuItem ]
            let navigation = [ Breadcrumb; BreadcrumbItem; ButtonGroup ]
            let overlays = [ Dialog; Drawer; Tooltip; Details ]
            let animations = [ Animation ]
            let extras = [ Switch; Icon; Badge; Tag ]
            let all = core @ navigation @ overlays @ animations @ extras

        /// Registers the specified Shoelace components.
        /// This sets the base path for Shoelace assets and dynamically imports the components.
        let register (components: Component list) : unit =
            // Set the base path for Shoelace assets (icons, etc.)
            let setBasePath: string -> unit =
                import "setBasePath" "@shoelace-style/shoelace/dist/utilities/base-path.js"
            setBasePath "https://cdn.jsdelivr.net/npm/@shoelace-style/shoelace@2.15.0/cdn/"

            components
            |> List.map (componentImportPath >> importDynamic)
            |> List.toArray
            |> allSettled
            |> Promise.start

        /// Registers the specified Shoelace components along with optional assets (e.g., themes).
        /// This sets the base path for Shoelace assets and dynamically imports the components.
        let registerWithAssets (components: Component list) (assets: Asset list) : unit =
            // Set the base path for Shoelace assets (icons, etc.)
            let setBasePath: string -> unit =
                import "setBasePath" "@shoelace-style/shoelace/dist/utilities/base-path.js"
            setBasePath "https://cdn.jsdelivr.net/npm/@shoelace-style/shoelace@2.15.0/cdn/"

            let assetImports = assets |> List.map (assetImportPath >> importDynamic)
            let componentImports = components |> List.map (componentImportPath >> importDynamic)

            (assetImports @ componentImports)
            |> List.toArray
            |> allSettled
            |> Promise.start
