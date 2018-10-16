module Djambi.Api.Db.Repositories.LobbyRepository

open System
open Dapper
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Model.LobbyDbModel
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model.LobbyModel
    
//Lobbies
let createLobby (request : CreateLobbyRequest) : Lobby AsyncHttpResult =
    let param = DynamicParameters()
                    .add("RegionCount", request.regionCount)
                    .add("CreatedByUserId", request.createdByUserId)
                    .add("AllowGuests", request.allowGuests)
                    .add("IsPublic", request.isPublic)
                    .addOption("Description", request.description)

    let cmd = proc("Lobbies_Create", param)

    let createResponse (lobbyId : int) =
        {
            id = lobbyId 
            description = request.description
            regionCount = request.regionCount
            createdByUserId = request.createdByUserId
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
                        
let getLobbies (query : LobbiesQuery) : Lobby List AsyncHttpResult =
    let param = DynamicParameters()
                    .addOption("LobbyId", query.lobbyId)
                    .addOption("DescriptionContains", query.descriptionContains)
                    .addOption("CreatedByUserId", query.createdByUserId)
                    .addOption("PlayerUserId", query.playerUserId)
                    .addOption("IsPublic", query.isPublic)
                    .addOption("AllowGuests", query.allowGuests)

    let cmd = proc("Lobbies_Get", param)

    queryMany<LobbySqlModel>(cmd, "Lobby")
    |> thenMap (List.map mapLobby)

let getLobby (lobbyId : int) : Lobby AsyncHttpResult = 
    let param = DynamicParameters()
                    .add("LobbyId", lobbyId)
                    .add("DescriptionContains", null)
                    .add("CreatedByUserId", null)
                    .add("PlayerUserId",  null)
                    .add("IsPublic", null)
                    .add("AllowGuests", null)

    let cmd = proc("Lobbies_Get", param)

    querySingle<LobbySqlModel>(cmd, "Lobby")
    |> thenMap mapLobby

//Players
let getPlayers (lobbyId : int) : Player List AsyncHttpResult =
    let param = DynamicParameters()
                    .add("LobbyId", lobbyId);

    let cmd = proc("Players_Get", param)

    queryMany<PlayerSqlModel>(cmd, "Player")
    |> thenMap (List.map mapPlayer)
       
let addPlayerToLobby (request : CreatePlayerRequest) : int AsyncHttpResult =
    let param = DynamicParameters()
                    .add("LobbyId", request.lobbyId)
                    .add("PlayerTypeId", mapPlayerTypeToId request.playerType)
                    .addOption("UserId", request.userId)
                    .addOption("Name", request.name)
        
    let cmd = proc("Players_Add", param)

    querySingle<int>(cmd, "Player")
    
let removePlayerFromLobby(playerId : int) : Unit AsyncHttpResult =
    let param = DynamicParameters()
                    .add("PlayerId", playerId)
    let cmd = proc("Players_Remove", param)
    queryUnit(cmd, "Player")

let getVirtualPlayerNames() : string list AsyncHttpResult =
    let param = new DynamicParameters()
    let cmd = proc("Players_GetVirtualNames", param)
    queryMany<string>(cmd, "Virtual player names")
    
let getLobbyWithPlayers (lobbyId : int) : LobbyWithPlayers AsyncHttpResult =
    getLobby lobbyId
    |> thenBindAsync (fun lobby -> 
        getPlayers lobbyId
        |> thenMap (fun players -> 
            {
                id = lobby.id
                description = lobby.description
                regionCount = lobby.regionCount
                allowGuests = lobby.allowGuests
                isPublic = lobby.isPublic
                createdByUserId = lobby.createdByUserId
                createdOn = lobby.createdOn
                players = players
            }
        )
    )