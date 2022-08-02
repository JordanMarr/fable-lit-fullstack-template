module WebLit.App

open Elmish
open Lit
open Lit.Elmish
open UseStore

Registrations.registerComponents()
let router = Grapnel.initRouter()

type Page = 
    | Welcome
    | ListCatFacts
    | ViewCatFact of fact: string
    | CatInfo

type Model = { CurrentPage: Page }

let init () = 
    { 
        CurrentPage = Welcome 
    }, Cmd.none

type Msg = 
    | SetCurrentPage of Page

let update msg model = 
    match msg with
    | SetCurrentPage page ->
        { model with CurrentPage = page }, Cmd.none


[<LitElement("my-app")>]
let MyApp() =
    let _ = LitElement.init(fun cfg -> cfg.useShadowDom <- false)
    let model, dispatch = Hook.useElmish(init, update)
    let ctx = Hook.useStore(AppContext.store)
    
    let navLinkIsActive page = 
        match model.CurrentPage, page with
        | ViewCatFact _, ListCatFacts -> "primary"
        | c, p when c = p -> "primary"
        | _ -> "default"

    Hook.useEffectOnce(fun () -> 
        router.get("/", fun _ -> dispatch (SetCurrentPage Welcome))
        router.get("/cat-facts", fun _ -> dispatch (SetCurrentPage ListCatFacts))
        router.get("/cat-fact/:fact", fun (req: Req<{| fact: string |}>) -> 
            dispatch (SetCurrentPage (Page.ViewCatFact req.``params``.fact))
        )
        router.get("/cat-info", fun _ -> dispatch (SetCurrentPage CatInfo))
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
            <sl-button href="#" @click={fun _ -> router.navigate("/cat-info")} variant={navLinkIsActive CatInfo} outline>
                <bs-icon src="info-circle-fill" color="white" size="14px"></bs-icon>
                Cat Info Form
            </sl-button>
            
            <div style="float: right; padding: 10px">  
                <bs-icon src="person-fill" color="white" size="18px"></bs-icon>
                {ctx.Username}
            </div>
        </nav>
        <main style="margin: 20px;">
            {
                match model.CurrentPage with
                | Welcome -> WelcomePage.Page()
                | ListCatFacts -> ListCatFactsPage.Page()
                | ViewCatFact fact -> ViewCatFactPage.Page(fact)
                | CatInfo -> CatInfoPage.Page()
            }
        </main>
        <footer></footer>
        """
