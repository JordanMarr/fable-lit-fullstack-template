namespace Fable.Lit.Dsl.Shoelace

open Fable.Core.JsInterop
open Fable.Lit.Dsl

/// A strongly-typed reference to a Shoelace details element.
/// This provides a typed abstraction over raw JS interop for imperative details control.
/// DetailsRef is a class (reference type), so it persists across renders.
/// No Hook.useRef is required.
type DetailsRef() =
    member val Element: obj option = None with get, set

/// Functions for working with Shoelace details (expandable/collapsible sections).
/// This module provides a typed API for creating details refs and controlling details imperatively.
/// Shoelace sl-details supports show()/hide() to expand/collapse.
///
/// Usage:
///   let details = Details.createRef()
///
///   slDetails {
///       Details.bind details
///       summary' "Click to expand"
///       "Content goes here..."
///   }
///
///   slButton {
///       onClick (fun _ -> Details.show details)
///       "Expand"
///   }
module Details =

    /// Creates a new details reference.
    let createRef () = DetailsRef()

    /// Binds a Shoelace details element to a DetailsRef.
    /// Ignores null/undefined values that Lit may pass when clearing the part.
    let bind (r: DetailsRef) =
        bindRef (fun el ->
            // el can be null/undefined when the part is being cleared.
            if not (isNullOrUndefined el) then
                r.Element <- Some el
        )

    let show (r: DetailsRef) =
        match r.Element with
        | Some el -> el?show() |> ignore
        | None -> ()

    let hide (r: DetailsRef) =
        match r.Element with
        | Some el -> el?hide() |> ignore
        | None -> ()
