module WebLit.Ctrls

module Toast = 
    open Fable.Core.JsInterop
    open Fable.Core.JS

    let private toaster = lazy Browser.Dom.document.getElementById("toaster")
    let private toast (variant: string) (msg: string) = 
        toaster.Value.innerHTML <- msg
        toaster.Value?variant <- variant
        toaster.Value?toast() |> ignore

    let info (msg: string) = 
        console.info msg
        toast "primary" $"<bs-icon slot='icon' src='info-circle' color='var(--sl-color-primary-600)'></bs-icon> {msg}"
    let warn (msg: string) =
        console.warn msg
        toast "warning" $"<bs-icon slot='icon' src='exclamation-triangle' color='var(--sl-color-warning-600)'></bs-icon> {msg}"
    let error (msg: string) = 
        console.error msg
        toast "danger" $"<bs-icon slot='icon' src='exclamation-octagon' color='var(--sl-color-danger-600)'></bs-icon> {msg}"
    let success (msg: string) =
        console.info msg
        toast "success" $"<bs-icon slot='icon' src='check2-circle' color='var(--sl-color-success-600)'></bs-icon> {msg}"
    let neutral (msg: string) =
        console.info msg
        toast "neutral" $"<bs-icon slot='icon' src='gear' color='var(--sl-color-neutral-600)'></bs-icon> {msg}"

    open Elmish

    module Cmd =
        let private onFail ex = failwith "toast failed"
        let toastInfo (msg: string) = Cmd.OfFunc.attempt info msg onFail
        let toastWarning (msg: string) = Cmd.OfFunc.attempt warn msg onFail
        let toastNeutral (msg: string) = Cmd.OfFunc.attempt error msg onFail
        let toastSuccess (msg: string) = Cmd.OfFunc.attempt success msg onFail
        let toastError (msg: string) = Cmd.OfFunc.attempt error msg onFail


open Lit
open Fable.Core

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
