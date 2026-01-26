namespace Fable.Lit.Dsl.Shoelace

open Fable.Core.JsInterop
open Fable.Lit.Dsl

/// A strongly-typed reference to a Shoelace dialog element.
/// This provides a typed abstraction over raw JS interop for imperative dialog control.
type DialogRef =
    { mutable Element: obj option }

/// Functions for working with Shoelace dialogs.
/// This module provides a typed API for creating dialog refs and controlling dialogs imperatively.
///
/// Usage:
///   let dialog = Dialog.createRef()
///
///   slDialog {
///       Dialog.bind dialog
///       label' "My Dialog"
///       // dialog content
///   }
///
///   slButton {
///       onClick (fun _ -> Dialog.show dialog)
///       "Open"
///   }
///
/// This pattern can be replicated for other imperative Shoelace components:
/// - Drawer: DrawerRef with show/hide
/// - Tooltip: TooltipRef with show/hide
/// - Menu: MenuRef with show/hide
/// - Animation: AnimationRef with play/cancel
module Dialog =

    /// Creates a new dialog reference.
    let createRef () : DialogRef =
        { Element = None }

    /// Alias for createRef for more concise usage.
    /// Usage: let dialog = Dialog.ref()
    let inline ref () = createRef()

    /// Binds the dialog reference to the element.
    /// Use this inside an slDialog builder to capture the element reference.
    let bind (r: DialogRef) : Attr =
        bindRef (fun el -> r.Element <- Some el)

    /// Shows the dialog.
    let show (r: DialogRef) =
        match r.Element with
        | Some el -> el?show() |> ignore
        | None -> ()

    /// Hides the dialog.
    let hide (r: DialogRef) =
        match r.Element with
        | Some el -> el?hide() |> ignore
        | None -> ()
