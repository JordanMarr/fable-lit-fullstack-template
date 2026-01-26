namespace Fable.Lit.Dsl.Shoelace

open Fable.Core.JsInterop
open Fable.Lit.Dsl

/// A strongly-typed reference to a Shoelace dialog element.
/// This provides a typed abstraction over raw JS interop for imperative dialog control.
/// DialogRef is a class (reference type), so it persists across renders.
/// No Hook.useRef is required.
type DialogRef() =
    member val Element: obj option = None with get, set

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
    let createRef () = DialogRef()

    /// Binds a Shoelace dialog element to a DialogRef.
    /// Ignores null/undefined values that Lit may pass when clearing the part.
    let bind (r: DialogRef) =
        bindRef (fun el ->
            // el can be null/undefined when the part is being cleared.
            if not (isNullOrUndefined el) then
                r.Element <- Some el
        )

    let show (r: DialogRef) =
        match r.Element with
        | Some el -> el?show() |> ignore
        | None -> ()

    let hide (r: DialogRef) =
        match r.Element with
        | Some el -> el?hide() |> ignore
        | None -> ()
