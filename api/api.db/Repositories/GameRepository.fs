module Djambi.Api.Db.Repositories.GameRepository

open System
open Dapper
open Newtonsoft.Json
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
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

let createGame (request : CreateGameRequest) : int AsyncHttpResult =
    let param = DynamicParameters()
                    .add("RegionCount", request.parameters.regionCount)
                    .add("CreatedByUserId", request.createdByUserId)
                    .add("AllowGuests", request.parameters.allowGuests)
                    .add("IsPublic", request.parameters.isPublic)
                    .addOption("Description", request.parameters.description)

    let cmd = proc("Games_Create", param)

    querySingle<int>(cmd, "Game")

let addPlayer (gameId : int, request : CreatePlayerRequest) : Player AsyncHttpResult =
    let param = DynamicParameters()
                    .add("GameId", gameId)
                    .add("PlayerKindId", mapPlayerKindToId request.kind)
                    .addOption("UserId", request.userId)
                    .addOption("Name", request.name)

    let cmd = proc("Players_Add", param)

    querySingle<int>(cmd, "Player")
    |> thenBindAsync getPlayer

let removePlayer(playerId : int) : Unit AsyncHttpResult =
    let param = DynamicParameters()
                    .add("PlayerId", playerId)
    let cmd = proc("Players_Remove", param)
    queryUnit(cmd, "Player")

let getNeutralPlayerNames() : string list AsyncHttpResult =
    let param = DynamicParameters()
    let cmd = proc("Players_GetNeutralNames", param)
    queryMany<string>(cmd, "Neutral player names")
    
[<Obsolete("Deprecate this and use updateGame")>]
let updateGameParameters (gameId : int) (parameters : GameParameters) : Unit AsyncHttpResult =
    let param = DynamicParameters()
                    .add("GameId", gameId)
                    .addOption("Description", parameters.description)
                    .add("AllowGuests", parameters.allowGuests)
                    .add("IsPublic", parameters.isPublic)
                    .add("RegionCount", parameters.regionCount)
    let cmd = proc("Games_UpdateParameters", param)
    queryUnit(cmd, "Game")
    
[<Obsolete("Deprecate this and use updateGame")>]
let updateGameState(request : UpdateGameStateRequest) : Unit AsyncHttpResult =
    let param = DynamicParameters()
                    .add("GameId", request.gameId)
                    .add("GameStatusId", request.status |> mapGameStatusToId)
                    .add("PiecesJson", JsonConvert.SerializeObject(request.pieces))
                    .add("CurrentTurnJson", JsonConvert.SerializeObject(request.currentTurn))
                    .add("TurnCycleJson", JsonConvert.SerializeObject(request.turnCycle))
    let cmd = proc("Games_UpdateState", param)
    queryUnit(cmd, "Game")
            
let updateGame(game : Game) : Unit AsyncHttpResult =
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
    let cmd = proc("Games_Update", param)
    queryUnit(cmd, "Game")

[<Obsolete("Deprecate this and use updatePlayer")>]
let setPlayerStartConditions(request : SetPlayerStartConditionsRequest) : Unit AsyncHttpResult =
    let param = DynamicParameters()
                    .add("PlayerId", request.playerId)
                    .add("ColorId", byte request.colorId)
                    .add("StartingRegion", byte request.startingRegion)
                    .addOption("StartingTurnNumber", request.startingTurnNumber |> Option.map byte)
    let cmd = proc("Players_SetStartConditions", param)
    queryUnit(cmd, "Player")

[<Obsolete("Deprecate this and use updatePlayer")>]
let killPlayer(playerId : int) : Unit AsyncHttpResult =
    let param = DynamicParameters()
                    .add("PlayerId", playerId)
    let cmd = proc("Players_Kill", param)
    queryUnit(cmd, "Player")

let updatePlayer(player : Player) : Unit AsyncHttpResult =
    let param = DynamicParameters()
                    .add("PlayerId", player.id)
                    .addOption("ColorId", player.colorId)
                    .addOption("StartingTurnNumber", player.startingTurnNumber)
                    .addOption("StartingRegion", player.startingRegion)
                    .addOption("IsAlive", player.isAlive)
    let cmd = proc("Players_Update", param)
    queryUnit(cmd, "Player")