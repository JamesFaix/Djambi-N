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

type Startup() =

    member val Configuration : IConfiguration = (Config.config :> IConfiguration) with get, set

    member __.ConfigureServices(services : IServiceCollection) : unit =
        // Framework services
        services.AddCors() |> ignore
        services.AddGiraffe() |> ignore // Phase out

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
            builder.WithOrigins(__.Configuration.GetValue<string>("webAddress"))
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    |> ignore

        let apiHandler =
            // DB layer
            let ctxProvider = CommandContextProvider(__.Configuration.GetValue<string>("apexConnectionString"))
            let gameRepo = GameRepository(ctxProvider)
            let userRepo = UserRepository(ctxProvider)
            let eventRepo = EventRepository(ctxProvider, gameRepo)
            let searchRepo = SearchRepository(ctxProvider)
            let sessionRepo = SessionRepository(ctxProvider, userRepo)
            let snapshotRepo = SnapshotRepository(ctxProvider)

            // App layer
            let boardServ = BoardService()
            let gameCrudServ = GameCrudService(gameRepo)
            let notificationServ = NotificationService(Log.Logger)
            let playerServ = PlayerService(gameRepo)
            let selectionOptionsServ = SelectionOptionsService()
            let sessionServ = SessionService(sessionRepo, userRepo)
            let userServ = UserService(userRepo)
            let gameStartServ = GameStartService(playerServ, selectionOptionsServ)
            let eventServ = EventService(gameStartServ)
            let indirectEffectsServ = IndirectEffectsService(eventServ, selectionOptionsServ)
            let playerStatusChangeServ = PlayerStatusChangeService(eventServ, indirectEffectsServ)
            let selectionServ = SelectionService(selectionOptionsServ)
            let turnServ = TurnService(eventServ, indirectEffectsServ, selectionOptionsServ)
      
            let boardMan = BoardManager(boardServ)
            let gameMan = GameManager(eventRepo,
                                    eventServ,
                                    gameCrudServ,
                                    gameRepo,
                                    gameStartServ,
                                    notificationServ,
                                    playerServ,
                                    playerStatusChangeServ,
                                    selectionServ,
                                    turnServ)
            let searchMan = SearchManager(searchRepo)
            let sessionMan = SessionManager(sessionServ)
            let snapshotMan = SnapshotManager(eventRepo, gameRepo, snapshotRepo)
            let userMan = UserManager(userServ)

            let httpUtil = HttpUtility(__.Configuration.GetValue<string>("cookieDomain"), sessionServ, Log.Logger)
            let boardCtrl = BoardController(boardMan, httpUtil)
            let eventCtrl = EventController(gameMan, httpUtil)
            let gameCtrl = GameController(gameMan, httpUtil)
            let notificationCtrl = NotificationController(httpUtil, notificationServ, Log.Logger)
            let playerCtrl = PlayerController(httpUtil, gameMan)
            let searchCtrl = SearchController(searchMan, httpUtil)
            let sessionCtrl = SessionController(httpUtil, sessionMan)
            let snapshotCtrl = SnapshotController(httpUtil, snapshotMan)
            let turnCtrl = TurnController(httpUtil, gameMan)
            let userCtrl = UserController(httpUtil, userMan)

            let getHandler : HttpFunc -> HttpContext -> HttpFuncResult =
                    choose [
                        subRoute "/api"
                            (choose [

                            //Session
                                POST >=> route Routes.sessions >=> (sessionCtrl :> ISessionController).openSession
                                DELETE >=> route Routes.sessions >=> (sessionCtrl :> ISessionController).closeSession

                            //Users
                                POST >=> route Routes.users >=> (userCtrl :> IUserController).createUser
                                GET >=> routef Routes.userFormat (userCtrl :> IUserController).getUser
                                GET >=> route Routes.currentUser >=> (userCtrl :> IUserController).getCurrentUser
                                DELETE >=> routef Routes.userFormat (userCtrl :> IUserController).deleteUser

                            //Board
                                GET >=> routef Routes.boardFormat (boardCtrl :> IBoardController).getBoard
                                GET >=> routef Routes.pathsFormat (boardCtrl :> IBoardController).getCellPaths

                            //Game lobby
                                POST >=> route Routes.games >=> (gameCtrl :> IGameController).createGame
                                GET >=> routef Routes.gameFormat (gameCtrl :> IGameController).getGame
                                PUT >=> routef Routes.gameParametersFormat (gameCtrl :> IGameController).updateGameParameters
                                POST >=> routef Routes.startGameFormat (gameCtrl :> IGameController).startGame

                            //Players
                                POST >=> routef Routes.playersFormat (playerCtrl :> IPlayerController).addPlayer
                                DELETE >=> routef Routes.playerFormat (playerCtrl :> IPlayerController).removePlayer
                                PUT >=> routef Routes.playerStatusChangeFormat (playerCtrl :> IPlayerController).updatePlayerStatus

                            //Turn actions
                                POST >=> routef Routes.selectCellFormat (turnCtrl :> ITurnController).selectCell
                                POST >=> routef Routes.resetTurnFormat (turnCtrl :> ITurnController).resetTurn
                                POST >=> routef Routes.commitTurnFormat (turnCtrl :> ITurnController).commitTurn

                            //Events
                                POST >=> routef Routes.eventsQueryFormat (eventCtrl :> IEventController).getEvents

                            //Search
                                POST >=> route Routes.searchGames >=> (searchCtrl :> ISearchController).searchGames

                            //Snapshots
                                POST >=> routef Routes.snapshotsFormat (snapshotCtrl :> ISnapshotController).createSnapshot
                                GET >=> routef Routes.snapshotsFormat (snapshotCtrl :> ISnapshotController).getSnapshotsForGame
                                DELETE >=> routef Routes.snapshotFormat (snapshotCtrl :> ISnapshotController).deleteSnapshot
                                POST >=> routef Routes.snapshotLoadFormat (snapshotCtrl :> ISnapshotController).loadSnapshot

                            //Notifications
                                GET >=> route Routes.notificationsSse >=> (notificationCtrl :> INotificationsController).connectSse
                                GET >=> route Routes.notificationsWebSockets >=> (notificationCtrl :> INotificationsController).connectWebSockets
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

                    if __.Configuration.GetValue<bool>("enableWebServerDevelopmentMode") then
                        //This has to be allowed in development because of the React development packages.
                        //TODO: Disable this in release configuration
                        isPublic <- isPublic || path.StartsWith("/node_modules")

                    if isPublic then
                        Log.Logger.Information(sprintf "File: GET %s" path)

                    isPublic
                ),
                (fun a ->
                    let sfOptions = StaticFileOptions()
                    sfOptions.FileProvider <- new PhysicalFileProvider(__.Configuration.GetValue<string>("webRoot"))
            
                    a.UseDefaultFiles()
                        .UseStaticFiles(sfOptions) 
                        |> ignore
                )
            )

        configureNewtonsoft() 

        if __.Configuration.GetValue<bool>("enableWebServer")
        then
            configureWebServer(app) |> ignore

        app.UseGiraffeErrorHandler(errorHandler) |> ignore
        app.UseCors(configureCors) |> ignore
        app.UseWebSockets() |> ignore
        app.UseGiraffe(apiHandler) |> ignore

        ()