module WebLit.Util

open Fable.Core
open Browser
open Browser.Types
open Elmish
open Lit
open Lit.Elmish

module LitLabs =
    type ScrollLayout =
        interface end

    [<StringEnum; RequireQualifiedAccess>]
    type ScrollToPosition =
        | Start
        | Center
        | End
        | Nearest

    [<AttachMembers>]
    type ScrollToIndex(index: int, ?position: ScrollToPosition) =
        member _.index = index
        member _.position = defaultArg position ScrollToPosition.Start

    type Motion =
        abstract animate: unit -> unit

    [<ImportAll("@lit-labs/motion")>]
    let motion: Motion = jsNative

    type Virtualizer =
        abstract Layout1d: ScrollLayout
        abstract Layout1dGrid: ScrollLayout
        [<ParamObject>]
        abstract scroll:
            items: 'T array *
            renderItem: ('T -> int -> TemplateResult) *
            layout: ScrollLayout *
            ?keyFunction: ('T -> string) *
            ?scrollTarget: Element *
            ?totalItems: int *
            ?scrollToIndex: ScrollToIndex -> unit

    [<ImportAll("@lit-labs/virtualizer")>]
    let virtualizer: Virtualizer = jsNative

module Storage =
    let mapInit decode storageKey (init: unit -> 'model * Cmd<'msg>) =
        fun () ->
            let defaultModel, cmd = init()
            match localStorage.getItem(storageKey) with
            | null -> defaultModel, cmd
            | json ->
                try
                    let stored = decode json
                    stored, cmd
                with e ->
                    JS.console.warn($"Cannot decode localStorage '{storageKey}'", e.Message)
                    defaultModel, cmd

    let mapUpdate encode storageKey (update: 'msg -> 'model -> 'model * Cmd<'msg>) =
        fun msg model ->
            let newModel, cmd = update msg model
            localStorage.setItem(storageKey, encode newModel)
            newModel, cmd

    let inline generateCoders<'T>() =
        let encoder =
            let enc = Thoth.Json.Encode.Auto.generateEncoder<'T>()
            fun model -> enc model |> Thoth.Json.Encode.toString 0

        let decoder =
            let dec = Thoth.Json.Decode.Auto.generateDecoder<'T>()
            fun json -> Thoth.Json.Decode.unsafeFromString dec json

        encoder, decoder

module Program =
    /// Load/save Elmish state to browser's localStorage.
    ///
    /// Better used in apps that don't update the Elmish model on every key stroke to prevent hitting localStorage too many times.
    let inline withLocalStorage (storageKey: string) (program: Program<unit, 'model, 'msg, 'view>) =
        let encoder, decoder = Storage.generateCoders()
        Program.map (Storage.mapInit decoder storageKey) (Storage.mapUpdate encoder storageKey) id id id program

type Hook with
    /// Load/save Elmish state to browser's localStorage.
    ///
    /// Better used in apps that don't update the Elmish model on every key stroke to prevent hitting localStorage too many times.
    static member inline useElmishWithLocalStorage(init, update, ?storageKey: string) =
        let init, update =
            match storageKey with
            | Some storageKey when not(System.String.IsNullOrWhiteSpace(storageKey)) ->
                let storageKey = storageKey.Trim()
                let encoder, decoder = Storage.generateCoders()
                Storage.mapInit decoder storageKey init, Storage.mapUpdate encoder storageKey update
            | _ ->
                init, update
        Hook.useElmish(init, update)

let onEnterOrEscape onEnter onEscape (ev: Event) =
    let ev = ev :?> KeyboardEvent
    match ev.key with
    | "Enter" -> onEnter ev
    | "Escape" -> onEscape ev
    | _ -> ()

[<Emit("fetch($0).then(x => x.json())")>]
let fetchJson<'T> (url: string): JS.Promise<'T> = jsNative

type Option<'T> with
    member this.Iter(f) =
        Option.iter f this

type LitBindings with
    [<ImportMember("lit")>]
    static member html': Template.JsTag<TemplateResult> = jsNative

[<AutoOpen>]
module StringBuffer =
    open System.Text

    type StringBuffer = StringBuilder -> unit

    type StringBufferBuilder () =
        member inline __.Yield (txt: string) = fun (b: StringBuilder) -> Printf.bprintf b "%s" txt
        member inline __.Yield (c: char) = fun (b: StringBuilder) -> Printf.bprintf b "%c" c
        member inline __.Yield (strings: #seq<string>) =
            fun (b: StringBuilder) -> for s in strings do Printf.bprintf b "%s\n" s
        member inline __.YieldFrom (f: StringBuffer) = f
        member __.Combine (f, g) = fun (b: StringBuilder) -> f b; g b
        member __.Delay f = fun (b: StringBuilder) -> (f()) b
        member __.Zero () = ignore

        member __.For (xs: 'a seq, f: 'a -> StringBuffer) =
            fun (b: StringBuilder) ->
                use e = xs.GetEnumerator ()
                while e.MoveNext() do
                    (f e.Current) b

        member __.While (p: unit -> bool, f: StringBuffer) =
            fun (b: StringBuilder) -> while p () do f b

        member __.Run (f: StringBuffer) =
            let b = StringBuilder()
            do f b
            b.ToString()

    let stringBuffer = new StringBufferBuilder ()

    type StringBufferBuilder with
      member inline __.Yield (b: byte) = fun (sb: StringBuilder) -> Printf.bprintf sb "%02x " b


[<AutoOpen>]
module HtmlBuffer =
    open System.Text

    type HtmlBuffer = StringBuilder -> unit

    type HtmlBufferBuilder () =
        member inline __.Yield (txt: string) = fun (b: StringBuilder) -> Printf.bprintf b "%s" txt
        member inline __.Yield (c: char) = fun (b: StringBuilder) -> Printf.bprintf b "%c" c
        member inline __.Yield (strings: #seq<string>) =
            fun (b: StringBuilder) -> for s in strings do Printf.bprintf b "%s\n" s
        member inline __.YieldFrom (f: HtmlBuffer) = f
        member __.Combine (f, g) = fun (b: StringBuilder) -> f b; g b
        member __.Delay f = fun (b: StringBuilder) -> (f()) b
        member __.Zero () = ignore

        member __.For (xs: 'a seq, f: 'a -> HtmlBuffer) =
            fun (b: StringBuilder) ->
                use e = xs.GetEnumerator ()
                while e.MoveNext() do
                    (f e.Current) b

        member __.While (p: unit -> bool, f: HtmlBuffer) =
            fun (b: StringBuilder) -> while p () do f b

        member __.Run (f: HtmlBuffer) =
            let b = StringBuilder()
            do f b
            html $"{b}"

    let html' = new HtmlBufferBuilder ()

    type HtmlBufferBuilder with
      member inline __.Yield (b: byte) = fun (sb: StringBuilder) -> Printf.bprintf sb "%02x " b
