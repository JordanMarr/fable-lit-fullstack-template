module WebLit.WelcomePage

open Shared
open Elmish
open Lit
open Lit.Elmish
open Shared.Api

[<HookComponent>]
let Page() = 
    html $"""
        <h1>Welcome to Cat Facts!</h1>        
        """
