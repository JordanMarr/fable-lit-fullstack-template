module WebLit.ViewCatFactPage

open Lit
open LitRouter
open Fable.Lit.Dsl
open Fable.Lit.Dsl.Shoelace

let private hmr = HMR.createToken()

[<HookComponent>]
let Page (fact: string) =
    Hook.useHmr(hmr)

    view {
        slBreadcrumb {
            style "margin: 10px;"
            slBreadcrumbItem {
                onClick (fun _ -> Router.navigatePath("/"))
                "Home"
            }
            slBreadcrumbItem {
                onClick (fun _ -> Router.navigatePath("/cat-facts"))
                "Cat Facts"
            }
            slBreadcrumbItem {
                style "font-weight: bold;"
                "Fact"
            }
        }

        slCard {
            class' "card-overview"

            img {
                slot' "image"
                attr "src" "https://images.unsplash.com/photo-1559209172-0ff8f6d49ff7?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=500&q=80"
                attr "alt" "A kitten sits patiently between a terracotta pot and decorative grasses."
            }

            strong { "Fact" }
            br { nothing }
            div { fact }
            small { "Meow!" }

            div {
                slot' "footer"
                slButton {
                    variantPrimary
                    pill true
                    onClick (fun _ -> Router.navigatePath("/cat-facts"))
                    "Tell me more!!"
                }
            }
        }
    }
