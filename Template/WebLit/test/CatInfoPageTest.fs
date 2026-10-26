module CatInfoPageTest

open System
open Fable.Core.JsInterop
open Browser
open Expect
open Expect.Dom
open WebTestRunner
open Lit
open Lit.Test
open Shared.Api
open WebLit.CatInfoPage

let validCat = { Name = "Whiskers"; Age = 3; LastVetCheckup = DateTime.Now }

describe "CatInfoPage" <| fun () ->
    // Test the Elmish update function directly (commands are ignored)
    it "Save with an empty cat shows validation errors" <| fun () -> promise {
        let model, _ = init ()
        let model, _ = update Save model

        model.Saved |> Expect.equal false
        model.Validation.HasErrors(nameof model.Cat.Name) |> Expect.equal true
        model.Validation.HasErrors(nameof model.Cat.Age) |> Expect.equal true
    }

    it "Save with a valid cat succeeds" <| fun () -> promise {
        let model, _ = init ()
        let model, _ = update (SetCat validCat) model
        let model, _ = update Save model

        model.Saved |> Expect.equal true
        model.Validation.IsValid() |> Expect.equal true
    }

    it "Cancel resets the form" <| fun () -> promise {
        let model, _ = init ()
        let model, _ = update (SetCat validCat) model
        let model, _ = update Cancel model

        model.Cat |> Expect.equal (fst (init ())).Cat
    }

    // Test the UI (running Elmish underneath)
    it "Submitting the empty form renders the validation summary" <| fun () -> promise {
        // Saving shows a toast, so stub out the app's toaster element
        let toaster = document.createElement "div"
        toaster.id <- "toaster"
        toaster?toast <- fun () -> ()
        document.body.appendChild toaster |> ignore

        use! container = render (html $"<div>{Page()}</div>")
        let el = container.El
        el |> Expect.error "no errors before submitting" (fun el -> el.getByText("Name is required"))

        el.querySelector("form")?requestSubmit()
        do! elementUpdated el

        el |> Expect.success "name error shown" (fun el -> el.getByText("Name is required"))
        el |> Expect.success "age error shown" (fun el -> el.getByText("Age must be a positive number."))
        toaster.remove()
    }
