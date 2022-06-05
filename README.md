# fable-lit-fullstack-template
A SAFE-style template with Fable.Lit, Fable.Remoting and Giraffe

Based on:
https://github.com/Zaid-Ajaj/SAFE.Simplified

## Features

### WebLit.fsproj (Client)
* Fable.Remoting
* Bindings for `Grapnel` Router
* `Shoelace` and `FluentUI` web components imported (cherry-picked)
* A minimal `vite.config.js` file that configures https proxy server + a common proxy redirects
* `"vite-plugin-mkcert` plugin installed for https support for proxy server

### WebApi.fsproj (Server)
* Giraffe
* Fable.Remoting + custom error handler
* A very simple REST module
* Environment specific settings files already configured
* Serilog logger

### Build.fsproj (FAKE)
* Zaid's awesome FAKE build setup!

## Notes
Be sure to install the appropriate IDE extension for html and css syntax coloring within your `html $""" """` templates!
If using VS Code:
* [Highlight HTML/SQL Templates in F#](https://marketplace.visualstudio.com/items?itemName=alfonsogarciacaro.vscode-template-fsharp-highlight)

If using Visual Studio:
* [Html for F# (Lit Template)](https://marketplace.visualstudio.com/items?itemName=daniel-hardt.html-for-fsharp-lit-template)

Currently, VS Code with the "Highlight HTML/SQL Templates in F#" extension provides the best experience because it actually provides contextual IntelliSense for the HTML and CSS, plus you can use all the other amazing HTML extensions.


