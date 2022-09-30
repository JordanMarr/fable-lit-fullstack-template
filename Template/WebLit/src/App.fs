module WebLit.App

open Elmish
open Lit
open Lit.Elmish
open LitStore
open LitRouter

Registrations.registerComponents()
Ctrls.register()

[<LitElement("my-app")>]
let MyApp() =
    let _ = LitElement.init(fun cfg -> cfg.useShadowDom <- false)
    let ctx = Hook.useStore(AppContext.store)
    let path = Hook.useRouter(RouteMode.Path)
    
    let navLinkIsActive pathToCheck = 
        match path, pathToCheck with
        | c, p when c = p -> "primary"
        | "cat-fact" :: _, "cat-facts" :: _ -> "primary"
        | _ -> "default"

    html $"""
        <nav>
            <sl-button @click={fun _ -> Router.navigatePath("/")} variant={navLinkIsActive []} outline>
                <bs-icon src="house" color="white" size="14px"></bs-icon>
                Home
            </sl-button>
            <sl-button @click={fun _ -> Router.navigatePath("/cat-facts")} variant={navLinkIsActive ["cat-facts"]} outline>
                <bs-icon src="list-ul" color="white" size="14px"></bs-icon>
                View Cat Facts
            </sl-button>
            <sl-button @click={fun _ -> Router.navigatePath("/cat-info")} variant={navLinkIsActive ["cat-info"]} outline>
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
                match path with
                | [ ] ->                    WelcomePage.Page()
                | [ "cat-facts" ] ->        ListCatFactsPage.Page()
                | [ "cat-fact"; fact ] ->   ViewCatFactPage.Page(fact)
                | [ "cat-info" ] ->         CatInfoPage.Page()
                | _ ->                      html $"<h1>Page not found.</h1>"
            }
        </main>
        <footer></footer>

        <sl-alert id="toaster" duration="3000" closable></sl-alert>
        """
