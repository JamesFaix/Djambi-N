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
open Djambi.Api.Common.Json
open Djambi.Api.Db
open Djambi.Api.Logic
open Djambi.Api.Web
open Djambi.Utilities
open Serilog

let log = 
    LoggerConfiguration()
        .WriteTo.File("test.log")
        .WriteTo.Console()
        .CreateLogger()

let errorHandler (ex : Exception) (logger : Microsoft.Extensions.Logging.ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

let env = Environment.load(5)
let config = ConfigurationBuilder()
                 .AddJsonFile("appsettings.json", false, true)
                 .Build()

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
    
let configureFileServer(app : IApplicationBuilder) : IApplicationBuilder =
    app.UseDefaultFiles()
        .UseWhen(
            (fun ctx ->
                let path = ctx.Request.Path.ToString()
                let isPublic =
                    path.StartsWith("/resources") ||
                    path.StartsWith("/dist") ||
                    path = "/index.html" ||

                    //This has to be allowed in development because of the React development packages.
                    //TODO: Disable this in release configuration
                    path.StartsWith("/node_modules")

                if isPublic then
                    log.Information(sprintf "File: GET %s" path)

                isPublic
            ),
            (fun a ->
                let clientPath = Path.Combine(env.root, "web")
                let sfOptions = StaticFileOptions()
                sfOptions.FileProvider <- new PhysicalFileProvider(clientPath)
                a.UseStaticFiles(sfOptions) |> ignore
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

let configureApp (app : IApplicationBuilder) =
    //This will only provide custom serialization of responses to clients.
    //Custom deserialization of request bodies does not work that same way in this framework.
    //See HttpUtility for deserialization.
    configureNewtonsoft()

    configureFileServer(app)
        .UseGiraffeErrorHandler(errorHandler)
        //(match env.IsDevelopment() with
        //| true  -> app.UseDeveloperExceptionPage()
        //| false -> app.UseGiraffeErrorHandler errorHandler)
        .UseCors(configureCors)
        .UseGiraffe(apiHandler)

let configureServices (services : IServiceCollection) =
    services.AddCors()    |> ignore
    services.AddGiraffe() |> ignore

[<EntryPoint>]
let main _ =
    let webRoot = Path.Combine(env.root, "web")

    WebHostBuilder()
        .UseConfiguration(config)
        .UseUrls(env.apiAddress)
        .UseKestrel()
        .UseWebRoot(webRoot)
        .ConfigureServices(configureServices)
        .Configure(Action<IApplicationBuilder> configureApp)
        .Build()
        .Run()
    0