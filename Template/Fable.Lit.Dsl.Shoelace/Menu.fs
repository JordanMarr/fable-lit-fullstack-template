namespace Fable.Lit.Dsl.Shoelace

open Fable.Core.JsInterop
open Fable.Lit.Dsl

/// A strongly-typed reference to a Shoelace menu element.
/// This provides a typed abstraction over raw JS interop for imperative menu control.
/// MenuRef is a class (reference type), so it persists across renders.
/// No Hook.useRef is required.
type MenuRef() =
    member val Element: obj option = None with get, set

/// Functions for working with Shoelace menus.
/// This module provides a typed API for creating menu refs and controlling menus imperatively.
/// Shoelace sl-menu supports show()/hide() when used in overlays (e.g., dropdowns).
///
/// Usage:
///   let menu = Menu.createRef()
///
///   slMenu {
///       Menu.bind menu
///       slMenuItem { "Item 1" }
///       slMenuItem { "Item 2" }
///   }
module Menu =

    /// Creates a new menu reference.
    let createRef () = MenuRef()

    /// Binds a Shoelace menu element to a MenuRef.
    /// Ignores null/undefined values that Lit may pass when clearing the part.
    let bind (r: MenuRef) =
        bindRef (fun el ->
            // el can be null/undefined when the part is being cleared.
            if not (isNullOrUndefined el) then
                r.Element <- Some el
        )

    let show (r: MenuRef) =
        match r.Element with
        | Some el -> el?show() |> ignore
        | None -> ()

    let hide (r: MenuRef) =
        match r.Element with
        | Some el -> el?hide() |> ignore
        | None -> ()
