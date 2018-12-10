module Djambi.Api.Db.Repositories.GameRepository

open System
open Dapper
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
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
        
let deleteGame (gameId : int) : Unit AsyncHttpResult =
    let param = DynamicParameters()
                    .add("GameId", gameId)
    let cmd = proc("Games_Delete", param)
    queryUnit(cmd, "Game")

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

let createGame (request : CreateGameRequest, createdByUserId : int) : Game AsyncHttpResult =
    let param = DynamicParameters()
                    .add("RegionCount", request.regionCount)
                    .add("CreatedByUserId", createdByUserId)
                    .add("AllowGuests", request.allowGuests)
                    .add("IsPublic", request.isPublic)
                    .addOption("Description", request.description)

    let cmd = proc("Games_Create", param)

    querySingle<int>(cmd, "Game")
    |> thenBindAsync getGame

let addPlayer (gameId : int, request : CreatePlayerRequest) : Player AsyncHttpResult =
    let param = DynamicParameters()
                    .add("GameId", gameId)
                    .add("PlayerTypeId", mapPlayerKindToId request.kind)
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
    let param = new DynamicParameters()
    let cmd = proc("Players_GetNeutralNames", param)
    queryMany<string>(cmd, "Neutral player names")
    