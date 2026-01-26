namespace Fable.Lit.Dsl.Shoelace

open Fable.Core
open Fable.Core.JsInterop

/// Simple, explicit, string-based Shoelace registration API.
[<AutoOpen>]
module ShoelaceRegistration =

    [<AutoOpen>]
    module private PromiseExtensions =
        type PromiseResolution<'ResolutionValue> =
            {| status: string; value: 'ResolutionValue option; reason: exn option |}
        [<Emit("Promise.allSettled($0)")>]
        let inline allSettled<'T> (promises: JS.Promise<'T> array): JS.Promise<PromiseResolution<'T>> = jsNative

    module Shoelace =
        /// String bindings for Shoelace assets (themes and components).
        /// Use these with `Shoelace.register` to import only what you need.
        module Asset =
            // Themes
            
            let [<Literal>] DarkTheme = "@shoelace-style/shoelace/dist/themes/dark.css"
            let [<Literal>] LightTheme = "@shoelace-style/shoelace/dist/themes/light.css"

            // Components            
            let [<Literal>] Alert = "@shoelace-style/shoelace/dist/components/alert/alert.js"
            let [<Literal>] AnimatedImage = "@shoelace-style/shoelace/dist/components/animated-image/animated-image.js"
            let [<Literal>] Animation = "@shoelace-style/shoelace/dist/components/animation/animation.js"
            let [<Literal>] Avatar = "@shoelace-style/shoelace/dist/components/avatar/avatar.js"
            let [<Literal>] Badge = "@shoelace-style/shoelace/dist/components/badge/badge.js"
            let [<Literal>] Breadcrumb = "@shoelace-style/shoelace/dist/components/breadcrumb/breadcrumb.js"
            let [<Literal>] BreadcrumbItem = "@shoelace-style/shoelace/dist/components/breadcrumb-item/breadcrumb-item.js"
            let [<Literal>] Button = "@shoelace-style/shoelace/dist/components/button/button.js"
            let [<Literal>] ButtonGroup = "@shoelace-style/shoelace/dist/components/button-group/button-group.js"
            let [<Literal>] Card = "@shoelace-style/shoelace/dist/components/card/card.js"
            let [<Literal>] Carousel = "@shoelace-style/shoelace/dist/components/carousel/carousel.js"
            let [<Literal>] CarouselItem = "@shoelace-style/shoelace/dist/components/carousel-item/carousel-item.js"
            let [<Literal>] Checkbox = "@shoelace-style/shoelace/dist/components/checkbox/checkbox.js"
            let [<Literal>] ColorPicker = "@shoelace-style/shoelace/dist/components/color-picker/color-picker.js"
            let [<Literal>] CopyButton = "@shoelace-style/shoelace/dist/components/copy-button/copy-button.js"
            let [<Literal>] Details = "@shoelace-style/shoelace/dist/components/details/details.js"
            let [<Literal>] Dialog = "@shoelace-style/shoelace/dist/components/dialog/dialog.js"
            let [<Literal>] Divider = "@shoelace-style/shoelace/dist/components/divider/divider.js"
            let [<Literal>] Drawer = "@shoelace-style/shoelace/dist/components/drawer/drawer.js"
            let [<Literal>] Dropdown = "@shoelace-style/shoelace/dist/components/dropdown/dropdown.js"
            let [<Literal>] FormatBytes = "@shoelace-style/shoelace/dist/components/format-bytes/format-bytes.js"
            let [<Literal>] FormatDate = "@shoelace-style/shoelace/dist/components/format-date/format-date.js"
            let [<Literal>] FormatNumber = "@shoelace-style/shoelace/dist/components/format-number/format-number.js"
            let [<Literal>] Icon = "@shoelace-style/shoelace/dist/components/icon/icon.js"
            let [<Literal>] IconButton = "@shoelace-style/shoelace/dist/components/icon-button/icon-button.js"
            let [<Literal>] ImageComparer = "@shoelace-style/shoelace/dist/components/image-comparer/image-comparer.js"
            let [<Literal>] Include = "@shoelace-style/shoelace/dist/components/include/include.js"
            let [<Literal>] Input = "@shoelace-style/shoelace/dist/components/input/input.js"
            let [<Literal>] Menu = "@shoelace-style/shoelace/dist/components/menu/menu.js"
            let [<Literal>] MenuItem = "@shoelace-style/shoelace/dist/components/menu-item/menu-item.js"
            let [<Literal>] MenuLabel = "@shoelace-style/shoelace/dist/components/menu-label/menu-label.js"
            let [<Literal>] MutationObserver = "@shoelace-style/shoelace/dist/components/mutation-observer/mutation-observer.js"
            let [<Literal>] Option = "@shoelace-style/shoelace/dist/components/option/option.js"
            let [<Literal>] Popup = "@shoelace-style/shoelace/dist/components/popup/popup.js"
            let [<Literal>] ProgressBar = "@shoelace-style/shoelace/dist/components/progress-bar/progress-bar.js"
            let [<Literal>] ProgressRing = "@shoelace-style/shoelace/dist/components/progress-ring/progress-ring.js"
            let [<Literal>] QrCode = "@shoelace-style/shoelace/dist/components/qr-code/qr-code.js"
            let [<Literal>] Radio = "@shoelace-style/shoelace/dist/components/radio/radio.js"
            let [<Literal>] RadioButton = "@shoelace-style/shoelace/dist/components/radio-button/radio-button.js"
            let [<Literal>] RadioGroup = "@shoelace-style/shoelace/dist/components/radio-group/radio-group.js"
            let [<Literal>] Range = "@shoelace-style/shoelace/dist/components/range/range.js"
            let [<Literal>] Rating = "@shoelace-style/shoelace/dist/components/rating/rating.js"
            let [<Literal>] RelativeTime = "@shoelace-style/shoelace/dist/components/relative-time/relative-time.js"
            let [<Literal>] ResizeObserver = "@shoelace-style/shoelace/dist/components/resize-observer/resize-observer.js"
            let [<Literal>] Select = "@shoelace-style/shoelace/dist/components/select/select.js"
            let [<Literal>] Skeleton = "@shoelace-style/shoelace/dist/components/skeleton/skeleton.js"
            let [<Literal>] Spinner = "@shoelace-style/shoelace/dist/components/spinner/spinner.js"
            let [<Literal>] SplitPanel = "@shoelace-style/shoelace/dist/components/split-panel/split-panel.js"
            let [<Literal>] Switch = "@shoelace-style/shoelace/dist/components/switch/switch.js"
            let [<Literal>] Tab = "@shoelace-style/shoelace/dist/components/tab/tab.js"
            let [<Literal>] TabGroup = "@shoelace-style/shoelace/dist/components/tab-group/tab-group.js"
            let [<Literal>] TabPanel = "@shoelace-style/shoelace/dist/components/tab-panel/tab-panel.js"
            let [<Literal>] Tag = "@shoelace-style/shoelace/dist/components/tag/tag.js"
            let [<Literal>] Textarea = "@shoelace-style/shoelace/dist/components/textarea/textarea.js"
            let [<Literal>] Tooltip = "@shoelace-style/shoelace/dist/components/tooltip/tooltip.js"
            let [<Literal>] Tree = "@shoelace-style/shoelace/dist/components/tree/tree.js"
            let [<Literal>] TreeItem = "@shoelace-style/shoelace/dist/components/tree-item/tree-item.js"
            let [<Literal>] VisuallyHidden = "@shoelace-style/shoelace/dist/components/visually-hidden/visually-hidden.js"

            /// Convenience groups for common registration patterns.
            module All =
                let core = [
                    Button; Card; Input; Alert; Dropdown; Menu; MenuItem; MenuLabel
                    Checkbox; Radio; RadioButton; RadioGroup; Select; Option; Switch; Textarea; Range
                ]

                let overlays = [
                    Dialog; Drawer; Tooltip; Details; Popup
                ]

                let navigation = [
                    Breadcrumb; BreadcrumbItem; ButtonGroup; Tab; TabGroup; TabPanel
                ]

                let feedback = [
                    Alert; Badge; Tag; Spinner; ProgressBar; ProgressRing; Skeleton
                ]

                let media = [
                    Avatar; Icon; IconButton; AnimatedImage; Carousel; CarouselItem; ImageComparer
                ]

                let utilities = [
                    Animation; CopyButton; Divider; FormatBytes; FormatDate; FormatNumber
                    Include; MutationObserver; QrCode; Rating; RelativeTime; ResizeObserver
                    SplitPanel; Tree; TreeItem; VisuallyHidden; ColorPicker
                ]

                let all =
                    core @ overlays @ navigation @ feedback @ media @ utilities
                    |> List.distinct

        /// Sets the base path for Shoelace assets (icons, etc.).
        /// Call this before importing components.
        let setBasePath () : unit =
            let setBasePath': string -> unit =
                import "setBasePath" "@shoelace-style/shoelace/dist/utilities/base-path.js"
            setBasePath' "https://cdn.jsdelivr.net/npm/@shoelace-style/shoelace@2.15.0/cdn/"

        /// Starts importing the provided array of dynamic import promises.
        /// Use with an array of `importDynamic Asset.X` calls.
        let startImports (imports: JS.Promise<obj> array) : unit =
            imports
            |> allSettled
            |> Promise.start
