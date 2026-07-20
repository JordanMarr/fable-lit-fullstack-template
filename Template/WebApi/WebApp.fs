module WebApi.WebApp

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Serilog
open Serilog.Events
open Shared.Api
open Serde.FS.AspNet

/// What the root path "/" serves.
type RootContent =
    /// Development: redirect "/" to the Vite dev server.
    | RedirectToDevServer of url: string
    /// Production: serve the built client from wwwroot (SPA fallback to index.html).
    | SpaFallback

type WebAppOptions =
    { Args: string[]
      /// None = ASP.NET default (launchSettings / ASPNETCORE_ENVIRONMENT).
      Environment: string option
      ContentRoot: string
      WebRoot: string
      /// Empty = ASP.NET defaults (launchSettings / ASPNETCORE_URLS).
      Urls: string list
      UseHttpsRedirection: bool
      /// Empty = CORS middleware not added.
      CorsOrigins: string list
      Root: RootContent }

/// Builds the fully configured WebApplication.
/// Shared by WebApi (standalone web host) and Desktop (in-process Kestrel behind a native window).
let build (options: WebAppOptions) =
    let builder =
        WebApplication.CreateBuilder(
            WebApplicationOptions(
                Args = options.Args,
                EnvironmentName = (options.Environment |> Option.toObj),
                ContentRootPath = options.ContentRoot,
                WebRootPath = options.WebRoot
            )
        )

    if not options.Urls.IsEmpty then
        builder.WebHost.UseUrls(Array.ofList options.Urls) |> ignore

    builder.Configuration
        .AddJsonFile("appsettings.json", optional = false, reloadOnChange = true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional = true)
        .AddEnvironmentVariables()
    |> ignore

    builder.Host.UseSerilog(fun _ configureLogger ->
        configureLogger
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.Console()
        |> ignore
    )
    |> ignore

    builder.Services.AddCors() |> ignore
    builder.Services.AddSingleton<ApiBuilder.ServerApi>() |> ignore

    let app = builder.Build()

    if app.Environment.IsDevelopment() then
        app.UseDeveloperExceptionPage() |> ignore

    if options.UseHttpsRedirection then
        app.UseHttpsRedirection() |> ignore

    if not options.CorsOrigins.IsEmpty then
        app.UseCors(fun (cors: CorsPolicyBuilder) ->
            cors
                .WithOrigins(Array.ofList options.CorsOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
            |> ignore
        )
        |> ignore

    app.UseStaticFiles() |> ignore
    app.UseSerilogRequestLogging() |> ignore

    let serverApi = app.Services.GetRequiredService<ApiBuilder.ServerApi>()
    app.MapRpcApi<IServerApi>(serverApi) |> ignore

    app.MapGet("/api/ping", Func<string>(fun () -> "pong")) |> ignore

    match options.Root with
    | RedirectToDevServer url ->
        app.MapGet("/", Func<IResult>(fun () -> Results.Redirect(url, permanent = true))) |> ignore
    | SpaFallback ->
        app.MapFallbackToFile("index.html") |> ignore

    app
