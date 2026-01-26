namespace Fable.Lit.Dsl.Shoelace

open Fable.Core.JsInterop
open Fable.Lit.Dsl

/// A strongly-typed reference to a Shoelace tooltip element.
/// This provides a typed abstraction over raw JS interop for imperative tooltip control.
/// TooltipRef is a class (reference type), so it persists across renders.
/// No Hook.useRef is required.
type TooltipRef() =
    member val Element: obj option = None with get, set

/// Functions for working with Shoelace tooltips.
/// This module provides a typed API for creating tooltip refs and controlling tooltips imperatively.
///
/// Usage:
///   let tooltip = Tooltip.createRef()
///
///   slTooltip {
///       Tooltip.bind tooltip
///       content' "More info"
///       slButton { "Hover or focus me" }
///   }
///
///   // Optional: imperative show/hide if needed
///   Tooltip.show tooltip
module Tooltip =

    /// Creates a new tooltip reference.
    let createRef () = TooltipRef()

    /// Binds a Shoelace tooltip element to a TooltipRef.
    /// Ignores null/undefined values that Lit may pass when clearing the part.
    let bind (r: TooltipRef) =
        bindRef (fun el ->
            // el can be null/undefined when the part is being cleared.
            if not (isNullOrUndefined el) then
                r.Element <- Some el
        )

    let show (r: TooltipRef) =
        match r.Element with
        | Some el -> el?show() |> ignore
        | None -> ()

    let hide (r: TooltipRef) =
        match r.Element with
        | Some el -> el?hide() |> ignore
        | None -> ()
