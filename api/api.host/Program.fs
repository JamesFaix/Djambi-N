module Djambi.Api.App

open System
open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Djambi.Api.Db
open Djambi.Api.Http
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

let config = ConfigurationBuilder()
                 .AddJsonFile("appsettings.json", false, true)
                 .AddJsonFile(Environment.environmentConfigPath(5), false, true) //Working directory is project directory
                 .Build()

SqlUtility.connectionString <- config.GetConnectionString("Main")
                                     .Replace("{sqlAddress}", config.["sqlAddress"])

let configureCors (builder : CorsPolicyBuilder) =
    builder.WithOrigins(config.["webAddress"])
           .AllowAnyMethod()
           .AllowAnyHeader()
           .AllowCredentials()
           |> ignore

let configureApp (app : IApplicationBuilder) =
//    let env = app.ApplicationServices.GetService<IHostingEnvironment>()

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
        .UseUrls(config.["apiAddress"])
        .UseKestrel()
        .UseIISIntegration()
        .ConfigureServices(configureServices)
        .Configure(Action<IApplicationBuilder> configureApp)
        .ConfigureLogging(configureLogging)
        .Build()
        .Run()
    0