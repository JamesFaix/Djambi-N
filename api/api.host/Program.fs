module Djambi.Api.Host.App

open System
open System.Linq
open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Configuration
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Logging
open Newtonsoft.Json
open Djambi.Api.Common.Json
open Djambi.Api.Db
open Djambi.Api.Logic
open Djambi.Api.Web
open Djambi.Utilities

// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(EventId(), ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

// ---------------------------------
// Config and Main
// ---------------------------------

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

    let settings = new JsonSerializerSettings()
    settings.Converters <- converters.ToList()
    JsonConvert.DefaultSettings <- (fun () -> settings)

let configureApp (app : IApplicationBuilder) =
//    let env = app.ApplicationServices.GetService<IHostingEnvironment>()

    //This will only provide custom serialization of responses to clients.
    //Custom deserialization of request bodies does not work that same way in this framework.
    //See HttpUtility for deserialization.
    configureNewtonsoft()

    let connStr = config.GetConnectionString("Main")
                        .Replace("{sqlAddress}", env.sqlAddress)

    let dbRoot = DbRoot(connStr)
    let servRoot = ServiceRoot(dbRoot)
    let manRoot = ManagerRoot(dbRoot, servRoot)
    let webRoot = WebRoot(env.cookieDomain, manRoot, servRoot)
    let routing = RoutingTable(webRoot)

    app.UseGiraffeErrorHandler(errorHandler)
    //(match env.IsDevelopment() with
    //| true  -> app.UseDeveloperExceptionPage()
    //| false -> app.UseGiraffeErrorHandler errorHandler)
        .UseCors(configureCors)
        .UseGiraffe(routing.getHandler)

let configureServices (services : IServiceCollection) =
    services.AddCors()    |> ignore
    services.AddGiraffe() |> ignore

let configureLogging (builder : ILoggingBuilder) =
    let filter (l : LogLevel) = l.Equals LogLevel.Error
    builder.AddFilter(filter).AddConsole().AddDebug() |> ignore

[<EntryPoint>]
let main _ =
    WebHostBuilder()
        .UseConfiguration(config)
        .UseUrls(env.apiAddress)
        .UseKestrel()
        .ConfigureServices(configureServices)
        .Configure(Action<IApplicationBuilder> configureApp)
        .ConfigureLogging(configureLogging)
        .Build()
        .Run()
    0