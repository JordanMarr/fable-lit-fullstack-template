namespace Fable.Lit.Dsl.Shoelace

open Fable.Lit.Dsl

/// Element helpers for Shoelace web components.
/// These are thin wrappers over the core DSL's ElementBuilder.
[<AutoOpen>]
module ShoelaceElements =

    // Buttons
    let slButton = ElementBuilder("sl-button")
    let slButtonGroup = ElementBuilder("sl-button-group")
    let slIconButton = ElementBuilder("sl-icon-button")
    let slCopyButton = ElementBuilder("sl-copy-button")

    // Form controls
    let slInput = ElementBuilder("sl-input")
    let slTextarea = ElementBuilder("sl-textarea")
    let slSelect = ElementBuilder("sl-select")
    let slOption = ElementBuilder("sl-option")
    let slCheckbox = ElementBuilder("sl-checkbox")
    let slSwitch = ElementBuilder("sl-switch")
    let slRadio = ElementBuilder("sl-radio")
    let slRadioButton = ElementBuilder("sl-radio-button")
    let slRadioGroup = ElementBuilder("sl-radio-group")
    let slRange = ElementBuilder("sl-range")
    let slColorPicker = ElementBuilder("sl-color-picker")
    let slRating = ElementBuilder("sl-rating")

    // Feedback
    let slAlert = ElementBuilder("sl-alert")
    let slDialog = ElementBuilder("sl-dialog")
    let slDrawer = ElementBuilder("sl-drawer")
    let slTooltip = ElementBuilder("sl-tooltip")
    let slPopup = ElementBuilder("sl-popup")
    let slSpinner = ElementBuilder("sl-spinner")
    let slProgressBar = ElementBuilder("sl-progress-bar")
    let slProgressRing = ElementBuilder("sl-progress-ring")
    let slSkeleton = ElementBuilder("sl-skeleton")

    // Navigation
    let slBreadcrumb = ElementBuilder("sl-breadcrumb")
    let slBreadcrumbItem = ElementBuilder("sl-breadcrumb-item")
    let slMenu = ElementBuilder("sl-menu")
    let slMenuItem = ElementBuilder("sl-menu-item")
    let slMenuLabel = ElementBuilder("sl-menu-label")
    let slDropdown = ElementBuilder("sl-dropdown")
    let slTabGroup = ElementBuilder("sl-tab-group")
    let slTab = ElementBuilder("sl-tab")
    let slTabPanel = ElementBuilder("sl-tab-panel")
    let slTree = ElementBuilder("sl-tree")
    let slTreeItem = ElementBuilder("sl-tree-item")

    // Layout
    let slCard = ElementBuilder("sl-card")
    let slDetails = ElementBuilder("sl-details")
    let slDivider = ElementBuilder("sl-divider")
    let slSplitPanel = ElementBuilder("sl-split-panel")
    let slCarousel = ElementBuilder("sl-carousel")
    let slCarouselItem = ElementBuilder("sl-carousel-item")

    // Media
    let slIcon = ElementBuilder("sl-icon")
    let slAvatar = ElementBuilder("sl-avatar")
    let slImage = ElementBuilder("sl-image-comparer")
    let slAnimatedImage = ElementBuilder("sl-animated-image")
    let slQrCode = ElementBuilder("sl-qr-code")

    // Data display
    let slBadge = ElementBuilder("sl-badge")
    let slTag = ElementBuilder("sl-tag")
    let slRelativeTime = ElementBuilder("sl-relative-time")
    let slFormatBytes = ElementBuilder("sl-format-bytes")
    let slFormatDate = ElementBuilder("sl-format-date")
    let slFormatNumber = ElementBuilder("sl-format-number")

    // Utilities
    let slVisuallyHidden = ElementBuilder("sl-visually-hidden")
    let slInclude = ElementBuilder("sl-include")
    let slMutationObserver = ElementBuilder("sl-mutation-observer")
    let slResizeObserver = ElementBuilder("sl-resize-observer")
    let slAnimation = ElementBuilder("sl-animation")
