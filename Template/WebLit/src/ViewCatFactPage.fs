module WebLit.ViewCatFactPage

open Shared
open Elmish
open Lit
open Lit.Elmish
open Shared.Api
open Utils

[<HookComponent>]
let Page(fact: string) = 
    html $"""
        <fluent-breadcrumb style="margin: 10px;">
            <fluent-breadcrumb-item href="#" @click={fun () -> Grapnel.navigate("/cat-facts")}>Cat Facts</fluent-breadcrumb-item>
            <fluent-breadcrumb-item style="font-weight: bold;">{fact}</fluent-breadcrumb-item>
        </fluent-breadcrumb>
        """
