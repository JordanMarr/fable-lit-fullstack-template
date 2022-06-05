module WebLit.App

open Lit
open Utils

Registrations.registerComponents()
let router = Grapnel.initRouter()

type Page = 
    | Welcome
    | ListCatFacts
    | ViewCatFact of fact: string

[<LitElement("my-app")>]
let MyApp() =
    let _ = LitElement.init(fun cfg -> cfg.useShadowDom <- false)
    let currentPage, setCurrentPage = Hook.useState Welcome
    
    let navLinkIsActive page = 
        match currentPage, page with
        | ViewCatFact _, ListCatFacts -> "primary"
        | c, p when c = p -> "primary"
        | _ -> "default"

    Hook.useEffectOnce(fun () -> 
        router.get("/", fun _ -> setCurrentPage Welcome)
        router.get("/cat-facts", fun _ -> setCurrentPage ListCatFacts)
        router.get("/cat-fact/:fact", fun (req: Req<{| fact: string |}>) -> setCurrentPage (Page.ViewCatFact req.``params``.fact))
    )
    
    html $"""
        <nav>
            <sl-button href="#" @click={fun _ -> router.navigate("/")} variant={navLinkIsActive Welcome} outline>
                <bs-icon src="house" color="white" size="14px"></bs-icon>
                Home
            </sl-button>
            <sl-button href="#" @click={fun _ -> router.navigate("/cat-facts")} variant={navLinkIsActive ListCatFacts} outline>
                <bs-icon src="list-ul" color="white" size="14px"></bs-icon>
                View Cat Facts
            </sl-button>
        </nav>
        <main style="margin: 20px;">
            {
                match currentPage with
                | Welcome -> WelcomePage.Page()
                | ListCatFacts -> ListCatFactsPage.Page()
                | ViewCatFact fact -> ViewCatFactPage.Page(fact)
            }
        </main>
        """
