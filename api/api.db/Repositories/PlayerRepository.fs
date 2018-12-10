module Djambi.Api.Db.Repositories.PlayerRepository

open Dapper
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Mapping
open Djambi.Api.Db.Model
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model

let private getPlayers (lobbyId : int option, gameId : int option) : Player List AsyncHttpResult =
    let param = DynamicParameters()
                    .addOption("LobbyId", lobbyId)
                    .addOption("GameId", gameId)
                    .add("PlayerId", null);

    let cmd = proc("Players_Get", param)

    queryMany<PlayerSqlModel>(cmd, "Player")
    |> thenMap (List.map mapPlayerResponse)

let getPlayersForLobby (lobbyId : int) : Player List AsyncHttpResult =
    getPlayers (Some lobbyId, None)

let getPlayersForGame (gameId : int) : Player List AsyncHttpResult =
    getPlayers (None, Some gameId)

let getPlayer (playerId : int) : Player AsyncHttpResult =
    let param = DynamicParameters()
                    .add("LobbyId", null)
                    .add("GameId", null)
                    .add("PlayerId", playerId);

    let cmd = proc("Players_Get", param)

    querySingle<PlayerSqlModel>(cmd, "Player")
    |> thenMap mapPlayerResponse

let addPlayerToLobby (lobbyId : int, request : CreatePlayerRequest) : Player AsyncHttpResult =
    let param = DynamicParameters()
                    .add("LobbyId", lobbyId)
                    .add("PlayerTypeId", mapPlayerKindToId request.kind)
                    .addOption("UserId", request.userId)
                    .addOption("Name", request.name)

    let cmd = proc("Players_Add", param)

    querySingle<int>(cmd, "Player")
    |> thenBindAsync getPlayer

let removePlayerFromLobby(playerId : int) : Unit AsyncHttpResult =
    let param = DynamicParameters()
                    .add("PlayerId", playerId)
    let cmd = proc("Players_Remove", param)
    queryUnit(cmd, "Player")

let getNeutralPlayerNames() : string list AsyncHttpResult =
    let param = new DynamicParameters()
    let cmd = proc("Players_GetVirtualNames", param)
    queryMany<string>(cmd, "Neutral player names")