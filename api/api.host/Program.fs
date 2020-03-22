module Apex.Api.Host.App

open System
open System.IO
open System.Linq
open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.FileProviders
open Microsoft.Extensions.Logging
open Newtonsoft.Json
open Serilog
open Apex.Api.Common.Json
open Apex.Api.Db
open Apex.Api.Logic
open Apex.Api.Web
open Apex.Api.Db.Repositories
open Apex.Api.Logic.Services
open Apex.Api.Logic.Managers
open Apex.Api.Web.Controllers
open Microsoft.AspNetCore.Http
open Apex.Api.Logic.Interfaces
open Apex.Api.Web.Interfaces

let options = Apex.Api.Host.Config.options

let log = 
    let logPath = Path.Combine(options.logsDir, "server.log")
    LoggerConfiguration()
        .WriteTo.File(logPath)
        .WriteTo.Console()
        .CreateLogger()

log.Information("Starting server...")

let errorHandler (ex : Exception) (logger : Microsoft.Extensions.Logging.ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    log.Error(ex, "An unexpected error occurred.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

if options.enableWebServer then
    log.Information(sprintf "Web root path: %s" options.webRoot)

let configureCors (builder : CorsPolicyBuilder) =
    builder.WithOrigins(options.webAddress)
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

            if options.enableWebServerDevelopmentMode then
                //This has to be allowed in development because of the React development packages.
                //TODO: Disable this in release configuration
                isPublic <- isPublic || path.StartsWith("/node_modules")

            if isPublic then
                log.Information(sprintf "File: GET %s" path)

            isPublic
        ),
        (fun a ->
            let sfOptions = StaticFileOptions()
            sfOptions.FileProvider <- new PhysicalFileProvider(options.webRoot)
            
            a.UseDefaultFiles()
                .UseStaticFiles(sfOptions) 
                |> ignore
        )
    )

let apiHandler =
    // DB layer
    let ctxProvider = CommandContextProvider(options.apexConnectionString)
    let gameRepo = GameRepository(ctxProvider)
    let userRepo = UserRepository(ctxProvider)
    let eventRepo = EventRepository(ctxProvider, gameRepo)
    let searchRepo = SearchRepository(ctxProvider)
    let sessionRepo = SessionRepository(ctxProvider, userRepo)
    let snapshotRepo = SnapshotRepository(ctxProvider)

    // App layer
    let boardServ = BoardService()
    let gameCrudServ = GameCrudService(gameRepo)
    let notificationServ = NotificationService(log)
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

    let httpUtil = HttpUtility(options.cookieDomain, sessionServ, log)
    let boardCtrl = BoardController(boardMan, httpUtil)
    let eventCtrl = EventController(gameMan, httpUtil)
    let gameCtrl = GameController(gameMan, httpUtil)
    let notificationCtrl = NotificationController(httpUtil, notificationServ, log)
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

let configureServices (services : IServiceCollection) =
    services.AddCors()    |> ignore
    services.AddGiraffe() |> ignore

let configureApp (app : IApplicationBuilder) =
    //This will only provide custom serialization of responses to clients.
    //Custom deserialization of request bodies does not work that same way in this framework.
    //See HttpUtility for deserialization.

    configureNewtonsoft()

    (if options.enableWebServer then configureWebServer app else app)
        .UseGiraffeErrorHandler(errorHandler)
        .UseCors(configureCors)
        .UseWebSockets()
        .UseGiraffe(apiHandler)

    log.Information("Server started.")

[<EntryPoint>]
let main _ =
    (WebHostBuilder()
        .UseUrls(options.apiAddress)
        .UseKestrel()
        |> (fun b -> if options.enableWebServer then b.UseWebRoot(options.webRoot) else b)
    )
        .ConfigureServices(configureServices)
        .Configure(Action<IApplicationBuilder> configureApp)
        .Build()
        .Run()
    0