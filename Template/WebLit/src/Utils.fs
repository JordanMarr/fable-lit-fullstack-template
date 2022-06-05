module WebLit.Utils
open Fable.Core
open Fable.Core.JsInterop
open Browser.Types
open Lit

let registerFluentUI() =
    let provideFluentDesignSystem: unit -> obj = importMember "@fluentui/web-components"
    
    // Register all components
    //let allComponents: obj = importMember "@fluentui/web-components"
    //provideFluentDesignSystem()?register(allComponents)
    
    // Cherrypick components
    let fluentSlider: unit -> unit = importMember "@fluentui/web-components"
    let fluentSliderLabel: unit -> unit = importMember "@fluentui/web-components"
    provideFluentDesignSystem()?register(
        fluentSlider(), 
        fluentSliderLabel()
    )

let private registerShoelace() = 
    //importSideEffects "@shoelace-style/shoelace/dist/themes/light.css"
    importSideEffects "@shoelace-style/shoelace/dist/themes/dark.css"
    importSideEffects "@shoelace-style/shoelace/dist/components/button/button.js"
    importSideEffects "@shoelace-style/shoelace/dist/components/card/card.js"
    importSideEffects "@shoelace-style/shoelace/dist/components/breadcrumb/breadcrumb.js"
    importSideEffects "@shoelace-style/shoelace/dist/components/breadcrumb-item/breadcrumb-item.js"

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

    open Fable.Core.JsInterop

    [<ImportMember("grapnel")>]
    let Router : RouterStatic = jsNative

    let navigate (path: string) = 
        let router = Router.Create()
        router.navigate(path)

    let init () = 
        printfn "Initializing Grapnel Router"
        importAll "../node_modules/grapnel/dist/grapnel.min" |> ignore
