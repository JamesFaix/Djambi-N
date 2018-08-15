module djambi.api.App

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

// ---------------------------------
// Web app
// ---------------------------------

let webApp (controllers : ControllerRegistry) =
    choose [
        subRoute "/api"
            (choose [  
            
                POST >=> route "/users" >=> controllers.lobby.createUser
                GET >=> routef "/users/%i" controllers.lobby.getUser
                DELETE >=> routef "/users/%i" controllers.lobby.deleteUser
                PATCH >=> routef "/users/%i" controllers.lobby.updateUser

                GET >=> route "/games/open" >=> controllers.lobby.getOpenGames
                POST >=> route "/games" >=> controllers.lobby.createGame
                DELETE >=> routef "/games/%i" controllers.lobby.deleteGame

                POST >=> routef "/games/%i/users/%i" controllers.lobby.addPlayerToGame
                DELETE >=> routef "/games/%i/users/%i" controllers.lobby.removePlayerFromGame

                POST >=> routef "/games/%i/start-request" controllers.lobby.startGame

                GET >=> routef "/boards/%i" controllers.play.getBoard
                GET >=> routef "/boards/%i/cells/%i/paths" controllers.play.getCellPaths
                
                GET >=> routef "/games/%i/state" controllers.play.getGameState

                POST >=> routef "/games/%i/current-turn/selections" controllers.play.makeSelection
                POST >=> routef "/games/%i/current-turn/reset-request" controllers.play.resetTurn
                POST >=> routef "/games/%i/current-turn/commit-request" controllers.play.commitTurn

                POST >=> routef "/games/%id/messages" controllers.play.sendMessage
            ])
        setStatusCode 404 >=> text "Not Found" ]

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

let configureApp (app : IApplicationBuilder) =
    let env = app.ApplicationServices.GetService<IHostingEnvironment>()

    let settings = app.ApplicationServices.GetService<IConfiguration>()
    let cnStr = settings.GetConnectionString("Main")
    let lobbyRepository = new LobbyRepository(cnStr)
    let playRepository = new PlayRepository(cnStr)

    let controllers = 
        {
            lobby = new LobbyController(lobbyRepository)
            play = new PlayController(playRepository)
        }

    app.UseGiraffeErrorHandler(errorHandler)
    //(match env.IsDevelopment() with
    //| true  -> app.UseDeveloperExceptionPage()
    //| false -> app.UseGiraffeErrorHandler errorHandler)
        .UseCors(configureCors)
        .UseGiraffe(webApp controllers)

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