namespace Fable.Lit.Dsl.Shoelace

open Fable.Lit.Dsl

/// Event helpers for Shoelace web components.
/// All handlers receive `obj` which can be cast to specific event types if needed.
[<AutoOpen>]
module ShoelaceEvents =
    // =========================================================================
    // Form control events
    // =========================================================================

    /// Fired when the value changes (after user interaction).
    let onSlChange (handler: obj -> unit) : Attr = Event("sl-change", handler)

    /// Fired as the user types (real-time input).
    let onSlInput (handler: obj -> unit) : Attr = Event("sl-input", handler)

    /// Fired when the control receives focus.
    let onSlFocus (handler: obj -> unit) : Attr = Event("sl-focus", handler)

    /// Fired when the control loses focus.
    let onSlBlur (handler: obj -> unit) : Attr = Event("sl-blur", handler)

    /// Fired when the control's value is cleared.
    let onSlClear (handler: obj -> unit) : Attr = Event("sl-clear", handler)

    /// Fired when the control is invalid.
    let onSlInvalid (handler: obj -> unit) : Attr = Event("sl-invalid", handler)

    // =========================================================================
    // Dialog/Drawer/Dropdown events
    // =========================================================================

    /// Fired when the element starts to show.
    let onSlShow (handler: obj -> unit) : Attr = Event("sl-show", handler)

    /// Fired after the element has shown and animations complete.
    let onSlAfterShow (handler: obj -> unit) : Attr = Event("sl-after-show", handler)

    /// Fired when the element starts to hide.
    let onSlHide (handler: obj -> unit) : Attr = Event("sl-hide", handler)

    /// Fired after the element has hidden and animations complete.
    let onSlAfterHide (handler: obj -> unit) : Attr = Event("sl-after-hide", handler)

    /// Fired when the initial focus is set (dialogs/drawers).
    let onSlInitialFocus (handler: obj -> unit) : Attr = Event("sl-initial-focus", handler)

    /// Fired when a request to close is made (dialogs/drawers).
    let onSlRequestClose (handler: obj -> unit) : Attr = Event("sl-request-close", handler)

    // =========================================================================
    // Alert events
    // =========================================================================

    /// Fired when the alert closes.
    let onSlClose (handler: obj -> unit) : Attr = Event("sl-close", handler)

    /// Fired after the alert closes and animations complete.
    let onSlAfterClose (handler: obj -> unit) : Attr = Event("sl-after-close", handler)

    // =========================================================================
    // Tab events
    // =========================================================================

    /// Fired when a tab is shown.
    let onSlTabShow (handler: obj -> unit) : Attr = Event("sl-tab-show", handler)

    /// Fired when a tab is hidden.
    let onSlTabHide (handler: obj -> unit) : Attr = Event("sl-tab-hide", handler)

    // =========================================================================
    // Menu events
    // =========================================================================

    /// Fired when a menu item is selected.
    let onSlSelect (handler: obj -> unit) : Attr = Event("sl-select", handler)

    // =========================================================================
    // Tree events
    // =========================================================================

    /// Fired when a tree item is selected.
    let onSlSelectionChange (handler: obj -> unit) : Attr = Event("sl-selection-change", handler)

    /// Fired when a tree item expands.
    let onSlExpand (handler: obj -> unit) : Attr = Event("sl-expand", handler)

    /// Fired after a tree item expands and animations complete.
    let onSlAfterExpand (handler: obj -> unit) : Attr = Event("sl-after-expand", handler)

    /// Fired when a tree item collapses.
    let onSlCollapse (handler: obj -> unit) : Attr = Event("sl-collapse", handler)

    /// Fired after a tree item collapses and animations complete.
    let onSlAfterCollapse (handler: obj -> unit) : Attr = Event("sl-after-collapse", handler)

    /// Fired when a tree item's lazy content should be loaded.
    let onSlLazyLoad (handler: obj -> unit) : Attr = Event("sl-lazy-load", handler)

    /// Fired when a tree item's lazy loading completes.
    let onSlLazyChange (handler: obj -> unit) : Attr = Event("sl-lazy-change", handler)

    // =========================================================================
    // Details events
    // =========================================================================

    // sl-show, sl-after-show, sl-hide, sl-after-hide are already defined above

    // =========================================================================
    // Carousel events
    // =========================================================================

    /// Fired when the active slide changes.
    let onSlSlideChange (handler: obj -> unit) : Attr = Event("sl-slide-change", handler)

    // =========================================================================
    // Copy button events
    // =========================================================================

    /// Fired when content is copied.
    let onSlCopy (handler: obj -> unit) : Attr = Event("sl-copy", handler)

    /// Fired when copying fails.
    let onSlError (handler: obj -> unit) : Attr = Event("sl-error", handler)

    // =========================================================================
    // Resize/Mutation observer events
    // =========================================================================

    /// Fired when a resize is observed.
    let onSlResize (handler: obj -> unit) : Attr = Event("sl-resize", handler)

    /// Fired when a mutation is observed.
    let onSlMutation (handler: obj -> unit) : Attr = Event("sl-mutation", handler)

    // =========================================================================
    // Popup events
    // =========================================================================

    /// Fired when the popup is repositioned.
    let onSlReposition (handler: obj -> unit) : Attr = Event("sl-reposition", handler)

    // =========================================================================
    // Include events
    // =========================================================================

    /// Fired when included content loads.
    let onSlLoad (handler: obj -> unit) : Attr = Event("sl-load", handler)
    // onSlError is already defined above

    // =========================================================================
    // Rating events
    // =========================================================================

    /// Fired when the user hovers over a rating value.
    let onSlHover (handler: obj -> unit) : Attr = Event("sl-hover", handler)

    // =========================================================================
    // Tag events
    // =========================================================================

    /// Fired when the tag's remove button is clicked.
    let onSlRemove (handler: obj -> unit) : Attr = Event("sl-remove", handler)

    // =========================================================================
    // Split panel events
    // =========================================================================

    /// Fired when the divider position changes.
    let onSlReposition' (handler: obj -> unit) : Attr = Event("sl-reposition", handler)
