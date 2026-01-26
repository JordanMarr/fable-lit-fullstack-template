module WebLit.App

open Lit
open LitStore
open LitRouter
open Fable.Lit.Dsl
open Fable.Lit.Dsl.Shoelace
open Ctrls.Bootstrap

Registrations.registerComponents()
Ctrls.register()

[<LitElement("my-app")>]
let MyApp() =
    let _ = LitElement.init(fun cfg -> cfg.useShadowDom <- false)
    let ctx = Hook.useStore(AppContext.store)
    let path = Hook.useRouter(RouteMode.Path)

    let isActive pathToCheck =
        match path, pathToCheck with
        | c, p when c = p -> true
        | "cat-fact" :: _, ["cat-facts"] -> true
        | _ -> false

    html {
        nav {
            navButton "Home" "house" "/" (isActive [])
            navButton "View Cat Facts" "list-ul" "/cat-facts" (isActive ["cat-facts"])
            navButton "Cat Info Form" "info-circle-fill" "/cat-info" (isActive ["cat-info"])
            navButton "DSL Demo" "code-slash" "/dsl-demo" (isActive ["dsl-demo"])
            navButton "Shoelace DSL" "brush" "/shoelace-dsl-demo" (isActive ["shoelace-dsl-demo"])

            div {
                style "float: right; padding: 10px"
                bsIcon "person-fill" "white" "18px"
                ctx.Username
            }
        }

        main {
            style "margin: 20px;"
            match path with
            | [] ->
                lit (WelcomePage.Page())
            | ["cat-facts"] ->
                lit (ListCatFactsPage.Page())
            | ["cat-fact"; fact] ->
                lit (ViewCatFactPage.Page(fact))
            | ["cat-info"] ->
                lit (CatInfoPage.Page())
            | ["dsl-demo"] ->
                lit (DslDemoPage.Page())
            | ["shoelace-dsl-demo"] ->
                lit (ShoelaceDslDemoPage.Page())
            | _ ->
                h1 { "Page not found." }
        }

        footer { nothing }

        slAlert {
            id "toaster"
            prop "duration" 3000
            closable true
        }
    }
    |> Renderer.render
