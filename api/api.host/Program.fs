module Djambi.Api.App

open System
open System.Linq
open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Newtonsoft.Json
open Djambi.Api.Db
open Djambi.Api.Http
open Djambi.Utilities
open Djambi.Api.Common.Json
open Djambi.Api.Web

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

SqlUtility.connectionString <- config.GetConnectionString("Main")
                                     .Replace("{sqlAddress}", env.sqlAddress)

HttpUtility.cookieDomain <- env.cookieDomain

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

    app.UseGiraffeErrorHandler(errorHandler)
    //(match env.IsDevelopment() with
    //| true  -> app.UseDeveloperExceptionPage()
    //| false -> app.UseGiraffeErrorHandler errorHandler)
        .UseCors(configureCors)
        .UseGiraffe(Routing.getRoutingTable)

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
        .UseIISIntegration()
        .ConfigureServices(configureServices)
        .Configure(Action<IApplicationBuilder> configureApp)
        .ConfigureLogging(configureLogging)
        .Build()
        .Run()
    0