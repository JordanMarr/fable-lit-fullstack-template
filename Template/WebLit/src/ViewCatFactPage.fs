module WebLit.ViewCatFactPage

open Shared
open Elmish
open Lit
open Lit.Elmish
open Shared.Api
open Utils

[<HookComponent>]
let Page (fact: string) =
    html
        $"""
        <sl-breadcrumb style="margin: 10px;">
            <sl-breadcrumb-item href="#" @click={fun () -> Grapnel.navigate ("/cat-facts")}>Cat Facts</sl-breadcrumb-item>
            <sl-breadcrumb-item style="font-weight: bold;">Fact</sl-breadcrumb-item>
        </sl-breadcrumb>

        <sl-card class="card-overview">
            <img
                slot="image"
                src="https://images.unsplash.com/photo-1559209172-0ff8f6d49ff7?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=500&q=80"
                alt="A kitten sits patiently between a terracotta pot and decorative grasses." />

          <strong>Fact</strong><br />
          <div>{fact}</div>
          <small>Meow!</small>

          <div slot="footer">
            <sl-button variant="primary" pill>More Info</sl-button>
            
          </div>
        </sl-card>
        """
