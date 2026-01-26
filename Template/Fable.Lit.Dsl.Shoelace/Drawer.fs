namespace Fable.Lit.Dsl.Shoelace

open Fable.Core.JsInterop
open Fable.Lit.Dsl

/// A strongly-typed reference to a Shoelace drawer element.
/// This provides a typed abstraction over raw JS interop for imperative drawer control.
/// DrawerRef is a class (reference type), so it persists across renders.
/// No Hook.useRef is required.
type DrawerRef() =
    member val Element: obj option = None with get, set

/// Functions for working with Shoelace drawers.
/// This module provides a typed API for creating drawer refs and controlling drawers imperatively.
///
/// Usage:
///   let drawer = Drawer.createRef()
///
///   slDrawer {
///       Drawer.bind drawer
///       label' "Settings"
///       // drawer content
///   }
///
///   slButton {
///       onClick (fun _ -> Drawer.show drawer)
///       "Open Drawer"
///   }
module Drawer =

    /// Creates a new drawer reference.
    let createRef () = DrawerRef()

    /// Binds a Shoelace drawer element to a DrawerRef.
    /// Ignores null/undefined values that Lit may pass when clearing the part.
    let bind (r: DrawerRef) =
        bindRef (fun el ->
            // el can be null/undefined when the part is being cleared.
            if not (isNullOrUndefined el) then
                r.Element <- Some el
        )

    let show (r: DrawerRef) =
        match r.Element with
        | Some el -> el?show() |> ignore
        | None -> ()

    let hide (r: DrawerRef) =
        match r.Element with
        | Some el -> el?hide() |> ignore
        | None -> ()
