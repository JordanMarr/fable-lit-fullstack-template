# fable-lit-fullstack-template [![NuGet version (fable-lit-fullstack-template)](https://img.shields.io/nuget/v/fable-lit-fullstack-template.svg?style=flat-square)](https://www.nuget.org/packages/fable-lit-fullstack-template/)

A SAFE-style template with Fable.Lit, Fable.Remoting and Giraffe

Based on:
https://github.com/Zaid-Ajaj/SAFE.Simplified (thank you Zaid!)

## Features

### WebLit.fsproj (Client)
* RPC to WebApi via [Fable.Remoting](https://zaid-ajaj.github.io/Fable.Remoting/#/)
* Page routing via [Fable.LitRouter](https://github.com/JordanMarr/Fable.LitRouter)
* Shared context via [Fable.LitStore](https://www.nuget.org/packages/Fable.LitStore)
* `Shoelace` and `FluentUI` web components imported (cherry-picked)
* A minimal `vite.config.js` file that configures https proxy server + a common proxy redirects
* `"vite-plugin-mkcert` plugin installed for https support for proxy server
* Bootstrap icons + a `bs-icon` custom element control.
* Toast notifications
* Form Validation (rules lives with entities in Shared.fs)

### WebApi.fsproj (Server)
* Giraffe
* Fable.Remoting + custom error handler
* A very simple REST module
* Environment specific settings files already configured
* Serilog logger
* Entity Validation (rules live with entities in Shared.fs)


## Install Template [![NuGet version (fable-lit-fullstack-template)](https://img.shields.io/nuget/v/fable-lit-fullstack-template.svg?style=flat-square)](https://www.nuget.org/packages/fable-lit-fullstack-template/)

```cmd
dotnet new install fable-lit-fullstack-template
```

## Use Template
This will create a new subfolder, `MyLitApp`, which will contain a `MyLitApp.sln`:

```cmd
dotnet new flft -o MyLitApp
```


## Build

### Initial Restore
To do the initial restore of both the WebApi and WebLit projects:
* :open_file_folder: Build: `dotnet run Restore`

Or you can manually restore each:
* :open_file_folder: WebApi: `dotnet restore`
* :open_file_folder: WebLit: `npm install`

### Run in Debug Mode
* :open_file_folder: WebApi: `dotnet watch`
* :open_file_folder: WebLit: `npm start`

### Pack in Release Mode
To build WebApi and WebLit in Release mode and output to the `Template/dist` folder:
* :open_file_folder: Build: `dotnet run Pack`
or
* :open_file_folder: Build: `dotnet run PackNoTests`

## Highlight Extension
Be sure to install the appropriate IDE extension for html and css syntax coloring within your `html $""" """` templates!

If using VS Code:
* [Highlight HTML/SQL Templates in F#](https://marketplace.visualstudio.com/items?itemName=alfonsogarciacaro.vscode-template-fsharp-highlight)

If using Visual Studio:
* [Html for F# (Lit Template)](https://marketplace.visualstudio.com/items?itemName=daniel-hardt.html-for-fsharp-lit-template)

Currently, VS Code with the "Highlight HTML/SQL Templates in F#" extension provides the best experience because it actually provides contextual IntelliSense for the HTML and CSS, plus you can use all the other amazing HTML extensions.


## Toast Module

![image](https://user-images.githubusercontent.com/1030435/193339122-fdf130d7-ed00-4f18-92e2-a87cba44d0ef.png)

You can create toast messages in two ways:

1) Call a `Toast` function directly:
```F#
module WebLit.WelcomePage

open Lit
open Ctrls

[<HookComponent>]
let Render() = 
    
    let sayHello() = 
        Toast.info "Hello, world!"

    html $"""
        <sl-button 
            @click={Ev (fun e -> sayHello())}>
            Say Hello
        </sl-button>
    """
```

2) Return a `Toast` `Cmd` (if using Elmish):
```F#
let update (msg: Msg) (model: Model) =
    match msg with
    | Save -> 
        model, Cmd.OfAsync.either Server.api.SaveProjectFiles model.Files SaveCompleted OnError
    | SaveCompleted _ -> 
        model, Toast.Cmd.success "Files saved."
    | OnError ex ->
        model, Toast.Cmd.error ex.Message
```

## Validation
The `Validation.fs` module lives in the Shared.fs project and contains functions for creating validation rules.

Usage:

### Shared.fs
1) Create a custom validation method (or function) alongside your entity in the Shared.fs project:

```F#
type CatInfo = 
    {
        Name: string
        Age: int
        LastVetCheckup: System.DateTime
    }
    member this.Validate() = 
        rules
        |> rulesFor (nameof this.Name) [ 
            this.Name |> Rules.required
            this.Name |> Rules.maxLen 10
        ]
        |> rulesFor (nameof this.Age) [
            Rules.isTrue (this.Age > 0) "Age must be a positive number."
        ]
        |> rulesFor (nameof this.LastVetCheckup) [
            // A custom rule
            let timeSinceLastVetCheckup = System.DateTime.Today - this.LastVetCheckup.Date
            printfn $"Total days since last checkup: {timeSinceLastVetCheckup.TotalDays}"
            if this.Age >= 10 && timeSinceLastVetCheckup.TotalDays > 90 then 
                Error "Cats over 10 years old should get a vet checkup every three months."
            elif timeSinceLastVetCheckup.TotalDays > 180 then 
                Error "Cats under 10 years old should get a vet checkup every six months."
            else 
                Ok ()
        ]
        |> validate
```

### WebLit.fs
2) In your WebLit.fs UI / form, track the entity state in your model using the `ValidationResult`:

```F#
type Model = 
    {
        Cat: CatInfo
        Validation: ValidationResult
        Saved: bool
    }
    
let init () = 
    { 
        Cat = 
            { CatInfo.Name = ""
            ; CatInfo.Age = 0
            ; CatInfo.LastVetCheckup = System.DateTime.MinValue }
        Validation = noErrors
        Saved = false
    }, Cmd.none
```

3) In the Elmish `update` function, update the `Validation` state by calling the custom `Validate` method when saving:
```F#
let update msg model = 
    match msg with
    | Save -> 
        let validation = model.Cat.Validate()
        { model with
            Validation = validation
            Saved = validation.HasErrors() = false
        }, Toast.Cmd.success "Changes saved."
```

4) In the form, set the `invalid` attributes of your inputs by checking the `model.Validation` state property for the given property:
```F#
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
```

5) At the top of the form, display the validation errors using the `Ctrls.ValidationSummary`:
```F#
<div>
    {ValidationSummary(model.Validation)}
</div>
```

### WebApi.fs
6) The validation rules may also be reused on the server side:
```F#
let saveCatInfo(catInfo: CatInfo) = 
    match catInfo.Validate().IsValid() with
    | true -> // save
    | false -> // reject
```
