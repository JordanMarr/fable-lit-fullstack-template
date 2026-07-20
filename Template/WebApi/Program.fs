module WebApi.Program

open System.IO

[<Serde.FS.EntryPoint>]
let main args =
    let contentRoot = Directory.GetCurrentDirectory()

    let app =
        WebApp.build
            { Args = args
              Environment = None
              ContentRoot = contentRoot
              WebRoot = Path.Combine(contentRoot, "wwwroot")
              Urls = []
              UseHttpsRedirection = true
              CorsOrigins = [ "https://localhost:3000" ]
              Root =
#if DEBUG
                WebApp.RedirectToDevServer "https://localhost:3000"
#else
                WebApp.SpaFallback
#endif
            }

    app.Run()
    0
