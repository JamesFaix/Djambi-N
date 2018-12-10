module Djambi.Api.Db.Repositories.LobbyRepository

open System
open Dapper
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Mapping
open Djambi.Api.Db.Model
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model

let createLobby (request : CreateLobbyRequest, createdByUserId : int) : GameParameters AsyncHttpResult =
    let param = DynamicParameters()
                    .add("RegionCount", request.regionCount)
                    .add("CreatedByUserId", createdByUserId)
                    .add("AllowGuests", request.allowGuests)
                    .add("IsPublic", request.isPublic)
                    .addOption("Description", request.description)

    let cmd = proc("Lobbies_Create", param)

    let createResponse (lobbyId : int) =
        {
            id = lobbyId
            description = request.description
            regionCount = request.regionCount
            createdByUserId = createdByUserId
            createdOn = DateTime.UtcNow
            isPublic = request.isPublic
            allowGuests = request.allowGuests
            //TODO: Add player count
        }

    querySingle<int>(cmd, "Lobby")
    |> thenMap createResponse

let deleteLobby (lobbyId : int) : Unit AsyncHttpResult =
    let param = DynamicParameters()
                    .add("LobbyId", lobbyId)
    let cmd = proc("Lobbies_Delete", param)
    queryUnit(cmd, "Lobby")

let getLobbies (query : GamesQuery) : GameParameters List AsyncHttpResult =
    let param = DynamicParameters()
                    .addOption("LobbyId", query.lobbyId)
                    .addOption("DescriptionContains", query.descriptionContains)
                    .addOption("CreatedByUserId", query.createdByUserId)
                    .addOption("PlayerUserId", query.playerUserId)
                    .addOption("IsPublic", query.isPublic)
                    .addOption("AllowGuests", query.allowGuests)

    let cmd = proc("Lobbies_Get", param)

    queryMany<GameParametersSqlModel>(cmd, "Lobby")
    |> thenMap (List.map mapGameParametersResponse)

let getLobby (lobbyId : int) : GameParameters AsyncHttpResult =
    let param = DynamicParameters()
                    .add("LobbyId", lobbyId)
                    .add("DescriptionContains", null)
                    .add("CreatedByUserId", null)
                    .add("PlayerUserId",  null)
                    .add("IsPublic", null)
                    .add("AllowGuests", null)

    let cmd = proc("Lobbies_Get", param)

    querySingle<GameParametersSqlModel>(cmd, "Lobby")
    |> thenMap mapGameParametersResponse