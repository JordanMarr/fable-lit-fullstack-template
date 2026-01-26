# 📘 Fable Lit Fullstack Template  
[![NuGet version (fable-lit-fullstack-template)](https://img.shields.io/nuget/v/fable-lit-fullstack-template.svg?style=flat-square)](https://www.nuget.org/packages/fable-lit-fullstack-template/)


A modern, ergonomic starter template for building full‑stack F# applications with **Fable**, **Lit**, and **Web Components** — powered by a brand‑new, strongly‑typed UI DSL that removes the biggest pain points of traditional Lit development.

This template is designed to give you a smooth, productive experience from day one, whether you're building a small prototype or a full production app.

---

# ✨ A Quick Look: A Counter Component in F#

This is all it takes to build a reactive component with the new DSL:

```fsharp
[<HookComponent>]
let Counter() =
    let count, setCount = Hook.useState(0)

    view {
        slButton {
            onClick (fun _ -> setCount(count + 1))
            $"Clicked {count} times"
        }
    }
```

No JSX.  
No HTML strings.  
No dependency arrays.  
Just clean, strongly‑typed F#.

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

# 📄 Example: A Full Page Built with the DSL

This is what a real page looks like using the DSL.  
No HTML strings.  
No JSX.  
Just clean, composable F#.

```fsharp
module WebLit.ViewCatFactPage

open Lit
open LitRouter
open Fable.Lit.Dsl
open Fable.Lit.Dsl.Shoelace

let private hmr = HMR.createToken()

[<HookComponent>]
let Page (fact: string) =
    Hook.useHmr(hmr)

    view {
        slBreadcrumb {
            style "margin: 10px;"
            slBreadcrumbItem {
                onClick (fun _ -> Router.navigatePath("/"))
                "Home"
            }
            slBreadcrumbItem {
                onClick (fun _ -> Router.navigatePath("/cat-facts"))
                "Cat Facts"
            }
            slBreadcrumbItem {
                style "font-weight: bold;"
                "Fact"
            }
        }

        slCard {
            class' "card-overview"

            img {
                slot' "image"
                attr "src" "https://images.unsplash.com/photo-1559209172-0ff8f6d49ff7?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=500&q=80"
                attr "alt" "A kitten sits patiently between a terracotta pot and decorative grasses."
            }

            strong { "Fact" }
            br { nothing }
            div { fact }
            small { "Meow!" }

            div {
                slot' "footer"
                slButton {
                    variantPrimary
                    pill true
                    onClick (fun _ -> Router.navigatePath("/cat-facts"))
                    "Tell me more!!"
                }
            }
        }
    }
```

This example demonstrates:

- the `view` builder  
- the Shoelace DSL  
- routing  
- events  
- slots  
- attributes  
- styling  
- nested composition  

It shows how the DSL scales to real‑world UI.

---

# 🌿 Why This Template Makes Lit Even Better

Lit is already fast and modern — but writing HTML templates inside F# strings can be awkward and editor‑dependent.

This template solves that with a new DSL that gives you:

### ✔ Strongly‑typed UI  
### ✔ Beautiful nested component trees  
### ✔ No IDE extensions required  
### ✔ Easy extensibility  
### ✔ Escape hatches to raw Lit  

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

```bash
npm install
npm run dev
npm run build
```

Includes:

- full Fable + .NET debugging  
- hot module reloading  
- server + client projects  
- shared F# code  

---

# 🧩 Extending the DSL

```fsharp
let myComponent = dsl "my-component"
```

The DSL is intentionally small, composable, and easy to grow.

---

# 🛣️ Roadmap

Future NuGet packages:

- **Fable.Lit.Dsl** -- currently included in this template
- **Fable.Lit.Dsl.Shoelace** -- currently included in this template  
- **Fable.Lit.Dsl.FluentUI / FAST**  

---

# 🎉 Enjoy building with F# + Lit

This template gives you a modern, friction‑free experience building Web Components and reactive UI in F#.  
The new DSL removes years of friction and opens the door to a more expressive, maintainable style of UI development.

Have fun — and build something amazing.

---

If you want, I can help you refine the tone, add badges, or generate a small architecture diagram to include in the README.