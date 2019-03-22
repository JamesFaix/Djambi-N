namespace Djambi.Api.Db

open System
open Djambi.Api.Db.Command

///<summary>
/// This module contains factory methods for SQL commands.
/// All method parameters match stored procedure parameters.
///</summary>
module Commands =
    let getUserPrivileges (userId : int option, name : string option) =
        proc("Users_GetPrivileges")
            .param("UserId", userId)
            .param("Name", name)

    let getUser (userId : int option, name : string option)  =
        proc("Users_Get")
            .param("UserId", userId)
            .param("Name", name)   

    let createUser (name : string, password : string) =
        proc("Users_Create")
            .param("Name", name)
            .param("Password", password)
    
    let deleteUser (userId : int) =
        proc("Users_Delete")
            .param("UserId", userId)

    let updateFailedLoginAttempts (userId : int, 
                                   failedLoginAttempts : int, 
                                   lastFailedLoginAttemptOn : DateTime option) =
        proc("Users_UpdateFailedLoginAttempts")
            .param("UserId", userId)
            .param("FailedLoginAttempts", failedLoginAttempts)
            .param("LastFailedLoginAttemptOn", lastFailedLoginAttemptOn)
       
    let getSession (sessionId : int option,
                    token : string option,
                    userId : int option) =
        proc("Sessions_Get")
            .param("SessionId", sessionId)
            .param("Token", token)
            .param("UserId", userId)

    let createSession (userId : int, token : string, expiresOn : DateTime) =
        proc("Sessions_Create")
            .param("UserId", userId)
            .param("Token", token)
            .param("ExpiresOn", expiresOn)

    let renewSessionExpiration (sessionId : int, expiresOn : DateTime) =
        proc("Sessions_Renew")
            .param("SessionId", sessionId)
            .param("ExpiresOn", expiresOn)

    let deleteSession (sessionId : int option, token : string option) =
        proc("Sessions_Delete")
            .param("SessionId", sessionId)
            .param("Token", token)

    let getGames (gameId : int option,
                  descriptionContains : string option,
                  createdByUserName : string option,
                  playerUserName : string option,
                  isPublic : bool option,
                  allowGuests : bool option,
                  gameStatusId : byte option) =
        proc("Games_Get")
            .param("GameId", gameId)
            .param("DescriptionContains", descriptionContains)
            .param("CreatedByUserName", createdByUserName)
            .param("PlayerUserName", playerUserName)
            .param("IsPublic", isPublic)
            .param("AllowGuests", allowGuests)
            .param("GameStatusId", gameStatusId)

    let getPlayers (gameIds : Int32ListTvp, playerId : int option) =
        proc("Players_Get")
            .param("GameIds", gameIds)
            .param("PlayerId", playerId);

    let createGame (regionCount : int,
                    createdByUserId : int,
                    allowGuests : bool,
                    isPublic : bool,
                    description : string option) =
        proc("Games_Create")
            .param("RegionCount", regionCount)
            .param("CreatedByUserId", createdByUserId)
            .param("AllowGuests", allowGuests)
            .param("IsPublic", isPublic)
            .param("Description", description)

    let addPlayer (gameId : int, 
                   playerKindId : byte, 
                   userId : int option, 
                   name : string option,
                   playerStatusId : byte,
                   colorId : int option,
                   startingRegion : int option,
                   startingTurnNumber : int option) =
        proc("Players_Add")
            .param("GameId", gameId)
            .param("PlayerKindId", playerKindId)
            .param("UserId", userId)
            .param("Name", name)
            .param("PlayerStatusId", playerStatusId)
            .param("ColorId", colorId)
            .param("StartingRegion", startingRegion)
            .param("StartingTurnNumber", startingTurnNumber)

    let removePlayer (gameId : int, playerId : int) = 
        proc("Players_Remove")
            .param("GameId", gameId)
            .param("PlayerId", playerId)
    
    let updateGame (gameId : int,
                    description : string option,
                    allowGuests : bool,
                    isPublic : bool,
                    regionCount : int,
                    gameStatusId : byte,
                    piecesJson : string, 
                    currentTurnJson : string,
                    turnCycleJson : string) =
        proc("Games_Update")
            .param("GameId", gameId)
            .param("Description", description)
            .param("AllowGuests", allowGuests)
            .param("IsPublic", isPublic)
            .param("RegionCount", regionCount)
            .param("GameStatusId", gameStatusId)
            .param("PiecesJson", piecesJson)
            .param("CurrentTurnJson", currentTurnJson)
            .param("TurnCycleJson", turnCycleJson)

    let updatePlayer (gameId : int,
                      playerId : int,
                      colorId : int option,
                      startingTurnNumber : int option,
                      startingRegion : int option,
                      playerStatusId : byte) =
        proc("Players_Update")
            .param("GameId", gameId)
            .param("PlayerId", playerId)
            .param("ColorId", colorId)
            .param("StartingTurnNumber", startingTurnNumber)
            .param("StartingRegion", startingRegion)
            .param("PlayerStatusId", playerStatusId)

    let getNeutralPlayerNames () =
        proc("Players_GetNeutralNames")

    let createEvent (gameId : int,
                     eventKindId : byte,
                     createdByUserId : int,
                     actingPlayerId : int option,
                     effectsJson : string) = 
        proc("Events_Create")
            .param("GameId", gameId)
            .param("EventKindId", eventKindId)
            .param("CreatedByUserId", createdByUserId)
            .param("ActingPlayerId", actingPlayerId)
            .param("EffectsJson", effectsJson)

    let getEvents (gameId : int,
                   ascending : bool,
                   maxResults : int option,
                   thresholdTime : DateTime option,
                   thresholdEventId : int option) =
        proc("Events_Get")
            .param("GameId", gameId)
            .param("Ascending", ascending)
            .param("MaxResults", maxResults)
            .param("ThresholdTime", thresholdTime)
            .param("ThresholdEventId", thresholdEventId)

    let getSnapshots (snapshotId : int option, gameId : int option) =
        proc("Snapshots_Get")
            .param("SnapshotId", snapshotId)
            .param("GameId", gameId)

    let deleteSnapshot (snapshotId : int) =
        proc("Snapshots_Delete")
            .param("SnapshotId", snapshotId)

    let createSnapshot (gameId : int,
                        createdByUserId : int,
                        description : string,
                        snapshotJson : string) = 
        proc("Snapshots_Create")
            .param("GameId", gameId)
            .param("CreatedByUserId", createdByUserId)
            .param("Description", description)
            .param("SnapshotJson", snapshotJson)

    let replaceEventHistory (gameId : int, history : EventListTvp) =
        proc("Snapshots_ReplaceEventHistory")
            .param("GameId", gameId)
            .param("History", history)

///<summary>
/// This module contains convenience methods for factory methods in <c>Commands</c>.
/// Method parameters may be domain-layer request objects that map to multiple stored procedure
/// parameters. Methods may also include serialization or marshalling logic.
///</summary>
module Commands2 =

    open Djambi.Api.Common.Json.JsonUtility
    open Djambi.Api.Db.Mapping
    open Djambi.Api.Db.Model
    open Djambi.Api.Model

    let createUser (request : CreateUserRequest) =
        Commands.createUser (request.name, request.password)

    let updateFailedLoginAttempts (request : UpdateFailedLoginsRequest) =
        Commands.updateFailedLoginAttempts (request.userId, 
                                            request.failedLoginAttempts,
                                            request.lastFailedLoginAttemptOn)

    let getSession (query : SessionQuery) =
        Commands.getSession (query.sessionId, query.token, query.userId)
            
    let createSession (request : CreateSessionRequest) =
        Commands.createSession (request.userId, request.token, request.expiresOn)

    let getGames (query : GamesQuery) =
        Commands.getGames (query.gameId,
                           query.descriptionContains,
                           query.createdByUserName,
                           query.playerUserName,
                           query.isPublic,
                           query.allowGuests,
                           query.status |> Option.map mapGameStatusToId)

    let getGame (gameId : int) =
        Commands.getGames (Some gameId, None, None, None, None, None, None)

    let getPlayers (gameIds : int list) =
        Commands.getPlayers (Int32ListTvp(gameIds), None)

    let getPlayer (gameId : int, playerId : int) =
        Commands.getPlayers (Int32ListTvp([gameId]), Some playerId)

    let createGame (request : CreateGameRequest) =
        Commands.createGame (request.parameters.regionCount,
                             request.createdByUserId,
                             request.parameters.allowGuests,
                             request.parameters.isPublic,
                             request.parameters.description)

    let addPendingPlayer (gameId : int, request : CreatePlayerRequest) =
        Commands.addPlayer (gameId, 
                            mapPlayerKindToId request.kind,
                            request.userId,
                            request.name,
                            mapPlayerStatusToId PlayerStatus.Pending,
                            None,
                            None,
                            None)

    let addFullPlayer (player : Player) =
        Commands.addPlayer (player.gameId,
                            mapPlayerKindToId player.kind,
                            player.userId,
                            (if player.kind = PlayerKind.User then None else Some player.name),
                            mapPlayerStatusToId player.status,
                            player.colorId,
                            player.startingRegion, 
                            player.startingTurnNumber)

    let updateGame (game : Game) =
        Commands.updateGame (game.id,
                             game.parameters.description,
                             game.parameters.allowGuests,
                             game.parameters.isPublic,
                             game.parameters.regionCount,
                             mapGameStatusToId game.status,
                             serialize game.pieces,
                             serialize game.currentTurn,
                             serialize game.turnCycle)

    let updatePlayer (player : Player) =
        Commands.updatePlayer (player.gameId,
                               player.id,
                               player.colorId,
                               player.startingTurnNumber,
                               player.startingRegion,
                               mapPlayerStatusToId player.status)

    let createEvent (gameId : int, request : CreateEventRequest) =
        Commands.createEvent (gameId,
                              mapEventKindToId request.kind,
                              request.createdByUserId,
                              request.actingPlayerId,
                              serialize request.effects)

    let getEvents (gameId : int, query : EventsQuery) =
        Commands.getEvents (gameId,
                            mapResultsDirectionToAscendingBool query.direction,
                            query.maxResults,
                            query.thresholdTime,
                            query.thresholdEventId)

    let createSnapshot (request : InternalCreateSnapshotRequest) =
        let jsonModel = 
            {
                game = request.game
                history = request.history
            }
        Commands.createSnapshot (request.game.id, 
                                 request.createdByUserId,
                                 request.description,
                                 serialize jsonModel)

    let replaceEventHistory (gameId : int, history : Event list) =
        Commands.replaceEventHistory (gameId, EventListTvp(history))