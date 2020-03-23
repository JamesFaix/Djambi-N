namespace Apex.Api.Host

open System
open System.Linq
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.FileProviders
open Microsoft.Extensions.Logging

open Giraffe
open Newtonsoft.Json
open Serilog

open Apex.Api.Common.Json
open Apex.Api.Db
open Apex.Api.Db.Repositories
open Apex.Api.Logic.Interfaces
open Apex.Api.Logic.Managers
open Apex.Api.Logic.Services
open Apex.Api.Web
open Apex.Api.Web.Controllers
open Apex.Api.Web.Interfaces
open Apex.Api.Model.Configuration
open Apex.Api.Db.Interfaces

type Startup() =

    member val Configuration : IConfiguration = (Config.config :> IConfiguration) with get, set

    member __.ConfigureServices(services : IServiceCollection) : unit =
        // Framework services
        services.AddCors() |> ignore
        services.AddGiraffe() |> ignore // Phase out

        // Configuration
        services.Configure<AppSettings>(__.Configuration) |> ignore
        services.Configure<SqlSettings>(__.Configuration.GetSection("Sql")) |> ignore
        services.Configure<WebServerSettings>(__.Configuration.GetSection("WebServer")) |> ignore
        services.Configure<ApiSettings>(__.Configuration.GetSection("Api")) |> ignore

        // Serilog
        services.AddSingleton<Serilog.ILogger>(Log.Logger) |> ignore

        // Database layer
        services.AddSingleton<CommandContextProvider>() |> ignore
        services.AddSingleton<IEventRepository, EventRepository>() |> ignore
        services.AddSingleton<GameRepository>() |> ignore
        services.AddSingleton<IGameRepository, GameRepository>() |> ignore
        services.AddSingleton<ISearchRepository, SearchRepository>() |> ignore
        services.AddSingleton<ISessionRepository, SessionRepository>() |> ignore
        services.AddSingleton<ISnapshotRepository, SnapshotRepository>() |> ignore
        services.AddSingleton<IUserRepository, UserRepository>() |> ignore

        // Logic layer
        services.AddSingleton<BoardService>() |> ignore
        services.AddSingleton<EventService>() |> ignore
        services.AddSingleton<GameCrudService>() |> ignore
        services.AddSingleton<GameStartService>() |> ignore
        services.AddSingleton<IndirectEffectsService>() |> ignore
        services.AddSingleton<INotificationService, NotificationService>() |> ignore
        services.AddSingleton<PlayerService>() |> ignore
        services.AddSingleton<PlayerStatusChangeService>() |> ignore
        services.AddSingleton<ISessionService, SessionService>() |> ignore
        services.AddSingleton<SelectionOptionsService>() |> ignore
        services.AddSingleton<SelectionService>() |> ignore
        services.AddSingleton<TurnService>() |> ignore
        services.AddSingleton<UserService>() |> ignore
        
        services.AddSingleton<IBoardManager, BoardManager>() |> ignore
        services.AddSingleton<ISearchManager, SearchManager>() |> ignore
        services.AddSingleton<ISessionManager, SessionManager>() |> ignore
        services.AddSingleton<ISnapshotManager, SnapshotManager>() |> ignore
        services.AddSingleton<IUserManager, UserManager>() |> ignore
        // TODO: Break up game manager
        services.AddSingleton<IEventManager, GameManager>() |> ignore
        services.AddSingleton<IGameManager, GameManager>() |> ignore
        services.AddSingleton<IPlayerManager, GameManager>() |> ignore
        services.AddSingleton<ITurnManager, GameManager>() |> ignore

        // Controller layer
        services.AddSingleton<HttpUtility>() |> ignore
        services.AddSingleton<IBoardController, BoardController>() |> ignore
        services.AddSingleton<IEventController, EventController>() |> ignore
        services.AddSingleton<IGameController, GameController>() |> ignore
        services.AddSingleton<INotificationsController, NotificationController>() |> ignore
        services.AddSingleton<IPlayerController, PlayerController>() |> ignore
        services.AddSingleton<ISearchController, SearchController>() |> ignore
        services.AddSingleton<ISessionController, SessionController>() |> ignore
        services.AddSingleton<ISnapshotController, SnapshotController>() |> ignore
        services.AddSingleton<ITurnController, TurnController>() |> ignore
        services.AddSingleton<IUserController, UserController>() |> ignore
        
        ()

    member __.Configure(app : IApplicationBuilder, env : IWebHostEnvironment) : unit =
        let configureNewtonsoft () =
            //This will only provide custom serialization of responses to clients.
            //Custom deserialization of request bodies does not work that same way in this framework.
            //See HttpUtility for deserialization.

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

        let errorHandler (ex : Exception) (logger : Microsoft.Extensions.Logging.ILogger) =
            logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
            Log.Logger.Error(ex, "An unexpected error occurred.")
            clearResponse >=> setStatusCode 500 >=> text ex.Message

        let configureCors (builder : CorsPolicyBuilder) =
            builder.WithOrigins(__.Configuration.GetValue<string>("Api:WebAddress"))
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    |> ignore

        let apiHandler =
            let getService () : 'a = app.ApplicationServices.GetService<'a>()
            let sessionCtrl = getService<ISessionController>()
            let userCtrl = getService<IUserController>()
            let boardCtrl = getService<IBoardController>()
            let gameCtrl = getService<IGameController>()
            let playerCtrl = getService<IPlayerController>()
            let turnCtrl = getService<ITurnController>()
            let eventCtrl = getService<IEventController>()
            let searchCtrl = getService<ISearchController>()
            let snapshotCtrl = getService<ISnapshotController>()
            let notificationCtrl = getService<INotificationsController>()

            let getHandler : HttpFunc -> HttpContext -> HttpFuncResult =
                    choose [
                        subRoute "/api"
                            (choose [

                            //Session
                                POST >=> route Routes.sessions >=> sessionCtrl.openSession
                                DELETE >=> route Routes.sessions >=> sessionCtrl.closeSession

                            //Users
                                POST >=> route Routes.users >=> userCtrl.createUser
                                GET >=> routef Routes.userFormat userCtrl.getUser
                                GET >=> route Routes.currentUser >=> userCtrl.getCurrentUser
                                DELETE >=> routef Routes.userFormat userCtrl.deleteUser

                            //Board
                                GET >=> routef Routes.boardFormat boardCtrl.getBoard
                                GET >=> routef Routes.pathsFormat boardCtrl.getCellPaths

                            //Game lobby
                                POST >=> route Routes.games >=> gameCtrl.createGame
                                GET >=> routef Routes.gameFormat gameCtrl.getGame
                                PUT >=> routef Routes.gameParametersFormat gameCtrl.updateGameParameters
                                POST >=> routef Routes.startGameFormat gameCtrl.startGame

                            //Players
                                POST >=> routef Routes.playersFormat playerCtrl.addPlayer
                                DELETE >=> routef Routes.playerFormat playerCtrl.removePlayer
                                PUT >=> routef Routes.playerStatusChangeFormat playerCtrl.updatePlayerStatus

                            //Turn actions
                                POST >=> routef Routes.selectCellFormat turnCtrl.selectCell
                                POST >=> routef Routes.resetTurnFormat turnCtrl.resetTurn
                                POST >=> routef Routes.commitTurnFormat turnCtrl.commitTurn

                            //Events
                                POST >=> routef Routes.eventsQueryFormat eventCtrl.getEvents

                            //Search
                                POST >=> route Routes.searchGames >=> searchCtrl.searchGames

                            //Snapshots
                                POST >=> routef Routes.snapshotsFormat snapshotCtrl.createSnapshot
                                GET >=> routef Routes.snapshotsFormat snapshotCtrl.getSnapshotsForGame
                                DELETE >=> routef Routes.snapshotFormat snapshotCtrl.deleteSnapshot
                                POST >=> routef Routes.snapshotLoadFormat snapshotCtrl.loadSnapshot

                            //Notifications
                                GET >=> route Routes.notificationsSse >=> notificationCtrl.connectSse
                                GET >=> route Routes.notificationsWebSockets >=> notificationCtrl.connectWebSockets
                            ])
                        setStatusCode 404 >=> text "Not Found" ]
            getHandler
    
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

                    if __.Configuration.GetValue<bool>("WebServer:EnableDevelopmentMode") then
                        //This has to be allowed in development because of the React development packages.
                        //TODO: Disable this in release configuration
                        isPublic <- isPublic || path.StartsWith("/node_modules")

                    if isPublic then
                        Log.Logger.Information(sprintf "File: GET %s" path)

                    isPublic
                ),
                (fun a ->
                    let sfOptions = StaticFileOptions()
                    sfOptions.FileProvider <- new PhysicalFileProvider(__.Configuration.GetValue<string>("WebServer:WebRoot"))
            
                    a.UseDefaultFiles()
                        .UseStaticFiles(sfOptions) 
                        |> ignore
                )
            )

        configureNewtonsoft() 

        if __.Configuration.GetValue<bool>("WebServer:Enable")
        then
            configureWebServer(app) |> ignore

        app.UseGiraffeErrorHandler(errorHandler) |> ignore
        app.UseCors(configureCors) |> ignore
        app.UseWebSockets() |> ignore
        app.UseGiraffe(apiHandler) |> ignore

        ()