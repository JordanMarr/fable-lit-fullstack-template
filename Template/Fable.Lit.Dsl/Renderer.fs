namespace Fable.Lit.Dsl

open Fable.Core
open Fable.Core.JsInterop
open Lit

/// Internal JavaScript interop for Lit template construction.
module internal JsInterop =
    /// Creates a TemplateStringsArray-like object for Lit's html tag function.
    /// Tagged template literals expect an array with a 'raw' property.
    [<Emit("Object.freeze(Object.assign($0, { raw: $0 }))")>]
    let makeTemplateStrings (strings: string[]) : obj = jsNative

    /// Calls a tagged template function with string parts and values.
    [<Emit("$0($1, ...$2)")>]
    let callTagFn (tagFn: obj) (strings: obj) (values: obj[]) : TemplateResult = jsNative

    /// The Lit html tag function.
    [<Import("html", "lit")>]
    let htmlTagFn : obj = jsNative

    /// The Lit ref directive for capturing element references.
    [<Import("ref", "lit/directives/ref.js")>]
    let litRef : (obj -> obj) = jsNative

    /// Creates an empty TemplateResult (Lit.nothing equivalent).
    [<Emit("$0``")>]
    let emptyTemplate (tagFn: obj) : TemplateResult = jsNative

    /// JavaScript Map for caching template strings.
    [<Emit("new Map()")>]
    let createMap () : obj = jsNative

    [<Emit("$0.get($1)")>]
    let mapGet (map: obj) (key: string) : obj option = jsNative

    [<Emit("$0.set($1, $2)")>]
    let mapSet (map: obj) (key: string) (value: obj) : unit = jsNative

/// Template strings cache to ensure Lit can efficiently diff templates.
/// Lit uses the template strings array reference as a cache key.
module internal TemplateCache =
    let private cache = JsInterop.createMap ()

    /// Gets or creates a cached template strings array for the given strings.
    let getTemplateStrings (strings: string list) : obj =
        let key = String.concat "\u0000" strings
        match JsInterop.mapGet cache key with
        | Some cached -> cached
        | None ->
            let templateStrings = JsInterop.makeTemplateStrings (List.toArray strings)
            JsInterop.mapSet cache key templateStrings
            templateStrings

/// Converts DSL Nodes to Lit TemplateResults.
[<RequireQualifiedAccess>]
module Renderer =

    /// Creates a Lit template from string parts and interpolated values.
    /// Uses cached template strings to ensure Lit can efficiently diff templates.
    let private createTemplate (strings: string list) (values: obj list) : TemplateResult =
        let templateStrings = TemplateCache.getTemplateStrings strings
        JsInterop.callTagFn JsInterop.htmlTagFn templateStrings (List.toArray values)

    /// Wraps a setter callback to convert undefined to None.
    let private wrapRefCallback (setter: obj option -> unit) : obj -> unit =
        fun el ->
            if isNull el || el = Fable.Core.JS.undefined then
                setter None
            else
                setter (Some el)

    /// Renders a single attribute to a string for static attributes,
    /// or returns the value for dynamic binding.
    let private renderAttrString (attr: Attr) : string * obj option =
        match attr with
        | Attr(name, value) ->
            // For simple string values, render inline
            match value with
            | :? string as s -> $" {name}=\"{s}\"", None
            | :? int as i -> $" {name}=\"{i}\"", None
            | :? float as f -> $" {name}=\"{f}\"", None
            | :? bool as b ->
                let boolStr = if b then "true" else "false"
                $" {name}=\"{boolStr}\"", None
            | _ -> $" {name}=", Some value
        | BoolAttr(name, enabled) ->
            if enabled then $" {name}", None
            else "", None
        | Prop(name, value) ->
            // Property bindings use Lit's .property=${value} syntax
            $" .{name}=", Some value
        | Event(name, handler) ->
            // Event handlers need to be interpolated with Lit's @event syntax
            $" @{name}=", Some handler
        | Ref setter ->
            // Ref bindings use Lit's ref directive: ${ref(callback)}
            let wrappedCallback = wrapRefCallback setter
            let refDirective = JsInterop.litRef wrappedCallback
            " ", Some refDirective

    /// Renders a list of attributes, returning the static string parts and dynamic values.
    let private renderAttrs (attrs: Attr list) : string list * obj list =
        let rec loop remainingAttrs (strings: string list) (values: obj list) =
            match remainingAttrs with
            | [] -> List.rev strings, List.rev values
            | attr :: restAttrs ->
                let str, valueOpt = renderAttrString attr
                match valueOpt with
                | None ->
                    // Static attribute - append to current string
                    match strings with
                    | current :: restStrings -> loop restAttrs ((current + str) :: restStrings) values
                    | [] -> loop restAttrs [str] values
                | Some value ->
                    // Dynamic attribute - add placeholder and value
                    match strings with
                    | current :: restStrings ->
                        loop restAttrs ("" :: (current + str) :: restStrings) (value :: values)
                    | [] ->
                        loop restAttrs [""; str] (value :: values)
        loop attrs [""] []

    /// Renders a Node to a TemplateResult.
    let rec render (node: Node) : TemplateResult =
        match node with
        | Text t ->
            // Text nodes are just strings - Lit handles escaping
            createTemplate [t] []

        | RawHtml raw ->
            // Use Lit's unsafeHTML directive
            LitBindings.unsafeHTML raw

        | Template t ->
            // Already a TemplateResult, pass through
            t

        | Fragment [] ->
            // Empty fragment
            Lit.nothing

        | Fragment nodes ->
            // Render all nodes and combine them
            let results = nodes |> List.map render
            createTemplate
                ([ "" ] @ List.replicate (results.Length - 1) "" @ [ "" ])
                (results |> List.map box)

        | AttrNode _ ->
            // AttrNodes should be filtered out before rendering
            Lit.nothing

        | Nothing ->
            // Empty node
            Lit.nothing

        | Element(tag, attrs, children) ->
            renderElement tag attrs children

    /// Renders an element node with its attributes and children.
    and private renderElement (tag: string) (attrs: Attr list) (children: Node list) : TemplateResult =
        // Build the template strings and values
        // Pattern: ["<tag attrs>", "", ..., "</tag>"] with child TemplateResults as values

        // Separate static vs dynamic attributes
        let attrStrings, attrValues = renderAttrs attrs

        // Render children
        let renderedChildren = children |> List.map render

        // Build the complete template
        match attrStrings, attrValues, renderedChildren with
        | [attrStr], [], [] ->
            // No dynamic attrs, no children: simple static element
            let template = $"<{tag}{attrStr}></{tag}>"
            createTemplate [template] []

        | [attrStr], [], children ->
            // No dynamic attrs, with children
            let openTag = $"<{tag}{attrStr}>"
            let closeTag = $"</{tag}>"
            let strings = [openTag] @ List.replicate (children.Length - 1) "" @ [closeTag]
            createTemplate strings (children |> List.map box)

        | attrStrs, attrVals, [] ->
            // Dynamic attrs, no children
            let openParts = attrStrs |> List.mapi (fun i s ->
                if i = 0 then $"<{tag}{s}"
                else s
            )
            let lastIdx = openParts.Length - 1
            let openParts' =
                openParts |> List.mapi (fun i s ->
                    if i = lastIdx then s + $"></{tag}>"
                    else s
                )
            createTemplate openParts' attrVals

        | attrStrs, attrVals, children ->
            // Dynamic attrs and children
            let openParts = attrStrs |> List.mapi (fun i s ->
                if i = 0 then $"<{tag}{s}"
                else s
            )
            let lastIdx = openParts.Length - 1
            let openParts' =
                openParts |> List.mapi (fun i s ->
                    if i = lastIdx then s + ">"
                    else s
                )
            let closeTag = $"</{tag}>"

            // Combine: open parts + (empty strings between children) + close tag
            let childStrings = List.replicate (children.Length - 1) ""
            let allStrings = openParts' @ childStrings @ [closeTag]
            let allValues = attrVals @ (children |> List.map box)

            createTemplate allStrings allValues

    /// Renders a Node to a TemplateResult (alias for render).
    let toTemplateResult = render
