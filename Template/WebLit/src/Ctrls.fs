module WebLit.Ctrls

open Lit
open Fable.Core
open Fable.Core.JsInterop
open Browser.Types

let register () = ()

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

