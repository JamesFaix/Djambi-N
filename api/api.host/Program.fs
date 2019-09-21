module Djambi.Api.Host.App

open System
open System.IO
open System.Linq
open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.FileProviders
open Microsoft.Extensions.Logging
open Newtonsoft.Json
open Serilog
open Djambi.Api.Common.Json
open Djambi.Api.Db
open Djambi.Api.Logic
open Djambi.Api.Web
open Djambi.Utilities

let env = Environment.load(5)

let log = 
    let logPath = Path.Combine(env.root, "logs/server.log")
    LoggerConfiguration()
        .WriteTo.File(logPath)
        .WriteTo.Console()
        .CreateLogger()

log.Information("Starting server...")

let errorHandler (ex : Exception) (logger : Microsoft.Extensions.Logging.ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    log.Error(ex, "An unexpected error occurred.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

let config = 
    ConfigurationBuilder()
        .AddJsonFile("appsettings.json", false, true)
        .Build()

let enableWebServer = config.GetValue<bool>("AppSettings:EnableWebServer")
let enableWebServerDevelopmentMode = config.GetValue<bool>("AppSettings:EnableWebServerDevelopmentMode")
let webRoot = Path.Combine(env.root, "web")

if enableWebServer then
    log.Information(sprintf "Web root path: %s" webRoot)

let configureCors (builder : CorsPolicyBuilder) =
    builder.WithOrigins(env.webAddress)
           .AllowAnyMethod()
           .AllowAnyHeader()
           .AllowCredentials()
           |> ignore

let configureNewtonsoft () =
    let converters : List<JsonConverter> =
        [
            OptionJsonConverter()
            TupleArrayJsonConverter()
            UnionEnumJsonConverter()
            SingleFieldUnionJsonConverter()
        ]

    let settings = JsonSerializerSettings()
    settings.Converters <- converters.ToList()
    JsonConvert.DefaultSettings <- (fun () -> settings)
    
let configureWebServer(app : IApplicationBuilder) : IApplicationBuilder =
    let isDefault (path : string) =
        path = "/" || path = "/index.html"

    app.UseWhen(
        (fun ctx ->
            let path = ctx.Request.Path.ToString()
            let mutable isPublic =
                path.StartsWith("/resources") ||
                path.StartsWith("/dist") ||
                isDefault path

            if enableWebServerDevelopmentMode then
                //This has to be allowed in development because of the React development packages.
                //TODO: Disable this in release configuration
                isPublic <- isPublic || path.StartsWith("/node_modules")

            if isPublic then
                log.Information(sprintf "File: GET %s" path)

            isPublic
        ),
        (fun a ->
            let sfOptions = StaticFileOptions()
            sfOptions.FileProvider <- new PhysicalFileProvider(webRoot)
            
            a.UseDefaultFiles()
                .UseStaticFiles(sfOptions) 
                |> ignore
        )
    )

let apiHandler =
    let connStr = config.GetConnectionString("Main")
                        .Replace("{sqlAddress}", env.sqlAddress)

    let dbRoot = DbRoot(connStr)
    let servRoot = ServiceRoot(dbRoot, log)
    let manRoot = ManagerRoot(dbRoot, servRoot)
    let webRoot = WebRoot(env.cookieDomain, manRoot, servRoot, log)
    let routing = RoutingTable(webRoot)

    routing.getHandler

let configureServices (services : IServiceCollection) =
    services.AddCors()    |> ignore
    services.AddGiraffe() |> ignore

let configureApp (app : IApplicationBuilder) =
    //This will only provide custom serialization of responses to clients.
    //Custom deserialization of request bodies does not work that same way in this framework.
    //See HttpUtility for deserialization.

    configureNewtonsoft()

    (if enableWebServer then configureWebServer app else app)
        .UseGiraffeErrorHandler(errorHandler)
        .UseCors(configureCors)
        .UseWebSockets()
        .UseGiraffe(apiHandler)

    log.Information("Server started.")

[<EntryPoint>]
let main _ =
    (WebHostBuilder()
        .UseConfiguration(config)
        .UseUrls(env.apiAddress)
        .UseKestrel()
        |> (fun b -> if enableWebServer then b.UseWebRoot(webRoot) else b)
    )
        .ConfigureServices(configureServices)
        .Configure(Action<IApplicationBuilder> configureApp)
        .Build()
        .Run()
    0