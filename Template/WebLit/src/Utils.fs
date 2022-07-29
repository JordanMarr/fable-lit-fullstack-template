module WebLit.Utils

open Lit
open Fable.Core
open Fable.Core.JsInterop
open Browser.Types

[<HookComponent>]
let ValidationSummary(validation: Shared.Validation.ValidationResult) =
    let renderError (error: Shared.Validation.Error) = html $"<li>{error.ErrorMessage}</li>"
    let errors = validation.GetErrors()
    html $"""
        <ul class="{if errors.Length > 0 then "validation-summary" else "d-none"}">
            {errors |> List.map renderError}
        </ul>
    """

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

/// Returns the "d-none" class to hide element if condition is true.
let hideIf (condition: bool) = 
    if condition then "hide" else ""

[<ImportMember("lit/directives/style-map.js")>]
let styleMap: obj -> TemplateResult = jsNative

[<LitElement("horiz-stack")>]
let HorizontalStack() = 
    let x, props =
        LitElement.init(fun init ->
            init.useShadowDom <- true
            init.props <- 
                {| 
                    columns = Prop.Of(defaultValue = 2, attribute = "columns")
                    gap = Prop.Of(defaultValue = "0px", attribute = "gap")
                    width = Prop.Of(defaultValue = "100%", attribute = "width")
                |}

            init.styles <- [
                css $"""
                .grid {{
                    display: grid;
                    grid-template-columns: repeat(var(--column-count, 2), 1fr);
                    grid-gap: var(--column-gap, 0px);
                    width: var(--grid-width, 100%%);
                }}
           
                ::slotted(div) {{
                    box-sizing: border-box;
                }}
                """
            ]
        )

    let styles =
        {| ``--column-count`` = props.columns.Value
           ``--column-gap`` = props.gap.Value 
           ``--grid-width`` = props.width.Value |}
    
    html $"""
        <div class="grid" style="{styleMap styles}">
            <slot></slot>
        </div>
    """

[<LitElement("vert-stack")>]
let VerticalStack() = 
    let _, props =
        LitElement.init(fun init ->
            init.useShadowDom <- true
            init.props <- 
                {| 
                    rows = Prop.Of(defaultValue = 2, attribute = "rows")
                    gap = Prop.Of(defaultValue = "0px", attribute = "gap")
                    height = Prop.Of(defaultValue = "100%", attribute = "height")
                |}

            init.styles <- [
                css $"""
                .grid {{
                    display: grid;
                    grid-template-rows: repeat(var(--row-count, 2), 1fr);
                    grid-gap: var(--row-gap, 0px);
                    height: var(--grid-height, 100%%);
                }}
           
                ::slotted(div) {{
                    box-sizing: border-box;
                }}
                """
            ]
        )

    let styles =
        {| ``--row-count`` = props.rows.Value
           ``--row-gap`` = props.gap.Value 
           ``--grid-height`` = props.height.Value |}

    html $"""
        <div class="grid" style="{styleMap styles}">
            <slot></slot>
        </div>
    """


/// Grapnel Router bindings.
[<AutoOpen>]
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

    let Router : RouterStatic = importAll "../node_modules/grapnel/dist/grapnel.min"

    let private router = lazy (Router.Create())

    let navigate (path: string) = 
        router.Value.navigate(path)

    let initRouter () = 
        printfn "Initializing Grapnel Router"
        router.Value
