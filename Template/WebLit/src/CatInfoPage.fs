module WebLit.CatInfoPage

open Shared
open Shared.Api
open Shared.Validation
open Fable.Core.JsInterop
open Elmish
open Lit
open Lit.Elmish
open Ctrls
open Router

type Model = 
    {
        Cat: CatInfo
        Validation: ValidationResult
        Saved: bool
    }

type Msg = 
    | SetCat of CatInfo
    | Save
    | Cancel

let init () = 
    { 
        Cat = 
            { CatInfo.Name = ""
            ; CatInfo.Age = 0
            ; CatInfo.LastVetCheckup = System.DateTime(2022,1,1) }
        Validation = noErrors
        Saved = false
    }, Cmd.none

let update msg model = 
    match msg with
    | SetCat cat -> 
        printfn $"Last Checkup: %A{cat.LastVetCheckup}"
        { model with Cat = cat }, Cmd.none
    | Save -> 
        let validation = model.Cat.Validate()
        { model with
            Validation = validation
            Saved = validation.HasErrors() = false
        }, Cmd.none
    | Cancel -> 
        init ()

[<HookComponent>]
let Page() = 
    let model, dispatch = Hook.useElmish(init, update)

    html $"""
        <sl-breadcrumb style="margin: 10px;">
            <sl-breadcrumb-item @click={fun () -> Router.navigatePath("/")}>Home</sl-breadcrumb-item>
            <sl-breadcrumb-item>Cat Info</sl-breadcrumb-item>
        </sl-breadcrumb>

        <div id="cat-info-page">
            {ValidationSummary(model.Validation)}

            <div class="{hideIf (not model.Saved)}">
                <span><bs-icon src="check-circle-fill" color="green" size="16px" /></span>
                <span style="display: inline-block; color: green; line-height: 80px;">The form is valid!</span>
            </div>

            <form                 
                @submit={Ev (fun e -> e.preventDefault(); dispatch Save)}
                style="width: 300px; margin: 20px;">

                <vert-stack gap="15px">

                    <sl-input 
                        label="Cat Name" 
                        .value={model.Cat.Name}
                        .invalid={model.Validation.HasErrors(nameof model.Cat.Name)}
                        @sl-change={Ev (fun e -> SetCat { model.Cat with Name = e.target.Value } |> dispatch)}>
                    </sl-input>

                    <sl-input 
                        label="Age" 
                        type="number"
                        .invalid={model.Validation.HasErrors(nameof model.Cat.Age)}
                        .value={model.Cat.Age}
                        @sl-change={Ev (fun e -> SetCat { model.Cat with Age = e.target?valueAsNumber } |> dispatch)}>
                    </sl-input>

                    <sl-input 
                        label="Last Vet Checkup" 
                        type="date"                        
                        .invalid={model.Validation.HasErrors(nameof model.Cat.LastVetCheckup)}
                        .value={model.Cat.LastVetCheckup.ToString("yyyy-MM-dd")}
                        @sl-change={Ev (fun e -> 
                            let date = System.DateTime.Parse(e.target.Value)
                            SetCat { model.Cat with LastVetCheckup = date } |> dispatch
                        )}>
                    </sl-input>

                    <horiz-stack style="width: 260px" gap="4px">
                        <sl-button type="submit" style="float: right; width: 120px;" variant="primary">Save</sl-button>
                        <sl-button style="float: right; width: 120px;" variant="default" @click={Ev (fun e -> dispatch Cancel)}>Cancel</sl-button>                    
                    </horiz-stack>

                </vert-stack>
            </form>
        </div>
        """