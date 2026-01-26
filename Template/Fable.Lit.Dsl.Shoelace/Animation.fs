namespace Fable.Lit.Dsl.Shoelace

open Fable.Core.JsInterop
open Fable.Lit.Dsl

/// A strongly-typed reference to a Shoelace animation element.
/// This provides a typed abstraction over raw JS interop for imperative animation control.
/// AnimationRef is a class (reference type), so it persists across renders.
/// No Hook.useRef is required.
type AnimationRef() =
    member val Element: obj option = None with get, set

/// Functions for working with Shoelace animations.
/// This module provides a typed API for creating animation refs and controlling animations imperatively.
/// Note: sl-animation uses play()/cancel() instead of show()/hide().
///
/// Usage:
///   let animation = Animation.createRef()
///
///   slAnimation {
///       Animation.bind animation
///       name' "bounce"
///       duration 1000
///       slButton { "Animated!" }
///   }
///
///   slButton {
///       onClick (fun _ -> Animation.play animation)
///       "Play Animation"
///   }
module Animation =

    /// Creates a new animation reference.
    let createRef () = AnimationRef()

    /// Binds a Shoelace animation element to an AnimationRef.
    /// Ignores null/undefined values that Lit may pass when clearing the part.
    let bind (r: AnimationRef) =
        bindRef (fun el ->
            // el can be null/undefined when the part is being cleared.
            if not (isNullOrUndefined el) then
                r.Element <- Some el
        )

    let play (r: AnimationRef) =
        match r.Element with
        | Some el -> el?play() |> ignore
        | None -> ()

    let cancel (r: AnimationRef) =
        match r.Element with
        | Some el -> el?cancel() |> ignore
        | None -> ()
