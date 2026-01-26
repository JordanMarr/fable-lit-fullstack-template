module WebLit.WelcomePage

open Lit
open LitStore
open Fable.Lit.Dsl
open Fable.Lit.Dsl.Shoelace

[<HookComponent>]
let Page() = 
    let ctx = Hook.useStore(AppContext.store)
    
    view {
        h1 { "Welcome to Cat Facts!" }

        slInput {
            style "width: 300px;"
            label' "Please enter a username:"
            value ctx.Username
            on "sl-change" (fun e -> AppContext.dispatch (AppContext.SetUsername e.target.Value))
        }
    }