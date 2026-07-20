# Recipe: Hosting the Fable Web UI Inside AutoCAD / Revit Add-ins

The template's `Desktop` project shows the core pattern: run the ASP.NET Core server **in-process** and point a native WebView at it. The same pattern works inside host applications like AutoCAD and Revit, giving you a hot-reloadable Fable.Lit UI in a palette or dockable pane — with the same Serde.FS typed RPC layer talking to the CAD API.

> **Requirements:** AutoCAD 2025+ / Revit 2025+ (the first releases on modern .NET — ASP.NET Core cannot load into the .NET Framework 4.8 process of earlier versions). For older versions, host the server out-of-process instead (e.g. a Windows Service).

## The architecture

```
acad.exe / revit.exe
 └─ Your add-in DLL
     ├─ Kestrel (in-process, http://127.0.0.1:<dynamic port>)
     │   └─ WebApp.build (same server code as the web/desktop hosts)
     │        └─ ServerApi RPC handlers ──► dispatcher bridge ──► CAD API (main thread)
     └─ WebView2 control (in a PaletteSet / DockablePane)
          └─ loads http://127.0.0.1:<port>  (the Fable.Lit client)
```

The client, the RPC contracts, and the server wiring are all unchanged. Only two pieces are add-in specific:

1. **Lifecycle** — start/stop Kestrel from the add-in's entry points.
2. **Thread marshaling** — RPC handlers run on Kestrel thread-pool threads, but CAD APIs are only legal on the host's main/API context. A small dispatcher bridge connects the two.

## 1. Starting the server from the add-in

Start Kestrel from the add-in lifecycle hook — `IExtensionApplication.Initialize` (AutoCAD) or `IExternalApplication.OnStartup` (Revit). As of Serde.FS `1.0.0-beta.4`, `MapRpcApi` self-initializes the generated codec/RPC registrations, so no bootstrap ceremony is needed even though the add-in is not the process entry point. (If you ever need registrations before the server starts — e.g. standalone `SerdeJson.serialize` calls — run `Serde.FS.Bootstrap.Run()` explicitly.)

```fsharp
open Microsoft.Extensions.Hosting
open WebApi

let mutable private app : Microsoft.AspNetCore.Builder.WebApplication option = None

let startServer (addinDir: string) =
    let webApp =
        WebApp.build
            { Args = [||]
              Environment = None
              ContentRoot = addinDir
              WebRoot = Path.Combine(addinDir, "wwwroot")
              Urls = [ "http://127.0.0.1:0" ]   // dynamic port: no conflicts with other add-ins
              UseHttpsRedirection = false
              CorsOrigins = []
              Root = WebApp.SpaFallback }
    webApp.Start()
    app <- Some webApp
    webApp.Urls |> Seq.head   // pass this URL to the WebView

let stopServer () =
    app |> Option.iter (fun a -> a.StopAsync().GetAwaiter().GetResult())
```

Call `stopServer()` from `IExtensionApplication.Terminate` / `IExternalApplication.OnShutdown`.

Ship the client by copying `WebLit/dist` into your add-in bundle as `wwwroot` (see the `PackDesktop` target in `Build/Program.fs` for the copy step to imitate), along with the `appsettings*.json` files.

## 2. The thread-marshaling problem

Kestrel serves RPC requests on thread-pool threads. Both CAD APIs will crash or corrupt state if touched from those threads:

- **Revit**: API calls are only legal inside a Revit API context (an event callback on Revit's main thread).
- **AutoCAD**: most API calls must run in the application/document context.

The fix is a dispatcher bridge: the RPC handler posts work onto a queue, wakes the host's API context, and awaits the result via `TaskCompletionSource`.

### Revit: `ExternalEvent` bridge

```fsharp
open System.Collections.Concurrent
open System.Threading.Tasks
open Autodesk.Revit.UI

type private WorkItem = { Run: UIApplication -> unit }

type RevitDispatcher private (event: ExternalEvent, queue: ConcurrentQueue<WorkItem>) =

    /// Call once from OnStartup (must be created in a valid Revit API context).
    static member Create() =
        let queue = ConcurrentQueue<WorkItem>()
        let handler =
            { new IExternalEventHandler with
                member _.GetName() = "RevitDispatcher"
                member _.Execute(uiApp) =
                    let mutable item = Unchecked.defaultof<WorkItem>
                    while queue.TryDequeue(&item) do
                        item.Run uiApp }
        RevitDispatcher(ExternalEvent.Create(handler), queue)

    /// Run f in the Revit API context and await its result from any thread.
    member _.Run (f: UIApplication -> 'T) : Async<'T> =
        let tcs = TaskCompletionSource<'T>()
        queue.Enqueue
            { Run = fun uiApp ->
                try tcs.SetResult(f uiApp)
                with ex -> tcs.SetException ex }
        event.Raise() |> ignore
        tcs.Task |> Async.AwaitTask
```

### AutoCAD: `ExecuteInApplicationContext` bridge

```fsharp
open System.Threading.Tasks
open Autodesk.AutoCAD.ApplicationServices.Core

module AcadDispatcher =

    /// Run f in the AutoCAD application context and await its result from any thread.
    let run (f: unit -> 'T) : Async<'T> =
        let tcs = TaskCompletionSource<'T>()
        Application.DocumentManager.ExecuteInApplicationContext(
            (fun _ ->
                try tcs.SetResult(f ())
                with ex -> tcs.SetException ex),
            null)
        tcs.Task |> Async.AwaitTask
```

### Using it from an RPC handler

Your `IServerApi` implementation takes the dispatcher as a constructor dependency (register it as a singleton in a customized `WebApp.build`, or pass it into `ServerApi` directly) and never touches the CAD API on the request thread:

```fsharp
type ServerApi(dispatcher: RevitDispatcher) =
    interface IServerApi with
        member _.GetSelectedElementIds() =
            dispatcher.Run(fun uiApp ->
                uiApp.ActiveUIDocument.Selection.GetElementIds()
                |> Seq.map (fun id -> id.Value)
                |> List.ofSeq)
```

From the Fable client this is just another typed RPC call — the client neither knows nor cares that the answer came off Revit's main thread.

## 3. The WebView2 palette

Autodesk palettes host WPF content, so wrap a WebView2 control in a `UserControl` and place it in an AutoCAD `PaletteSet` or Revit `DockablePane`.

**Critical gotcha:** WebView2 defaults its user-data folder to a directory next to the host executable — which is `acad.exe`/`revit.exe` in `Program Files`, a read-only location. Initialization fails silently. Always set an explicit, writable `UserDataFolder`:

```fsharp
let initWebView (webView: Microsoft.Web.WebView2.Wpf.WebView2) (serverUrl: string) =
    task {
        let userDataFolder =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MyAddin", "WebView2")
        let! env = Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(null, userDataFolder)
        do! webView.EnsureCoreWebView2Async(env)
        webView.CoreWebView2.Navigate(serverUrl)
    }
```

## 4. Dev-time hot reload inside the CAD host

Same trick as the `Desktop` project: in a debug build, run `npm run start` in `WebLit`, start the in-process Kestrel on `https://localhost:5001`, and navigate the WebView to `https://localhost:3000` instead of the local server URL. You can now edit Fable view code and watch the palette update inside AutoCAD/Revit without restarting the host — easily worth the setup.

## Other things worth knowing

- **Dependency conflicts**: all add-ins share the host process. The ASP.NET Core stack pulls in high-traffic assemblies (`Microsoft.Extensions.*`, `System.Text.Json`) that other add-ins may ship at different versions. Revit 2026+ isolates each add-in in its own `AssemblyLoadContext`, which largely solves this; on AutoCAD and Revit 2025, keep your dependency surface lean and test alongside commonly-installed add-ins.
- **Dynamic ports**: always bind `http://127.0.0.1:0`. A fixed port will eventually collide with another add-in or application on a user's machine. Loopback-only binding also avoids Windows Firewall prompts.
- **Shutdown**: stop the server in the add-in's terminate hook; an orphaned listener keeps the port until the CAD process exits.
