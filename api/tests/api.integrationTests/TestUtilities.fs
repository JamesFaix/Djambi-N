[<AutoOpen>]
module Apex.Api.IntegrationTests.TestUtilities

open System
open System.Linq
open FSharp.Control.Tasks
open Microsoft.Extensions.Configuration
open Serilog
open Apex.Api.Common.Control
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.Db
open Apex.Api.Db.Interfaces
open Apex.Api.Logic
open Apex.Api.Logic.Interfaces
open Apex.Api.Model
open Apex.Api.Logic.Managers
open Apex.Api.Logic.Services
open Apex.Api.Db.Repositories

let private config =
    ConfigurationBuilder()
        .AddEnvironmentVariables("APEX_")
        .Build()

let connectionString = config.["apexConnectionString"]

let log = LoggerConfiguration().CreateLogger()

// DB layer
let ctxProvider = CommandContextProvider(connectionString)
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


let random = Random()
let randomAlphanumericString (length : int) : string =
    let chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
    let charsLen = chars.Length
    let randomChars = 
        [|for i in 0..length-1 -> 
            chars.[random.Next(charsLen)]
        |]
    String(randomChars)

let getCreateUserRequest() : CreateUserRequest =
    {
        name = "Test_" + (randomAlphanumericString 15)
        password = randomAlphanumericString 20
    }

let getLoginRequest(userRequest : CreateUserRequest) : LoginRequest =
    {
        username = userRequest.name
        password = userRequest.password
    }

let getGameParameters() : GameParameters =
    {
        regionCount = 3
        description = Some "Test"
        isPublic = false
        allowGuests = false
    }

let getCreateGameRequest (userId : int) : CreateGameRequest =
    {
        parameters = getGameParameters()
        createdByUserId = userId
    }

let getCreatePlayerRequest : CreatePlayerRequest =
    {
        kind = PlayerKind.User
        userId = None
        name = None
    }

let adminUserId = 1

let getSessionForUser (userId : int) : Session =
    {
        user =
            {
                id = userId
                name = ""
                privileges = []
            }
        id = 0
        token = ""
        createdOn = DateTime.MinValue
        expiresOn = DateTime.MinValue
    }

let createUser() : UserDetails AsyncHttpResult =
    let userRequest = getCreateUserRequest()
    userServ.createUser userRequest None

let createuserSessionAndGame(allowGuests : bool) : (UserDetails * Session * Game) AsyncHttpResult =
    task {
        let! user = createUser() |> thenValue

        let session = getSessionForUser user.id

        let parameters = { getGameParameters() with allowGuests = allowGuests }
        let! game = (gameMan :> IGameManager).createGame parameters session
                     |> thenValue

        return Ok <| (user, session, game)
    }

let fillEmptyPlayerSlots (game : Game) : Game AsyncHttpResult =
    task {
        let missingPlayerCount = game.parameters.regionCount - game.players.Length
        for i in Enumerable.Range(0, missingPlayerCount) do
            let name = sprintf "neutral%i" (i+1)
            let request = CreatePlayerRequest.neutral name
            let! _ = (gameRepo :> IGameRepository).addPlayer (game.id, request) |> thenValue
            ()

        return! (gameRepo :> IGameRepository).getGame game.id
    }

let emptyEventRequest : CreateEventRequest =
    {
        kind = EventKind.CellSelected //Kind shouldn't matter
        effects = []
        createdByUserId = 1
        actingPlayerId = None
    }

let createEventRequest (effects : Effect list) : CreateEventRequest =
    { emptyEventRequest with effects = effects }

let defaultGame : Game =
    {
        id = 0
        status = GameStatus.Pending
        createdBy = {
            userId = 0
            userName = ""
            time = DateTime.MinValue
        }
        parameters =
            {
                allowGuests = false
                description = None
                isPublic = false
                regionCount = 0
            }
        players = []
        pieces = []
        turnCycle = []
        currentTurn = None
    }

let setSessionUserId (userId : int) (session : Session) : Session=
    { session with
        user = { session.user with
                    id = userId
               }
    }

let setSessionPrivileges (privileges : Privilege list) (session : Session) : Session =
    { session with
        user = { session.user with
                    privileges = privileges
        }
    }