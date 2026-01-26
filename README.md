# 📘 Fable Lit Fullstack Template  
NuGet version [(nuget.org in Bing)](https://www.bing.com/search?q="https%3A%2F%2Fwww.nuget.org%2Fpackages%2Ffable-lit-fullstack-template%2F")

A modern, ergonomic starter template for building full‑stack F# applications with **Fable**, **Lit**, and **Web Components** — powered by a brand‑new, strongly‑typed UI DSL that removes the biggest pain points of traditional Lit development.

This template is designed to give you a smooth, productive experience from day one, whether you're building a small prototype or a full production app.

---

# 🚀 Getting Started

Install the template globally:

```bash
dotnet new -i fable-lit-fullstack-template
```

Create a new project:

```bash
dotnet new fable-lit-fullstack -n MyApp
```

Run it:

```bash
cd MyApp
npm install
npm run dev
```

You now have a full F# + Fable + Lit application running with the new DSL baked in.

---

# ⚡ Why Lit Instead of React?

Lit is a modern, lightweight alternative to React — built directly on **native Web Components**, not a custom runtime.  
If you’re coming from React, you’ll find Lit refreshingly simple, fast, and future‑proof.

### 🚀 1. No Virtual DOM — real DOM updates, surgically applied  
React re-renders entire component trees and relies on a virtual DOM diffing algorithm to figure out what changed.  
Lit updates only the exact DOM nodes that need to change, using real browser APIs.

It’s faster because it’s simpler.

### 📦 2. Tiny bundle sizes  
React + ReactDOM is ~120kb minified.  
Lit is ~6kb.

Smaller bundles mean:

- faster startup  
- better Lighthouse scores  
- better mobile performance  

### 🌐 3. Web‑native, framework‑agnostic components  
Lit components are **Web Components**, which means they work everywhere:

- React  
- Vue  
- Svelte  
- Angular  
- plain HTML  
- server‑rendered apps  
- microfrontends  

React components only work in React.

### 🧩 4. No build‑time magic  
Lit uses:

- real JavaScript classes  
- real DOM APIs  
- real browser standards  

No JSX transform.  
No custom compiler.  
No runtime wrappers.

### 🛠️ 5. Better long‑term stability  
Web Components are part of the platform.  
They don’t get rewritten every 18 months.

### 💙 6. A perfect fit for F#  
Lit’s declarative, composable model maps beautifully to:

- computation expressions  
- immutable data  
- functional UI patterns  
- strongly‑typed DSLs  

React’s JSX does not.

### 🧠 7. No memoization, no dependency arrays, no stale closures  
React forces you to think about:

- `useMemo`  
- `useCallback`  
- dependency arrays  
- stale closures  
- preventing unnecessary re-renders  

Lit updates only the DOM nodes that change.  
F# encourages immutable data by default.

The result:

- no memoization  
- no dependency arrays  
- no re-render storms  
- no performance footguns  

Just clean, predictable updates.

---

# 💡 All the Good Parts of React — Without the Pain

If you enjoy React’s component model, you’ll feel right at home here.

This template supports:

### ✔ Simple, React‑style hooks  
Use `useState`, `useEffect`, and other familiar patterns — without dependency arrays or stale closures.

### ✔ Full Elmish when you want structure  
Switch to Elmish for larger components or full applications.  
You get predictable state, pure updates, and no hook rules.

### ✔ Declarative UI without JSX  
Write clean, strongly‑typed F# instead of JSX or HTML strings.  
No editor extensions required.

### ✔ Faster than React by design  
No virtual DOM.  
No diffing.  
No reconciliation.  
Just direct, surgical DOM updates.

---

# 🌿 Why This Template Makes Lit Even Better

Lit is already fast and modern — but writing HTML templates inside F# strings can be awkward and editor‑dependent.

This template solves that with a new DSL that gives you:

### ✔ Strongly‑typed UI  
No more stringly‑typed HTML.  
You get compile‑time checking, autocomplete, and predictable structure.

### ✔ Beautifully nested component trees  
Instead of this:

```fsharp
html $"""
<sl-card>
  <sl-button>Click me</sl-button>
</sl-card>
"""
```

You write this:

```fsharp
slCard {
    slButton { "Click me" }
}
```

Readable. Maintainable. Idiomatic.

### ✔ No IDE extensions required  
Works perfectly in:

- VS Code  
- Ionide  
- Rider  
- Visual Studio  

No HTML colorizer needed.  
No broken syntax highlighting.  
No friction.

### ✔ Easy to extend  
The DSL is modular and library‑agnostic.  
You can build DSLs for:

- Shoelace  
- FluentUI / FAST  
- Material Web  
- your company’s design system  

### ✔ Escape hatches included  
You can always drop back to raw Lit:

```fsharp
html $"""<div>Hello</div>"""
```

Or use the `el` helper for arbitrary elements.

---

# 📁 Template Structure

This template intentionally mixes two approaches:

### **1. DSL‑based pages (recommended)**  
Most pages use the new DSL for clarity and ergonomics.

### **2. A single raw Lit page (WelcomePage)**  
This page demonstrates:

- plain `html` interpolation  
- the `el` helper  
- FluentUI components  
- how to integrate third‑party Web Components manually  

This keeps the educational value without forcing users to rely on editor extensions.

---

# 🎨 Shoelace Integration

Shoelace components are registered using a simple, reliable system based on `[<Literal>]` asset paths:

```fsharp
importDynamic Shoelace.Asset.Button
importDynamic Shoelace.Asset.Dialog
importDynamic Shoelace.Asset.DarkTheme
```

No abstraction layers.  
No magic.  
Just clean, explicit imports.

---

# ▶️ Running the Template

### Install dependencies

```bash
npm install
```

### Run the dev server

```bash
npm run dev
```

### Build for production

```bash
npm run build
```

### Debugging

The template includes:

- full Fable + .NET debugging support  
- hot module reloading  
- server + client projects  
- shared F# code  

Everything works out of the box.

---

# 🧩 Extending the DSL

Adding a new component is as simple as:

```fsharp
let myComponent attrs children =
    el "my-component" attrs children
```

Or, for a nicer builder:

```fsharp
let myComponent = dsl "my-component"
```

The DSL is intentionally small, composable, and easy to grow.

---

# 🛣️ Roadmap

The DSLs included in this template are currently shipped inline for rapid iteration and transparency.  
Once the APIs stabilize, they will be published as official NuGet packages.

### Planned NuGet Packages

#### 📦 Fable.Lit.Dsl  
The core DSL for strongly‑typed UI composition  -- currently included in the template.

#### 📦 Fable.Lit.Dsl.Shoelace  
A first‑class DSL for Shoelace Web Components -- currently included in the template.

#### 📦 Fable.Lit.Dsl.FluentUI or Fable.Lit.Dsl.FAST  
A future DSL for Microsoft’s Fluent UI / FAST Web Components.

These packages will be extracted once the APIs feel “obvious in hindsight” and have been validated through real‑world usage.

---

# 🤝 Contributing

Feedback, issues, and PRs are welcome — especially around:

- DSL ergonomics  
- Shoelace component coverage  
- FluentUI/FAST integration  
- documentation improvements  
- real‑world usage patterns  

This template is meant to grow with the community.

---

# 🎉 Enjoy building with F# + Lit

This template is designed to give you a modern, friction‑free experience building Web Components and reactive UI in F#.  
The new DSL removes the biggest historical pain points and opens the door to a much more expressive, maintainable style of UI development.

Have fun — and build something amazing.

---
