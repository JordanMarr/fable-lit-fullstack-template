module Desktop.Program

open System
open System.Drawing
open System.IO
open Microsoft.Extensions.Hosting
open Photino.NET
open WebApi

[<EntryPoint; STAThread>]
let main args =
    let contentRoot = AppContext.BaseDirectory

#if DEBUG
    // Dev: run `npm run start` in WebLit first (Vite on https://localhost:3000).
    // Kestrel runs in-process on the standard dev port so the Vite proxy can reach it, and the
    // window points at Vite for full Fable HMR inside the native shell.
    // (Don't run the WebApi project at the same time - it uses the same port.)
    let app =
        WebApp.build
            { Args = args
              Environment = Some "Development"
              ContentRoot = contentRoot
              WebRoot = Path.Combine(contentRoot, "wwwroot")
              Urls = [ "https://localhost:5001" ]
              UseHttpsRedirection = false
              CorsOrigins = []
              Root = WebApp.RedirectToDevServer "https://localhost:3000" }

    app.Start()
    let windowUrl = "https://localhost:3000"
#else
    // Prod: serve the built client from wwwroot next to the exe on a dynamic loopback port.
    let app =
        WebApp.build
            { Args = args
              Environment = None
              ContentRoot = contentRoot
              WebRoot = Path.Combine(contentRoot, "wwwroot")
              Urls = [ "http://127.0.0.1:0" ] // port 0 = pick a free port
              UseHttpsRedirection = false
              CorsOrigins = []
              Root = WebApp.SpaFallback }

    app.Start()
    // After Start(), app.Urls reflects the actual bound address (resolved port).
    let windowUrl = app.Urls |> Seq.head
#endif

    let window =
        PhotinoWindow()
            .SetTitle("Fullstack")
            .SetUseOsDefaultSize(false)
            .SetSize(Size(1200, 800))
            .Center()
#if DEBUG
            .SetDevToolsEnabled(true)
#endif
            .Load(windowUrl)

    // Photino runs its message loop on the main thread until the window closes.
    window.WaitForClose()
    app.StopAsync().GetAwaiter().GetResult()
    0
