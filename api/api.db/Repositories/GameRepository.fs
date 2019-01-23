module Djambi.Api.Db.Repositories.GameRepository

open System
open Dapper
open FSharp.Control.Tasks
open Newtonsoft.Json
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db
open Djambi.Api.Db.Mapping
open Djambi.Api.Db.Model
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model
    
let getGamesWithoutPlayers (query : GamesQuery) : Game List AsyncHttpResult =
    let param = DynamicParameters()
                    .addOption("GameId", query.gameId)
                    .addOption("DescriptionContains", query.descriptionContains)
                    .addOption("CreatedByUserId", query.createdByUserId)
                    .addOption("PlayerUserId", query.playerUserId)
                    .addOption("IsPublic", query.isPublic)
                    .addOption("AllowGuests", query.allowGuests)

    let cmd = proc("Games_Get", param)

    queryMany<GameSqlModel>(cmd, "Game")
    |> thenMap (List.map mapGameResponse)

let getGameWithoutPlayers (gameId : int) : Game AsyncHttpResult =
    let param = DynamicParameters()
                    .add("GameId", gameId)
                    .add("DescriptionContains", null)
                    .add("CreatedByUserId", null)
                    .add("PlayerUserId",  null)
                    .add("IsPublic", null)
                    .add("AllowGuests", null)

    let cmd = proc("Games_Get", param)

    querySingle<GameSqlModel>(cmd, "Game")
    |> thenMap mapGameResponse
        
let getPlayersForGames (gameIds : int list) : Player List AsyncHttpResult =
    let param = DynamicParameters()
                    .add("GameIds", String.Join(',', gameIds))
                    .add("PlayerId", null);

    let cmd = proc("Players_Get", param)

    queryMany<PlayerSqlModel>(cmd, "Player")
    |> thenMap (List.map mapPlayerResponse)

let getPlayer (playerId : int) : Player AsyncHttpResult =
    let param = DynamicParameters()
                    .add("GameIds", null)
                    .add("PlayerId", playerId);

    let cmd = proc("Players_Get", param)

    querySingle<PlayerSqlModel>(cmd, "Player")
    |> thenMap mapPlayerResponse

let getGame (gameId : int) : Game AsyncHttpResult = 
    getGameWithoutPlayers gameId
    |> thenBindAsync (fun game -> 
        getPlayersForGames [gameId]
        |> thenMap (fun players -> { game with players = players })
    )

let getGames (query : GamesQuery) : Game list AsyncHttpResult = 
    getGamesWithoutPlayers query
    |> thenBindAsync (fun games -> 
        getPlayersForGames (games |> List.map (fun g -> g.id))
        |> thenMap (fun players -> 
            let playersByGame = players |> List.groupBy (fun p -> p.gameId)
            games 
            |> List.map (fun g -> 
                let playersOpt = playersByGame |> List.tryFind (fun (gameId, _) -> gameId = g.id) 
                let ps = match playersOpt with 
                         | Some (_, players) -> players
                         | _ -> List.empty
                { g with players = ps}
            )
        )
    )

let getCreateGameCommand (request : CreateGameRequest) : CommandDefinition =
    let param = DynamicParameters()
                    .add("RegionCount", request.parameters.regionCount)
                    .add("CreatedByUserId", request.createdByUserId)
                    .add("AllowGuests", request.parameters.allowGuests)
                    .add("IsPublic", request.parameters.isPublic)
                    .addOption("Description", request.parameters.description)
    proc("Games_Create", param)

let createGame (request : CreateGameRequest) : int AsyncHttpResult =
    let cmd = getCreateGameCommand request
    querySingle<int>(cmd, "Game")

let getAddPlayerCommand (gameId : int, request : CreatePlayerRequest) : CommandDefinition =
    let param = DynamicParameters()
                    .add("GameId", gameId)
                    .add("PlayerKindId", mapPlayerKindToId request.kind)
                    .addOption("UserId", request.userId)
                    .addOption("Name", request.name)
    proc("Players_Add", param)

let addPlayer (gameId : int, request : CreatePlayerRequest) : Player AsyncHttpResult =
    let cmd = getAddPlayerCommand (gameId, request)
    querySingle<int>(cmd, "Player")
    |> thenBindAsync getPlayer

let getRemovePlayerCommand (playerId : int) : CommandDefinition = 
    let param = DynamicParameters()
                    .add("PlayerId", playerId)
    proc("Players_Remove", param)

let removePlayer(playerId : int) : Unit AsyncHttpResult =
    let cmd = getRemovePlayerCommand playerId
    queryUnit(cmd, "Player")

let getNeutralPlayerNames() : string list AsyncHttpResult =
    let param = DynamicParameters()
    let cmd = proc("Players_GetNeutralNames", param)
    queryMany<string>(cmd, "Neutral player names")

let getUpdateGameCommand (game : Game) : CommandDefinition =
    let param = DynamicParameters()
                    .add("GameId", game.id)
                    .addOption("Description", game.parameters.description)
                    .add("AllowGuests", game.parameters.allowGuests)
                    .add("IsPublic", game.parameters.isPublic)
                    .add("RegionCount", game.parameters.regionCount)
                    .add("GameStatusId", game.status |> mapGameStatusToId)
                    .add("PiecesJson", JsonConvert.SerializeObject(game.pieces))
                    .add("CurrentTurnJson", JsonConvert.SerializeObject(game.currentTurn))
                    .add("TurnCycleJson", JsonConvert.SerializeObject(game.turnCycle))
    proc("Games_Update", param)
    
//Exposed for test setup
let updateGame(game : Game) : Unit AsyncHttpResult =
    let cmd = getUpdateGameCommand game
    queryUnit(cmd, "Game")

let getUpdatePlayerCommand (player : Player) : CommandDefinition =
    let param = DynamicParameters()
                    .add("PlayerId", player.id)
                    .addOption("ColorId", player.colorId)
                    .addOption("StartingTurnNumber", player.startingTurnNumber)
                    .addOption("StartingRegion", player.startingRegion)
                    .addOption("IsAlive", player.isAlive)
    proc("Players_Update", param)
    
//Exposed for test setup
let updatePlayer(player : Player) : Unit AsyncHttpResult =
    let cmd = getUpdatePlayerCommand player
    queryUnit(cmd, "Player")

let createGameAndAddPlayer (gameRequest : CreateGameRequest, playerRequest : CreatePlayerRequest) : int AsyncHttpResult =
    task {
        use conn = SqlUtility.getConnection()
        use tran = conn.BeginTransaction()
       
        try 
            let cmd = getCreateGameCommand gameRequest
                      |> CommandDefinition.withTransaction tran
            let! gameId = conn.QuerySingleAsync<int> cmd
            let cmd = getAddPlayerCommand (gameId, playerRequest)
                      |> CommandDefinition.withTransaction tran
            let! _ = conn.ExecuteAsync cmd
            tran.Commit()
            return Ok gameId
        with 
        | _ as ex -> return Error <| (SqlUtility.catchSqlException ex "Effect")
    }