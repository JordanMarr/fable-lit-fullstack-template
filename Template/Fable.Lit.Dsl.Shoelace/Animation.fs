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

    /// Starts the animation by setting the play property to true.
    let play (r: AnimationRef) =
        match r.Element with
        | Some el -> el?play <- true
        | None -> ()

    /// Stops the animation by setting the play property to false.
    let stop (r: AnimationRef) =
        r.Element 
        |> Option.iter (fun el -> el?play <- false)

    /// Cancels the animation and resets it to the initial state.
    let cancel (r: AnimationRef) =
        match r.Element with
        | Some el ->
            el?play <- false
            // Access the underlying Web Animation API to cancel
            let animation = el?animation
            if not (isNullOrUndefined animation) then
                animation?cancel() |> ignore
        | None -> ()
