module WebApi.Program

open Giraffe
open Shared
open System
open System.IO
open WebApi
open Serilog
open Serilog.Events
open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Logging

let rec private unwrapEx (ex: Exception) =
    if not (isNull ex.InnerException) then 
        printfn "Parent Ex: %s" ex.Message
        printfn "Unwrapping inner exception..."
        unwrapEx ex.InnerException
    else ex

let fableRemotingErrorHandler (ex: Exception) (ri: RouteInfo<HttpContext>) = 
    let logger = ri.httpContext.GetLogger()
    logger.LogError(sprintf "Error at %s on method %s" ri.path ri.methodName)
    // Decide whether or not you want to propagate the error to the client
    let ex = unwrapEx ex
    logger.LogError(sprintf "Error: %s" ex.Message)
    Propagate "An error occurred while processing the request."
    
let fableRemotingApi =
    Remoting.createApi()
    |> Remoting.fromContext (fun (ctx: HttpContext) -> ctx.GetService<ApiBuilder.ServerApi>().Build(ctx))
    |> Remoting.withRouteBuilder Api.routerPaths
    |> Remoting.withErrorHandler fableRemotingErrorHandler
    |> Remoting.buildHttpHandler

let webApp =     
    choose [ 

        fableRemotingApi

        GET >=> routeCi "/api/ping" >=> text "pong"
                
        //GET >=> routeCi "/signin" >=> (fun next ctx ->
        //    if ctx.User.Identity.IsAuthenticated then 
        //        // Try to get the redirectUrl embedded in the app sign-in link
        //        match ctx.Request.Query.TryGetValue("redirectUrl") with
        //        | true, urlValue -> redirectTo false urlValue.[0] next ctx 
        //        | _ -> redirectTo false "/" next ctx 
        //    else 
        //        challenge OpenIdConnectDefaults.AuthenticationScheme next ctx
        //)

        //GET >=> routeCi "/signout" 
        //    >=> (fun next ctx -> 
        //        ctx.Response.Cookies.Delete(".AspNetCore.Cookies")
        //        htmlView Views.goodbye next ctx) 


#if DEBUG
        GET >=> htmlView Views.devMode
#else
        GET >=> htmlFile "wwwroot/index.html"
#endif
    ]

let serviceConfig (ctx: WebHostBuilderContext) (services: IServiceCollection) =
    let cfg = ctx.Configuration
    services.AddCors()    |> ignore
    services.AddGiraffe() |> ignore
    services.AddSingleton<ApiBuilder.ServerApi>() |> ignore
    services.AddLogging() |> ignore
    

let configureAppSettings (context: WebHostBuilderContext) (config: IConfigurationBuilder) =
    config
        .AddJsonFile("appsettings.json", false, true)
        .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json" , true)
        .AddEnvironmentVariables()
        |> ignore

let configureCors (builder: CorsPolicyBuilder) =
    builder.WithOrigins([| "https://localhost:3000" |])
        .AllowAnyMethod()
        .AllowAnyHeader()
        |> ignore

let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

let configureApp (app : IApplicationBuilder) =
    let env = app.ApplicationServices.GetService<IWebHostEnvironment>()
    (if env.IsDevelopment()
     then app.UseDeveloperExceptionPage()
     else app.UseGiraffeErrorHandler(errorHandler))
        .UseHttpsRedirection()
        .UseCors(configureCors)
        .UseStaticFiles()
        .UseSerilogRequestLogging()
        .UseGiraffe(webApp)

[<EntryPoint>]
let main args =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot     = Path.Combine(contentRoot, "wwwroot")
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(fun (webHostBuilder: IWebHostBuilder) ->
            webHostBuilder
                .UseContentRoot(contentRoot)
                .UseWebRoot(webRoot)
                .Configure(Action<IApplicationBuilder> configureApp)
                .ConfigureServices(serviceConfig)
                .ConfigureAppConfiguration configureAppSettings
                |> ignore
        )
        .UseSerilog(fun hostingContext configureLogger ->
            configureLogger
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console() 
                |> ignore
        )
        .Build()
        .Run()
    0