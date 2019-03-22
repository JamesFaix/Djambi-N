namespace Djambi.Api.Db

open System
open Dapper
open Djambi.Api.Db.DapperExtensions

///<summary>
/// This module contains factory methods for SQL commands.
/// All method parameters match stored procedure parameters.
///</summary>
module Commands =
    let getUserPrivileges (userId : int option, name : string option) =
        Command.proc("Users_GetPrivileges")
            .param("UserId", userId)
            .param("Name", name)

    let getUser (userId : int option, name : string option)  =
        Command.proc("Users_Get")
            .param("UserId", userId)
            .param("Name", name)   

    let createUser (name : string, password : string) =
        Command.proc("Users_Create")
            .param("Name", name)
            .param("Password", password)
    
    let deleteUser (userId : int) =
        Command.proc("Users_Delete")
            .param("UserId", userId)

    let updateFailedLoginAttempts (userId : int, 
                                   failedLoginAttempts : int, 
                                   lastFailedLoginAttemptOn : DateTime option) =
        Command.proc("Users_UpdateFailedLoginAttempts")
            .param("UserId", userId)
            .param("FailedLoginAttempts", failedLoginAttempts)
            .param("LastFailedLoginAttemptOn", lastFailedLoginAttemptOn)
       
    let getSession (sessionId : int option,
                    token : string option,
                    userId : int option) =
        let param = DynamicParameters()
                        .addOption("SessionId", sessionId)
                        .addOption("Token", token)
                        .addOption("UserId", userId)
        proc("Sessions_Get", param)

    let createSession (userId : int, token : string, expiresOn : DateTime) =
        let param = DynamicParameters()
                        .add("UserId", userId)
                        .add("Token", token)
                        .add("ExpiresOn", expiresOn)
        proc("Sessions_Create", param)

    let renewSessionExpiration (sessionId : int, expiresOn : DateTime) =
        let param = DynamicParameters()
                        .add("SessionId", sessionId)
                        .add("ExpiresOn", expiresOn)
        proc("Sessions_Renew", param)

    let deleteSession (sessionId : int option, token : string option) =
        let param = DynamicParameters()
                        .addOption("SessionId", sessionId)
                        .addOption("Token", token)
        proc("Sessions_Delete", param)

    let getGames (gameId : int option,
                  descriptionContains : string option,
                  createdByUserName : string option,
                  playerUserName : string option,
                  isPublic : bool option,
                  allowGuests : bool option,
                  gameStatusId : byte option) =
        let param = DynamicParameters()
                        .addOption("GameId", gameId)
                        .addOption("DescriptionContains", descriptionContains)
                        .addOption("CreatedByUserName", createdByUserName)
                        .addOption("PlayerUserName", playerUserName)
                        .addOption("IsPublic", isPublic)
                        .addOption("AllowGuests", allowGuests)
                        .addOption("GameStatusId", gameStatusId)
        proc("Games_Get", param)

    let getPlayers (gameIds : Int32ListTvp, playerId : int option) =
        let param = DynamicParameters()
                        .add("GameIds", gameIds)
                        .addOption("PlayerId", playerId);
        proc("Players_Get", param)

    let createGame (regionCount : int,
                    createdByUserId : int,
                    allowGuests : bool,
                    isPublic : bool,
                    description : string option) =
        let param = DynamicParameters()
                        .add("RegionCount", regionCount)
                        .add("CreatedByUserId", createdByUserId)
                        .add("AllowGuests", allowGuests)
                        .add("IsPublic", isPublic)
                        .addOption("Description", description)
        proc("Games_Create", param)

    let addPlayer (gameId : int, 
                   playerKindId : byte, 
                   userId : int option, 
                   name : string option,
                   playerStatusId : byte,
                   colorId : int option,
                   startingRegion : int option,
                   startingTurnNumber : int option) =
        let param = DynamicParameters()
                        .add("GameId", gameId)
                        .add("PlayerKindId", playerKindId)
                        .addOption("UserId", userId)
                        .addOption("Name", name)
                        .add("PlayerStatusId", playerStatusId)
                        .addOption("ColorId", colorId)
                        .addOption("StartingRegion", startingRegion)
                        .addOption("StartingTurnNumber", startingTurnNumber)
        proc("Players_Add", param)

    let removePlayer (gameId : int, playerId : int) = 
        let param = DynamicParameters()
                        .add("GameId", gameId)
                        .add("PlayerId", playerId)
        proc("Players_Remove", param)
    
    let updateGame (gameId : int,
                    description : string option,
                    allowGuests : bool,
                    isPublic : bool,
                    regionCount : int,
                    gameStatusId : byte,
                    piecesJson : string, 
                    currentTurnJson : string,
                    turnCycleJson : string) =
        let param = DynamicParameters()
                        .add("GameId", gameId)
                        .addOption("Description", description)
                        .add("AllowGuests", allowGuests)
                        .add("IsPublic", isPublic)
                        .add("RegionCount", regionCount)
                        .add("GameStatusId", gameStatusId)
                        .add("PiecesJson", piecesJson)
                        .add("CurrentTurnJson", currentTurnJson)
                        .add("TurnCycleJson", turnCycleJson)
        proc("Games_Update", param)

    let updatePlayer (gameId : int,
                      playerId : int,
                      colorId : int option,
                      startingTurnNumber : int option,
                      startingRegion : int option,
                      playerStatusId : byte) =
        let param = DynamicParameters()
                        .add("GameId", gameId)
                        .add("PlayerId", playerId)
                        .addOption("ColorId", colorId)
                        .addOption("StartingTurnNumber", startingTurnNumber)
                        .addOption("StartingRegion", startingRegion)
                        .add("PlayerStatusId", playerStatusId)
        proc("Players_Update", param)

    let getNeutralPlayerNames () =
        proc("Players_GetNeutralNames", new DynamicParameters())

    let createEvent (gameId : int,
                     eventKindId : byte,
                     createdByUserId : int,
                     actingPlayerId : int option,
                     effectsJson : string) : CommandDefinition = 
        let param = DynamicParameters()
                        .add("GameId", gameId)
                        .add("EventKindId", eventKindId)
                        .add("CreatedByUserId", createdByUserId)
                        .addOption("ActingPlayerId", actingPlayerId)
                        .add("EffectsJson", effectsJson)
        proc("Events_Create", param)

    let getEvents (gameId : int,
                   ascending : bool,
                   maxResults : int option,
                   thresholdTime : DateTime option,
                   thresholdEventId : int option) =
        let param = DynamicParameters()
                        .add("GameId", gameId)
                        .add("Ascending", ascending)
                        .addOption("MaxResults", maxResults)
                        .addOption("ThresholdTime", thresholdTime)
                        .addOption("ThresholdEventId", thresholdEventId)
        proc("Events_Get", param)

    let getSnapshots (snapshotId : int option, gameId : int option) =
        let param = DynamicParameters()
                        .addOption("SnapshotId", snapshotId)
                        .addOption("GameId", gameId)
        proc("Snapshots_Get", param)

    let deleteSnapshot (snapshotId : int) =
        let param = DynamicParameters()
                        .add("SnapshotId", snapshotId)
        proc("Snapshots_Delete", param)

    let createSnapshot (gameId : int,
                        createdByUserId : int,
                        description : string,
                        snapshotJson : string) = 
        let param = DynamicParameters()
                        .add("GameId", gameId)
                        .add("CreatedByUserId", createdByUserId)
                        .add("Description", description)
                        .add("SnapshotJson", snapshotJson)
        proc("Snapshots_Create", param)

    let replaceEventHistory (gameId : int, history : EventListTvp) =
        let param = DynamicParameters()
                        .add("GameId", gameId)
                        .add("History", history)
        proc("Snapshots_ReplaceEventHistory", param)

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