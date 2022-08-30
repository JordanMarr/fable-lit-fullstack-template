module WebLit.App

open Elmish
open Lit
open Lit.Elmish
open LitStore
open Router

Registrations.registerComponents()
Ctrls.register()

type Model = { CurrentPath: string list }

let init () = 
    { 
        CurrentPath = Router.currentPath ()
    }, Cmd.none

type Msg = 
    | SetCurrentPath of string list

let update msg model = 
    match msg with
    | SetCurrentPath path ->
        { model with CurrentPath = path }, Cmd.none

let getRoutedPage path = 
    printfn "getRoutedPage"
    match path with
    | [ ] -> WelcomePage.Page()
    | [ "cat-fact"; fact ] -> ViewCatFactPage.Page(fact)
    | [ "cat-facts" ] -> ListCatFactsPage.Page()
    | [ "cat-info" ] -> CatInfoPage.Page()
    | _ -> html $"<h1>Page not found.</h1>"

[<LitElement("my-app")>]
let MyApp() =
    let _ = LitElement.init(fun cfg -> cfg.useShadowDom <- false)
    let model, dispatch = Hook.useElmish(init, update)
    let ctx = Hook.useStore(AppContext.store)
    
    let navLinkIsActive path = 
        match model.CurrentPath, path with
        | c, p when c = p -> "primary"
        | head1 :: _, head2 :: _ when head1 = head2 -> "primary"
        | _ -> "default"

    html $"""
        <nav>
            <sl-button href="#" @click={fun _ -> Router.navigatePath("/")} variant={navLinkIsActive []} outline>
                <bs-icon src="house" color="white" size="14px"></bs-icon>
                Home
            </sl-button>
            <sl-button href="#" @click={fun _ -> Router.navigatePath("/cat-facts")} variant={navLinkIsActive ["cat-facts"]} outline>
                <bs-icon src="list-ul" color="white" size="14px"></bs-icon>
                View Cat Facts
            </sl-button>
            <sl-button href="#" @click={fun _ -> Router.navigatePath("/cat-info")} variant={navLinkIsActive ["cat-info"]} outline>
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
                let page = getRoutedPage model.CurrentPath

                Hook.router [
                    router.pathMode
                    router.onUrlChanged (SetCurrentPath >> dispatch)
                    router.children page
                ]
                
            }
        </main>
        <footer></footer>
        """
