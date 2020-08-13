[<AutoOpen>]
module Apex.Api.IntegrationTests.TestUtilities

open System
open System.Linq
open System.Threading.Tasks
open FSharp.Control.Tasks
open Apex.Api.Db.Interfaces
open Apex.Api.Enums
open Apex.Api.Logic.Interfaces
open Apex.Api.Model

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

let getSessionForUser (user : User) : Session =
    {
        user = user
        id = 0
        token = ""
        createdOn = DateTime.MinValue
        expiresOn = DateTime.MinValue
    }

let createUser() : Task<User> =
    let host = HostFactory.createHost()
    let userRequest = getCreateUserRequest()
    host.Get<IUserManager>().createUser userRequest None

let createuserSessionAndGame(allowGuests : bool) : Task<(User * Session * Game)> =
    let host = HostFactory.createHost()
    task {
        let! user = createUser()

        let session = getSessionForUser user

        let parameters = { getGameParameters() with allowGuests = allowGuests }
        let! game = host.Get<IGameManager>().createGame parameters session

        return (user, session, game)
    }

let fillEmptyPlayerSlots (game : Game) : Task<Game> =
    let host = HostFactory.createHost()
    task {
        let missingPlayerCount = game.parameters.regionCount - game.players.Length
        for i in Enumerable.Range(0, missingPlayerCount) do
            let name = sprintf "neutral%i" (i+1)
            let player = CreatePlayerRequest.neutral name |> CreatePlayerRequest.toPlayer None
            let! _ = host.Get<IPlayerRepository>().addPlayer (game.id, player, true)
            ()

        return! host.Get<IGameRepository>().getGame game.id
    }

let emptyEventRequest (userId : int) : CreateEventRequest =
    {
        kind = EventKind.CellSelected //Kind shouldn't matter
        effects = []
        createdByUserId = userId
        actingPlayerId = None
    }

let createEventRequest (userId : int) (effects : Effect list) : CreateEventRequest =
    { (emptyEventRequest userId) with effects = effects }

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