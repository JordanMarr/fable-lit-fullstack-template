﻿module WebLit.WelcomePage

open Lit
open LitStore

[<HookComponent>]
let Page() = 
    let ctx = Hook.useStore(AppContext.store)
    
    html $"""
        <h1>Welcome to Cat Facts!</h1>
        <sl-input 
            style="width: 300px;"
            label="Please enter a username:"
            value={ctx.Username} 
            @sl-change={Ev (fun e -> AppContext.dispatch (AppContext.SetUsername e.target.Value))}>
        </sl-input>
        """
