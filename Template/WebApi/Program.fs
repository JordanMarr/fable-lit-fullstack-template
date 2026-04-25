module WebApi.Program

open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Serilog
open Serilog.Events
open Shared.Api
open Serde.FS.Json.AspNet

[<EntryPoint; Serde.FS.EntryPoint>]
let main args =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot = Path.Combine(contentRoot, "wwwroot")

    let builder =
        WebApplication.CreateBuilder(
            WebApplicationOptions(
                Args = args,
                ContentRootPath = contentRoot,
                WebRootPath = webRoot
            )
        )

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

    app.UseHttpsRedirection() |> ignore

    app.UseCors(fun (cors: CorsPolicyBuilder) ->
        cors
            .WithOrigins([| "https://localhost:3000" |])
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

#if DEBUG
    app.MapGet("/", Func<IResult>(fun () -> Results.Redirect("https://localhost:3000", permanent = true)))
    |> ignore
#else
    app.MapFallbackToFile("index.html") |> ignore
#endif

    app.Run()
    0
