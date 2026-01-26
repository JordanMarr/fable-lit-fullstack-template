namespace Fable.Lit.Dsl

open Lit

// =============================================================================
// Computation Expression Builders
// =============================================================================

/// Builder for creating HTML element nodes.
type ElementBuilder(tag: string) =

    member _.Yield(text: string) : Node =
        Text text

    member _.Yield(node: Node) : Node =
        node

    member _.Yield(attr: Attr) : Node =
        AttrNode attr

    member _.Yield(template: TemplateResult) : Node =
        Template template

    member _.Yield(()) : Node =
        Nothing

    member _.YieldFrom(nodes: Node seq) : Node =
        nodes |> Seq.toList |> Fragment

    member _.Combine(a: Node, b: Node) : Node =
        match a, b with
        | Nothing, x -> x
        | x, Nothing -> x
        | Fragment xs, Fragment ys -> Fragment (xs @ ys)
        | Fragment xs, n -> Fragment (xs @ [ n ])
        | n, Fragment ys -> Fragment (n :: ys)
        | x, y -> Fragment [ x; y ]

    member _.Delay(f: unit -> Node) : Node =
        f()

    member _.Zero() : Node =
        Nothing

    member _.For(xs: 'T seq, f: 'T -> Node) : Node =
        xs |> Seq.map f |> Seq.toList |> Fragment

    member _.Run(content: Node) : Node =
        // Flatten the content and separate attributes from children
        let rec flatten node =
            match node with
            | Fragment nodes -> nodes |> List.collect flatten
            | Nothing -> []
            | other -> [ other ]

        let flatContent = flatten content

        let attrs, children =
            flatContent
            |> List.fold (fun (attrs, children) node ->
                match node with
                | AttrNode attr -> (attr :: attrs, children)
                | Nothing -> (attrs, children)
                | other -> (attrs, other :: children)
            ) ([], [])

        Element(tag, List.rev attrs, List.rev children)

/// Builder for creating HTML fragments (no wrapper element).
/// Returns a Node for use in nested contexts.
type TemplateBuilder() =

    member _.Yield(text: string) : Node =
        Text text

    member _.Yield(node: Node) : Node =
        node

    member _.Yield(attr: Attr) : Node =
        AttrNode attr

    member _.Yield(t: TemplateResult) : Node =
        Template t

    member _.Yield(()) : Node =
        Nothing

    member _.YieldFrom(nodes: Node seq) : Node =
        nodes |> Seq.toList |> Fragment

    member _.Combine(a: Node, b: Node) : Node =
        match a, b with
        | Nothing, x -> x
        | x, Nothing -> x
        | Fragment xs, Fragment ys -> Fragment (xs @ ys)
        | Fragment xs, n -> Fragment (xs @ [ n ])
        | n, Fragment ys -> Fragment (n :: ys)
        | x, y -> Fragment [ x; y ]

    member _.Delay(f: unit -> Node) : Node =
        f()

    member _.Zero() : Node =
        Nothing

    member _.For(xs: 'T seq, f: 'T -> Node) : Node =
        xs |> Seq.map f |> Seq.toList |> Fragment

    member _.Run(content: Node) : Node =
        // Return the content as-is (possibly simplified)
        match content with
        | Nothing -> Nothing
        | Fragment [ single ] -> single
        | other -> other

/// Builder for creating views that auto-render to TemplateResult.
/// Use this at the top level of components.
type ViewBuilder() =
    inherit TemplateBuilder()

    member _.Run(content: Node) : TemplateResult =
        let node =
            match content with
            | Nothing -> Nothing
            | Fragment [ single ] -> single
            | other -> other
        Renderer.render node

// =============================================================================
// DSL Entry Points
// =============================================================================

[<AutoOpen>]
module HtmlDsl =
    /// Creates an HTML fragment or node (returns Node).
    /// Use this for nested fragments inside elements.
    let template = TemplateBuilder()

    /// Creates a view that auto-renders to TemplateResult.
    /// Use this at the top level of components.
    let view = ViewBuilder()

    /// Creates a custom element with the specified tag name.
    let el (tag: string) = ElementBuilder(tag)

    /// Represents an empty node that renders nothing.
    let nothing = Node.Nothing

// =============================================================================
// Standard HTML Elements
// =============================================================================

[<AutoOpen>]
module Elements =
    // Document metadata
    let head = ElementBuilder("head")
    let title = ElementBuilder("title")
    let meta = ElementBuilder("meta")
    let link = ElementBuilder("link")
    let style = ElementBuilder("style")
    let script = ElementBuilder("script")

    // Sectioning
    let body = ElementBuilder("body")
    let article = ElementBuilder("article")
    let section = ElementBuilder("section")
    let nav = ElementBuilder("nav")
    let aside = ElementBuilder("aside")
    let header = ElementBuilder("header")
    let footer = ElementBuilder("footer")
    let main = ElementBuilder("main")

    // Content grouping
    let div = ElementBuilder("div")
    let p = ElementBuilder("p")
    let hr = ElementBuilder("hr")
    let pre = ElementBuilder("pre")
    let blockquote = ElementBuilder("blockquote")
    let ol = ElementBuilder("ol")
    let ul = ElementBuilder("ul")
    let li = ElementBuilder("li")
    let dl = ElementBuilder("dl")
    let dt = ElementBuilder("dt")
    let dd = ElementBuilder("dd")
    let figure = ElementBuilder("figure")
    let figcaption = ElementBuilder("figcaption")

    // Text-level semantics
    let span = ElementBuilder("span")
    let a = ElementBuilder("a")
    let em = ElementBuilder("em")
    let strong = ElementBuilder("strong")
    let small = ElementBuilder("small")
    let s = ElementBuilder("s")
    let cite = ElementBuilder("cite")
    let q = ElementBuilder("q")
    let dfn = ElementBuilder("dfn")
    let abbr = ElementBuilder("abbr")
    let code = ElementBuilder("code")
    let var = ElementBuilder("var")
    let samp = ElementBuilder("samp")
    let kbd = ElementBuilder("kbd")
    let sub = ElementBuilder("sub")
    let sup = ElementBuilder("sup")
    let i = ElementBuilder("i")
    let b = ElementBuilder("b")
    let u = ElementBuilder("u")
    let mark = ElementBuilder("mark")
    let br = ElementBuilder("br")
    let wbr = ElementBuilder("wbr")

    // Headings
    let h1 = ElementBuilder("h1")
    let h2 = ElementBuilder("h2")
    let h3 = ElementBuilder("h3")
    let h4 = ElementBuilder("h4")
    let h5 = ElementBuilder("h5")
    let h6 = ElementBuilder("h6")

    // Embedded content
    let img = ElementBuilder("img")
    let iframe = ElementBuilder("iframe")
    let embed = ElementBuilder("embed")
    let object = ElementBuilder("object")
    let video = ElementBuilder("video")
    let audio = ElementBuilder("audio")
    let source = ElementBuilder("source")
    let track = ElementBuilder("track")
    let canvas = ElementBuilder("canvas")
    let svg = ElementBuilder("svg")

    // Tables
    let table = ElementBuilder("table")
    let caption = ElementBuilder("caption")
    let colgroup = ElementBuilder("colgroup")
    let col = ElementBuilder("col")
    let thead = ElementBuilder("thead")
    let tbody = ElementBuilder("tbody")
    let tfoot = ElementBuilder("tfoot")
    let tr = ElementBuilder("tr")
    let td = ElementBuilder("td")
    let th = ElementBuilder("th")

    // Forms
    let form = ElementBuilder("form")
    let label = ElementBuilder("label")
    let input = ElementBuilder("input")
    let button = ElementBuilder("button")
    let select = ElementBuilder("select")
    let datalist = ElementBuilder("datalist")
    let optgroup = ElementBuilder("optgroup")
    let option = ElementBuilder("option")
    let textarea = ElementBuilder("textarea")
    let output = ElementBuilder("output")
    let progress = ElementBuilder("progress")
    let meter = ElementBuilder("meter")
    let fieldset = ElementBuilder("fieldset")
    let legend = ElementBuilder("legend")

    // Interactive
    let details = ElementBuilder("details")
    let summary = ElementBuilder("summary")
    let dialog = ElementBuilder("dialog")
    let menu = ElementBuilder("menu")

    // Template
    let template = ElementBuilder("template")
    let slot = ElementBuilder("slot")

// =============================================================================
// Attributes
// =============================================================================

[<AutoOpen>]
module Attrs =
    /// Creates a generic attribute.
    let attr (name: string) (value: obj) : Attr =
        Attr(name, value)

    /// Creates a boolean attribute (present when true, absent when false).
    let boolAttr (name: string) (enabled: bool) : Attr =
        BoolAttr(name, enabled)

    /// Creates a property binding (Lit's .property=${value} syntax).
    let prop (name: string) (value: obj) : Attr =
        Attr.Prop(name, value)

    /// Creates an event handler attribute (Lit's @event=${handler} syntax).
    let on (eventName: string) (handler: Browser.Types.Event -> unit) : Attr =
        Event(eventName, handler)

    // Common attributes
    let id (value: string) : Attr = Attr("id", value)
    let class' (value: string) : Attr = Attr("class", value)
    let classList (classes: (string * bool) list) : Attr =
        let value =
            classes
            |> List.filter snd
            |> List.map fst
            |> String.concat " "
        Attr("class", value)
    let style (value: string) : Attr = Attr("style", value)
    let href (value: string) : Attr = Attr("href", value)
    let src (value: string) : Attr = Attr("src", value)
    let alt (value: string) : Attr = Attr("alt", value)
    let title (value: string) : Attr = Attr("title", value)
    let name (value: string) : Attr = Attr("name", value)
    let value (value: obj) : Attr = Attr("value", value)
    let type' (value: string) : Attr = Attr("type", value)
    let placeholder (value: string) : Attr = Attr("placeholder", value)
    let disabled (value: bool) : Attr = BoolAttr("disabled", value)
    let readonly (value: bool) : Attr = BoolAttr("readonly", value)
    let checked' (value: bool) : Attr = BoolAttr("checked", value)
    let selected (value: bool) : Attr = BoolAttr("selected", value)
    let required (value: bool) : Attr = BoolAttr("required", value)
    let hidden (value: bool) : Attr = BoolAttr("hidden", value)
    let autofocus (value: bool) : Attr = BoolAttr("autofocus", value)
    let multiple (value: bool) : Attr = BoolAttr("multiple", value)

    // Form attributes
    let action (value: string) : Attr = Attr("action", value)
    let method (value: string) : Attr = Attr("method", value)
    let enctype (value: string) : Attr = Attr("enctype", value)
    let target (value: string) : Attr = Attr("target", value)
    let for' (value: string) : Attr = Attr("for", value)
    let min (value: obj) : Attr = Attr("min", value)
    let max (value: obj) : Attr = Attr("max", value)
    let step (value: obj) : Attr = Attr("step", value)
    let pattern (value: string) : Attr = Attr("pattern", value)
    let maxlength (value: int) : Attr = Attr("maxlength", value)
    let minlength (value: int) : Attr = Attr("minlength", value)
    let rows (value: int) : Attr = Attr("rows", value)
    let cols (value: int) : Attr = Attr("cols", value)

    // Table attributes
    let colspan (value: int) : Attr = Attr("colspan", value)
    let rowspan (value: int) : Attr = Attr("rowspan", value)

    // ARIA attributes
    let role (value: string) : Attr = Attr("role", value)
    let ariaLabel (value: string) : Attr = Attr("aria-label", value)
    let ariaDescribedby (value: string) : Attr = Attr("aria-describedby", value)
    let ariaHidden (value: bool) : Attr = Attr("aria-hidden", if value then "true" else "false")
    let ariaExpanded (value: bool) : Attr = Attr("aria-expanded", if value then "true" else "false")
    let ariaSelected (value: bool) : Attr = Attr("aria-selected", if value then "true" else "false")
    let ariaControls (value: string) : Attr = Attr("aria-controls", value)

    // Data attributes
    let data (name: string) (value: string) : Attr = Attr($"data-{name}", value)

    // Media attributes
    let width (value: obj) : Attr = Attr("width", value)
    let height (value: obj) : Attr = Attr("height", value)
    let autoplay (value: bool) : Attr = BoolAttr("autoplay", value)
    let controls (value: bool) : Attr = BoolAttr("controls", value)
    let loop (value: bool) : Attr = BoolAttr("loop", value)
    let muted (value: bool) : Attr = BoolAttr("muted", value)

    // Common event handlers
    let onClick (handler: Browser.Types.MouseEvent -> unit) : Attr =
        Event("click", handler)
    let onInput (handler: Browser.Types.Event -> unit) : Attr =
        Event("input", handler)
    let onChange (handler: Browser.Types.Event -> unit) : Attr =
        Event("change", handler)
    let onSubmit (handler: Browser.Types.Event -> unit) : Attr =
        Event("submit", handler)
    let onKeyDown (handler: Browser.Types.KeyboardEvent -> unit) : Attr =
        Event("keydown", handler)
    let onKeyUp (handler: Browser.Types.KeyboardEvent -> unit) : Attr =
        Event("keyup", handler)
    let onFocus (handler: Browser.Types.FocusEvent -> unit) : Attr =
        Event("focus", handler)
    let onBlur (handler: Browser.Types.FocusEvent -> unit) : Attr =
        Event("blur", handler)
    let onMouseEnter (handler: Browser.Types.MouseEvent -> unit) : Attr =
        Event("mouseenter", handler)
    let onMouseLeave (handler: Browser.Types.MouseEvent -> unit) : Attr =
        Event("mouseleave", handler)

    /// Creates a ref binding that captures the DOM element after rendering.
    /// The callback receives the element when attached (or undefined when detached).
    let bindRef (setter: obj -> unit) : Attr =
        Ref setter

// =============================================================================
// Interop Helpers
// =============================================================================

[<AutoOpen>]
module LitInterop =
    /// Embeds an existing Lit TemplateResult inside the DSL.
    let lit (t: TemplateResult) : Node =
        Template t

    /// Embeds raw HTML (rendered via Lit's unsafeHTML).
    let rawHtml (s: string) : Node =
        RawHtml s

    /// Creates a text node.
    let text (s: string) : Node =
        Text s

    /// Creates a fragment from a list of nodes.
    let fragment (nodes: Node list) : Node =
        Fragment nodes
