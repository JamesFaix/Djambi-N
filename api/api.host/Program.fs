module Apex.Api.Host.App

open System
open System.IO
open System.Linq
open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.FileProviders
open Microsoft.Extensions.Logging
open Newtonsoft.Json
open Serilog
open Apex.Api.Common.Json
open Apex.Api.Db
open Apex.Api.Logic
open Apex.Api.Web

let options = Apex.Api.Host.Config.options

let log = 
    let logPath = Path.Combine(options.logsDir, "server.log")
    LoggerConfiguration()
        .WriteTo.File(logPath)
        .WriteTo.Console()
        .CreateLogger()

log.Information("Starting server...")

let errorHandler (ex : Exception) (logger : Microsoft.Extensions.Logging.ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    log.Error(ex, "An unexpected error occurred.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

if options.enableWebServer then
    log.Information(sprintf "Web root path: %s" options.webRoot)

let configureCors (builder : CorsPolicyBuilder) =
    builder.WithOrigins(options.webAddress)
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

            if options.enableWebServerDevelopmentMode then
                //This has to be allowed in development because of the React development packages.
                //TODO: Disable this in release configuration
                isPublic <- isPublic || path.StartsWith("/node_modules")

            if isPublic then
                log.Information(sprintf "File: GET %s" path)

            isPublic
        ),
        (fun a ->
            let sfOptions = StaticFileOptions()
            sfOptions.FileProvider <- new PhysicalFileProvider(options.webRoot)
            
            a.UseDefaultFiles()
                .UseStaticFiles(sfOptions) 
                |> ignore
        )
    )

let apiHandler =
    let dbRoot = DbRoot(options.djambiConnectionString)
    let servRoot = ServiceRoot(dbRoot, log)
    let manRoot = ManagerRoot(dbRoot, servRoot)
    let webRoot = WebRoot(options.cookieDomain, manRoot, servRoot, log)
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

    (if options.enableWebServer then configureWebServer app else app)
        .UseGiraffeErrorHandler(errorHandler)
        .UseCors(configureCors)
        .UseWebSockets()
        .UseGiraffe(apiHandler)

    log.Information("Server started.")

[<EntryPoint>]
let main _ =
    (WebHostBuilder()
        .UseUrls(options.apiAddress)
        .UseKestrel()
        |> (fun b -> if options.enableWebServer then b.UseWebRoot(options.webRoot) else b)
    )
        .ConfigureServices(configureServices)
        .Configure(Action<IApplicationBuilder> configureApp)
        .Build()
        .Run()
    0