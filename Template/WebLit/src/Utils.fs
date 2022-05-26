module WebLit.Utils
open Fable.Core
open Fable.Core.JsInterop
open Browser.Types

let registerComponents() =
    // Import FluentUI
    let allComponents: obj = importMember "@fluentui/web-components"
    let provideFluentDesignSystem: unit -> obj = importMember "@fluentui/web-components"
    provideFluentDesignSystem()?register(allComponents)

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
        [<Emit("new Grapnel({ pushState: true });")>]
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
