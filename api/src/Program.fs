module Djambi.Api.App

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe

open Djambi.Api.Persistence
open Djambi.Api.Http
open Djambi.Api.Domain

// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(EventId(), ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

// ---------------------------------
// Config and Main
// ---------------------------------

let configureCors (builder : CorsPolicyBuilder) =
    builder.WithOrigins("http://localhost:8080")
           .AllowAnyMethod()
           .AllowAnyHeader()
           |> ignore

let getRepositories(settings : IConfiguration) = 
    SqlUtility.connectionString <- settings.GetConnectionString("Main")

    let lobbyRepository = new LobbyRepository()
    {
        lobby = lobbyRepository
        gameStart = new GameStartRepository(lobbyRepository)
        play = new PlayRepository()
    }

let configureApp (app : IApplicationBuilder) =
//    let env = app.ApplicationServices.GetService<IHostingEnvironment>()
    let settings = app.ApplicationServices.GetService<IConfiguration>()

    let repositories = getRepositories(settings)

    let playService = new PlayService(repositories.play)
    let gameStartService = new GameStartService(repositories.gameStart, playService)

    let controllers : ControllerRegistry = 
        {
            lobby = new LobbyController(repositories.lobby)
            play = new PlayController(gameStartService, playService)
        }

    app.UseGiraffeErrorHandler(errorHandler)
    //(match env.IsDevelopment() with
    //| true  -> app.UseDeveloperExceptionPage()
    //| false -> app.UseGiraffeErrorHandler errorHandler)
        .UseCors(configureCors)
        .UseGiraffe(Routing.getRoutingTable controllers)

let configureServices (services : IServiceCollection) =
    services.AddCors()    |> ignore
    services.AddGiraffe() |> ignore

let configureLogging (builder : ILoggingBuilder) =
    let filter (l : LogLevel) = l.Equals LogLevel.Error
    builder.AddFilter(filter).AddConsole().AddDebug() |> ignore

let configureAppConfiguration(context : WebHostBuilderContext)(config :IConfigurationBuilder) =
    config.AddJsonFile("appsettings.json", false, true) |> ignore

[<EntryPoint>]
let main _ =
    WebHostBuilder()
        .UseKestrel()
        .UseIISIntegration()
        .Configure(Action<IApplicationBuilder> configureApp)
        .ConfigureServices(configureServices)
        .ConfigureLogging(configureLogging)
        .ConfigureAppConfiguration(configureAppConfiguration)
        .Build()
        .Run()
    0