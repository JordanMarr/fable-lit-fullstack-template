namespace Fable.Lit.Dsl.Shoelace

open Fable.Lit.Dsl

/// Property helpers for Shoelace web components.
/// Uses `prop` for Shoelace properties (not `attr`) as Shoelace
/// components are web components with JavaScript properties.
[<AutoOpen>]
module ShoelaceAttrs =
    // =========================================================================
    // Variant properties
    // =========================================================================

    /// Generic variant property.
    let variant (value: string) = prop "variant" value

    /// Primary variant (typically blue).
    let variantPrimary = prop "variant" "primary"

    /// Success variant (typically green).
    let variantSuccess = prop "variant" "success"

    /// Neutral variant (typically gray).
    let variantNeutral = prop "variant" "neutral"

    /// Warning variant (typically yellow/orange).
    let variantWarning = prop "variant" "warning"

    /// Danger variant (typically red).
    let variantDanger = prop "variant" "danger"

    /// Text variant (no background).
    let variantText = prop "variant" "text"

    /// Default variant.
    let variantDefault = prop "variant" "default"

    // =========================================================================
    // Size properties
    // =========================================================================

    /// Generic size property.
    let size (value: string) = prop "size" value

    /// Small size.
    let sizeSmall = prop "size" "small"

    /// Medium size (default).
    let sizeMedium = prop "size" "medium"

    /// Large size.
    let sizeLarge = prop "size" "large"

    // =========================================================================
    // State properties
    // =========================================================================

    /// Sets the disabled state.
    let disabled' (isDisabled: bool) = prop "disabled" isDisabled

    /// Sets the loading state.
    let loading (isLoading: bool) = prop "loading" isLoading

    /// Sets the checked state (for checkboxes, switches, radios).
    let checked' (isChecked: bool) = prop "checked" isChecked

    /// Sets the open state (for dialogs, drawers, dropdowns, etc.).
    let open' (isOpen: bool) = prop "open" isOpen

    /// Sets the indeterminate state (for checkboxes, progress).
    let indeterminate (isIndeterminate: bool) = prop "indeterminate" isIndeterminate

    /// Sets the closable state (for alerts, tags).
    let closable (isClosable: bool) = prop "closable" isClosable

    /// Sets the required state (for form controls).
    let required' (isRequired: bool) = prop "required" isRequired

    /// Sets the readonly state (for inputs).
    let readonly' (isReadonly: bool) = prop "readonly" isReadonly

    /// Sets the clearable state (for inputs, selects).
    let clearable (isClearable: bool) = prop "clearable" isClearable

    /// Sets the filled state (for inputs).
    let filled (isFilled: bool) = prop "filled" isFilled

    /// Sets the pill style (rounded corners).
    let pill (isPill: bool) = prop "pill" isPill

    /// Sets the outline style.
    let outline (isOutline: bool) = prop "outline" isOutline

    /// Sets the circle style (for buttons).
    let circle (isCircle: bool) = prop "circle" isCircle

    /// Sets the caret visibility (for dropdowns).
    let caret (showCaret: bool) = prop "caret" showCaret

    /// Sets the hoist state (for menus, dropdowns).
    let hoist (isHoisted: bool) = prop "hoist" isHoisted

    /// Sets the removable state (for tags).
    let removable (isRemovable: bool) = prop "removable" isRemovable

    // =========================================================================
    // Value properties
    // =========================================================================

    /// Sets the value property.
    let value' (v: obj) = prop "value" v

    /// Sets the label property.
    let label' (text: string) = prop "label" text

    /// Sets the placeholder property.
    let placeholder' (text: string) = prop "placeholder" text

    /// Sets the help text property.
    let helpText (text: string) = prop "helpText" text

    /// Sets the name property.
    let name' (text: string) = prop "name" text

    /// Sets the icon name property.
    let iconName (name: string) = prop "name" name

    /// Sets the src property (for images, includes).
    let src' (url: string) = prop "src" url

    /// Sets the href property (for links, buttons).
    let href' (url: string) = prop "href" url

    /// Sets the target property (for links).
    let target' (value: string) = prop "target" value

    /// Sets the type property (for inputs, buttons).
    let type'' (value: string) = prop "type" value

    /// Sets the pattern property (for inputs).
    let pattern' (regex: string) = prop "pattern" regex

    // =========================================================================
    // Numeric properties
    // =========================================================================

    /// Sets the min value.
    let min' (value: obj) = prop "min" value

    /// Sets the max value.
    let max' (value: obj) = prop "max" value

    /// Sets the step value.
    let step' (value: obj) = prop "step" value

    /// Sets the minlength value.
    let minlength' (value: int) = prop "minlength" value

    /// Sets the maxlength value.
    let maxlength' (value: int) = prop "maxlength" value

    /// Sets the rows value (for textareas).
    let rows' (value: int) = prop "rows" value

    // =========================================================================
    // Slot helper
    // =========================================================================

    /// Sets the slot attribute for placing content in named slots.
    let slot' (name: string) = attr "slot" name

    // =========================================================================
    // Placement properties
    // =========================================================================

    /// Generic placement property.
    let placement (value: string) = prop "placement" value

    /// Top placement.
    let placementTop = prop "placement" "top"

    /// Top-start placement.
    let placementTopStart = prop "placement" "top-start"

    /// Top-end placement.
    let placementTopEnd = prop "placement" "top-end"

    /// Bottom placement.
    let placementBottom = prop "placement" "bottom"

    /// Bottom-start placement.
    let placementBottomStart = prop "placement" "bottom-start"

    /// Bottom-end placement.
    let placementBottomEnd = prop "placement" "bottom-end"

    /// Left placement.
    let placementLeft = prop "placement" "left"

    /// Left-start placement.
    let placementLeftStart = prop "placement" "left-start"

    /// Left-end placement.
    let placementLeftEnd = prop "placement" "left-end"

    /// Right placement.
    let placementRight = prop "placement" "right"

    /// Right-start placement.
    let placementRightStart = prop "placement" "right-start"

    /// Right-end placement.
    let placementRightEnd = prop "placement" "right-end"

    // =========================================================================
    // Dialog/Drawer specific
    // =========================================================================

    /// Sets the modal behavior (prevents closing by clicking overlay).
    let modal (isModal: bool) = prop "modal" isModal

    /// Sets whether clicking the overlay closes the element.
    let noHeader (hide: bool) = prop "noHeader" hide

    // =========================================================================
    // Tab specific
    // =========================================================================

    /// Sets the panel name (for tabs).
    let panel (name: string) = prop "panel" name

    /// Sets the active state (for tabs).
    let active (isActive: bool) = prop "active" isActive

    /// Sets the tab activation behavior.
    let activation (value: string) = prop "activation" value
