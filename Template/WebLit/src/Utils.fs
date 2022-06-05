module WebLit.Utils
open Fable.Core
open Fable.Core.JsInterop
open Fable.Core.DynamicExtensions
open Browser.Types
open Lit

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

let private registerShoelace() = 
    //importSideEffects "@shoelace-style/shoelace/dist/themes/light.css"
    [| importDynamic "@shoelace-style/shoelace/dist/themes/dark.css"
       importDynamic "@shoelace-style/shoelace/dist/components/button/button.js"
       importDynamic "@shoelace-style/shoelace/dist/components/card/card.js"
       importDynamic "@shoelace-style/shoelace/dist/components/breadcrumb/breadcrumb.js"
       importDynamic "@shoelace-style/shoelace/dist/components/breadcrumb-item/breadcrumb-item.js" |]
    |> allSettled
    |> Promise.start

/// Imports and registers components
let registerComponents () = 
    registerFluentUI()
    registerShoelace()

[<LitElement("bs-icon")>]
let BootstrapIcon() = 
    let _, props =
        LitElement.init(fun init ->
            init.useShadowDom <- false
            init.props <- 
                {| 
                    src = Prop.Of(defaultValue = "SurveyQuestions", attribute = "src")
                    size = Prop.Of(defaultValue = "20px", attribute = "size")
                    color = Prop.Of(defaultValue = "#036ac4", attribute = "color")
                |}
        )

    html $"""
        <i class="bi bi-{props.src.Value}" style="font-size: {props.size.Value}; color: {props.color.Value};"></i>
    """

/// Grapnel Router bindings.
module Grapnel =
    type Req<'Args> = { ``params``: 'Args }
    type RouteHandler<'Args> = Req<'Args> -> unit
    type RouteHandlerWithEvent<'Args> = Req<'Args> -> Event -> unit

    type Router =
        /// Subscribes to a route.
        [<Emit("$0.get($1, function (req, e) { e.stopPropagation(); $2(req); })")>]
        abstract get<'Args> : path: string * RouteHandler<'Args> -> unit
        
        /// Subscribes to a route with an Event. 
        /// NOTE: You must manually call e.stopPropagation or the handler will execute twice!
        abstract get<'Args> : path: string * RouteHandlerWithEvent<'Args> -> unit
        
        /// Navigates to a route.
        abstract navigate : path: string -> unit

    type RouterStatic =
        [<Emit("new Grapnel({ pushState: true })")>]
        abstract Create: unit -> Router

    [<ImportMember("grapnel")>]
    let Router : RouterStatic = jsNative

    let private router = lazy (Router.Create())

    let navigate (path: string) = 
        router.Value.navigate(path)

    let init () = 
        printfn "Initializing Grapnel Router"
        importAll "../node_modules/grapnel/dist/grapnel.min" |> ignore
