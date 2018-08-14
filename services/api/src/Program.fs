module djambi.api.App

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open djambi.api.HttpHandlers
open Djambi.Api.Persistence

// ---------------------------------
// Web app
// ---------------------------------

let webApp (handler : HttpHandler) =
    choose [
        subRoute "/api"
            (choose [
                
                GET >=> routef "/boards/%i" handler.getBoard
                GET >=> routef "/boards/%i/cells/%i/paths" handler.getCellPaths
                
                POST >=> route "/users" >=> handler.createUser
                GET >=> routef "/users/%i" handler.getUser
                DELETE >=> routef "/users/%i" handler.deleteUser
                PATCH >=> routef "/users/%i" handler.updateUser

                GET >=> route "/games/open" >=> handler.getOpenGames
                POST >=> route "/games" >=> handler.createGame
                DELETE >=> routef "/games/%i" handler.deleteGame

                POST >=> routef "/games/%i/users/%i" handler.addPlayerToGame
                DELETE >=> routef "/games/%i/users/%i" handler.removePlayerFromGame

                POST >=> routef "/games/%i/start-request" handler.startGame

                GET >=> routef "/games/%i/state" handler.getGameState

                POST >=> routef "/games/%i/current-turn/selections" handler.makeSelection
                POST >=> routef "/games/%i/current-turn/reset-request" handler.resetTurn
                POST >=> routef "/games/%i/current-turn/commit-request" handler.commitTurn

                POST >=> routef "/games/%id/messages" handler.sendMessage
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
    let repository = new Repository(settings.GetConnectionString("Main"))
    let handler = new HttpHandler(repository)

    (match env.IsDevelopment() with
    | true  -> app.UseDeveloperExceptionPage()
    | false -> app.UseGiraffeErrorHandler errorHandler)
        .UseCors(configureCors)
        .UseGiraffe(webApp handler)

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