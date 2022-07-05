# fable-lit-fullstack-template
A SAFE-style template with Fable.Lit, Fable.Remoting and Giraffe

Based on:
https://github.com/Zaid-Ajaj/SAFE.Simplified (thank you Zaid!)

## Features

### WebLit.fsproj (Client)
* Fable.Remoting
* Bindings for `Grapnel` Router
* `Shoelace` and `FluentUI` web components imported (cherry-picked)
* A minimal `vite.config.js` file that configures https proxy server + a common proxy redirects
* `"vite-plugin-mkcert` plugin installed for https support for proxy server
* Bootstrap icons + a `bs-icon` custom element control.

### WebApi.fsproj (Server)
* Giraffe
* Fable.Remoting + custom error handler
* A very simple REST module
* Environment specific settings files already configured
* Serilog logger

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


