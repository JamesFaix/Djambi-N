module djambi.api.App

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open djambi.api.HttpHandlers

// ---------------------------------
// Web app
// ---------------------------------

let webApp =
    choose [
        subRoute "/api"
            (choose [
                POST >=> route "/users" >=> handleCreateUser
                GET >=> routef "/users/%i" handleGetUser
                DELETE >=> routef "/users/%i" handleDeleteUser
                PATCH >=> routef "/users/%i" handleUpdateUser

                GET >=> route "/games/open" >=> handleGetOpenGames
                POST >=> route "/games" >=> handleCreateGame
                DELETE >=> routef "/games/%i" handleDeleteGame

                POST >=> routef "/games/%i/users/%i" handleAddPlayerToGame
                DELETE >=> routef "/games/%i/users/%i" handleDeletePlayerFromGame

                POST >=> routef "/games/%i/start-request" handleStartGame

                GET >=> routef "/boards/%i" handleGetBoard

                GET >=> routef "/games/%i/state" handleGetGameState

                POST >=> routef "/games/%i/current-turn/selections" handleMakeSelection
                POST >=> routef "/games/%i/current-turn/reset-request" handleResetTurn
                POST >=> routef "/games/%i/current-turn/commit-request" handleCommitTurn

                POST >=> routef "/games/%id/messages" handleSendMessage
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
    (match env.IsDevelopment() with
    | true  -> app.UseDeveloperExceptionPage()
    | false -> app.UseGiraffeErrorHandler errorHandler)
        .UseCors(configureCors)
        .UseGiraffe(webApp)

let configureServices (services : IServiceCollection) =
    services.AddCors()    |> ignore
    services.AddGiraffe() |> ignore

let configureLogging (builder : ILoggingBuilder) =
    let filter (l : LogLevel) = l.Equals LogLevel.Error
    builder.AddFilter(filter).AddConsole().AddDebug() |> ignore

[<EntryPoint>]
let main _ =
    WebHostBuilder()
        .UseKestrel()
        .UseIISIntegration()
        .Configure(Action<IApplicationBuilder> configureApp)
        .ConfigureServices(configureServices)
        .ConfigureLogging(configureLogging)
        .Build()
        .Run()
    0