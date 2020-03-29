namespace Apex.Api.Db

open System
open Apex.Api.Db.Command
open Apex.Api.Db.Model
open System.ComponentModel
open Apex.Api.Enums

///<summary>
/// This module contains factory methods for SQL commands.
/// All method parameters match stored procedure parameters.
///</summary>
module Commands =

    ///--- Users ---

    let getUserPrivileges (userId : int option, name : string option) =
        proc("Users_GetPrivileges")
            .forEntity("Privilege")
            .param("UserId", userId)
            .param("Name", name)
            .returnsMany<Privilege>()

    let getUser (userId : int option, name : string option) =
        proc("Users_Get")
            .forEntity("User")
            .param("UserId", userId)
            .param("Name", name)
            .returnsSingle<UserSqlModel>()

    let createUser (name : string, password : string) =
        proc("Users_Create")
            .forEntity("User")
            .param("Name", name)
            .param("Password", password)
            .returnsSingle<int>()

    let deleteUser (userId : int) =
        proc("Users_Delete")
            .forEntity("User")
            .param("UserId", userId)
            .returnsNothing()

    let updateFailedLoginAttempts (userId : int,
                                   failedLoginAttempts : int,
                                   lastFailedLoginAttemptOn : DateTime option) =
        proc("Users_UpdateFailedLoginAttempts")
            .forEntity("User")
            .param("UserId", userId)
            .param("FailedLoginAttempts", failedLoginAttempts)
            .param("LastFailedLoginAttemptOn", lastFailedLoginAttemptOn)
            .returnsNothing()

    //--- Sessions ---

    let getSession (sessionId : int option,
                    token : string option,
                    userId : int option) =
        proc("Sessions_Get")
            .forEntity("Session")
            .param("SessionId", sessionId)
            .param("Token", token)
            .param("UserId", userId)
            .returnsSingle<SessionSqlModel>()

    let createSession (userId : int, token : string, expiresOn : DateTime) =
        proc("Sessions_Create")
            .forEntity("Session")
            .param("UserId", userId)
            .param("Token", token)
            .param("ExpiresOn", expiresOn)
            .returnsSingle<int>()

    let renewSessionExpiration (sessionId : int, expiresOn : DateTime) =
        proc("Sessions_Renew")
            .forEntity("Session")
            .param("SessionId", sessionId)
            .param("ExpiresOn", expiresOn)
            .returnsNothing()

    let deleteSession (sessionId : int option, token : string option) =
        proc("Sessions_Delete")
            .forEntity("Session")
            .param("SessionId", sessionId)
            .param("Token", token)
            .returnsNothing()

    //--- Games & Players ---

    let getGame (gameId : int) =
        proc("Games_Get")
            .forEntity("Game")
            .param("GameId", gameId)
            .returnsSingle<GameSqlModel>()

    let getPlayers (gameId : int, playerId : int option) =
        proc("Players_Get")
            .forEntity("Player")
            .param("GameId", gameId)
            .param("PlayerId", playerId)
            //Open for Commands2 to close

    let createGame (regionCount : int,
                    createdByUserId : int,
                    allowGuests : bool,
                    isPublic : bool,
                    description : string option) =
        proc("Games_Create")
            .forEntity("Game")
            .param("RegionCount", regionCount)
            .param("CreatedByUserId", createdByUserId)
            .param("AllowGuests", allowGuests)
            .param("IsPublic", isPublic)
            .param("Description", description)
            .returnsSingle<int>()

    let addPlayer (gameId : int,
                   playerKindId : PlayerKind,
                   userId : int option,
                   name : string option,
                   playerStatusId : PlayerStatus,
                   colorId : int option,
                   startingRegion : int option,
                   startingTurnNumber : int option) =
        proc("Players_Add")
            .forEntity("Player")
            .param("GameId", gameId)
            .param("PlayerKindId", playerKindId)
            .param("UserId", userId)
            .param("Name", name)
            .param("PlayerStatusId", playerStatusId)
            .param("ColorId", colorId)
            .param("StartingRegion", startingRegion)
            .param("StartingTurnNumber", startingTurnNumber)
            .returnsSingle<int>()

    let removePlayer (gameId : int, playerId : int) =
        proc("Players_Remove")
            .forEntity("Player")
            .param("GameId", gameId)
            .param("PlayerId", playerId)
            .returnsNothing()

    let updateGame (gameId : int,
                    description : string option,
                    allowGuests : bool,
                    isPublic : bool,
                    regionCount : int,
                    gameStatusId : GameStatus,
                    piecesJson : string,
                    currentTurnJson : string,
                    turnCycleJson : string) =
        proc("Games_Update")
            .forEntity("Game")
            .param("GameId", gameId)
            .param("Description", description)
            .param("AllowGuests", allowGuests)
            .param("IsPublic", isPublic)
            .param("RegionCount", regionCount)
            .param("GameStatusId", gameStatusId)
            .param("PiecesJson", piecesJson)
            .param("CurrentTurnJson", currentTurnJson)
            .param("TurnCycleJson", turnCycleJson)
            .returnsNothing()

    let updatePlayer (gameId : int,
                      playerId : int,
                      colorId : int option,
                      startingTurnNumber : int option,
                      startingRegion : int option,
                      playerStatusId : PlayerStatus) =
        proc("Players_Update")
            .forEntity("Player")
            .param("GameId", gameId)
            .param("PlayerId", playerId)
            .param("ColorId", colorId)
            .param("StartingTurnNumber", startingTurnNumber)
            .param("StartingRegion", startingRegion)
            .param("PlayerStatusId", playerStatusId)
            .returnsNothing()

    let getNeutralPlayerNames () =
        proc("Players_GetNeutralNames")
            .forEntity("Neutral player names")
            .returnsMany<string>()

    //--- Search ---

    let searchGames(currentUserId : int,
                    descriptionContains : string option,
                    createdByUserName : string option,
                    playerUserName : string option,
                    containsMe : bool option,
                    isPublic : bool option,
                    allowGuests : bool option,
                    gameStatusIds : Int32ListTvp,
                    createdBefore : DateTime option,
                    createdAfter : DateTime option,
                    lastEventBefore : DateTime option,
                    lastEventAfter : DateTime option) =
        proc("Games_Search")
            .forEntity("Game")
            .param("CurrentUserId", currentUserId)
            .param("DescriptionContains", descriptionContains)
            .param("CreatedByUserName", createdByUserName)
            .param("PlayerUserName", playerUserName)
            .param("ContainsMe", containsMe)
            .param("IsPublic", isPublic)
            .param("AllowGuests", allowGuests)
            .param("GameStatusIds", gameStatusIds)
            .param("CreatedBefore", createdBefore)
            .param("CreatedAfter", createdAfter)
            .param("LastEventBefore", lastEventBefore)
            .param("LastEventAfter", lastEventAfter)
            .returnsMany<SearchGameSqlModel>()

    //--- Events ---

    let createEvent (gameId : int,
                     eventKindId : EventKind,
                     createdByUserId : int,
                     actingPlayerId : int option,
                     effectsJson : string) =
        proc("Events_Create")
            .forEntity("Event")
            .param("GameId", gameId)
            .param("EventKindId", eventKindId)
            .param("CreatedByUserId", createdByUserId)
            .param("ActingPlayerId", actingPlayerId)
            .param("EffectsJson", effectsJson)
            .returnsSingle<int>()

    let getEvents (eventId : int option,
                   gameId : int,
                   ascending : bool,
                   maxResults : int option,
                   thresholdTime : DateTime option,
                   thresholdEventId : int option) =
        proc("Events_Get")
            .forEntity("Event")
            .param("EventId", eventId)
            .param("GameId", gameId)
            .param("Ascending", ascending)
            .param("MaxResults", maxResults)
            .param("ThresholdTime", thresholdTime)
            .param("ThresholdEventId", thresholdEventId)

    //--- Snapshots ---

    let getSnapshots (snapshotId : int option, gameId : int option) =
        proc("Snapshots_Get")
            .forEntity("Snapshot")
            .param("SnapshotId", snapshotId)
            .param("GameId", gameId)
            //Open for Commands2 to close

    let deleteSnapshot (snapshotId : int) =
        proc("Snapshots_Delete")
            .forEntity("Snapshot")
            .param("SnapshotId", snapshotId)
            .returnsNothing()

    let createSnapshot (gameId : int,
                        createdByUserId : int,
                        description : string,
                        snapshotJson : string) =
        proc("Snapshots_Create")
            .forEntity("Snapshot")
            .param("GameId", gameId)
            .param("CreatedByUserId", createdByUserId)
            .param("Description", description)
            .param("SnapshotJson", snapshotJson)
            .returnsSingle<int>()

    let replaceEventHistory (gameId : int, history : EventListTvp) =
        proc("Snapshots_ReplaceEventHistory")
            .forEntity("Snapshot")
            .param("GameId", gameId)
            .param("History", history)
            .returnsNothing()

///<summary>
/// This module contains convenience methods for factory methods in <c>Commands</c>.
/// Method parameters may be domain-layer request objects that map to multiple stored procedure
/// parameters. Methods may also include serialization or marshalling logic.
///</summary>
module Commands2 =

    open Apex.Api.Common.Json.JsonUtility
    open Apex.Api.Db.Mapping
    open Apex.Api.Model

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

    let searchGames (query : GamesQuery, currentUserId : int) =
        Commands.searchGames (currentUserId,
                              query.descriptionContains,
                              query.createdByUserName,
                              query.playerUserName,
                              query.containsMe,
                              query.isPublic,
                              query.allowGuests,
                              Int32ListTvp(query.statuses |> List.map int),
                              query.createdBefore,
                              query.createdAfter,
                              query.lastEventBefore,
                              query.lastEventAfter)

    let getPlayersForGame (gameId : int) =
        let cmd = Commands.getPlayers (gameId, None)
        cmd.returnsMany<PlayerSqlModel>()

    let getPlayer (gameId : int, playerId : int) =
        let cmd = Commands.getPlayers (gameId, Some playerId)
        cmd.returnsSingle<PlayerSqlModel>()

    let createGame (request : CreateGameRequest) =
        Commands.createGame (request.parameters.regionCount,
                             request.createdByUserId,
                             request.parameters.allowGuests,
                             request.parameters.isPublic,
                             request.parameters.description)

    let addPendingPlayer (gameId : int, request : CreatePlayerRequest) =
        Commands.addPlayer (gameId,
                            request.kind,
                            request.userId,
                            request.name,
                            PlayerStatus.Pending,
                            None,
                            None,
                            None)

    let addFullPlayer (player : Player) =
        Commands.addPlayer (player.gameId,
                            player.kind,
                            player.userId,
                            (if player.kind = PlayerKind.User then None else Some player.name),
                            player.status,
                            player.colorId,
                            player.startingRegion,
                            player.startingTurnNumber)

    let updateGame (game : Game) =
        Commands.updateGame (game.id,
                             game.parameters.description,
                             game.parameters.allowGuests,
                             game.parameters.isPublic,
                             game.parameters.regionCount,
                             game.status,
                             serialize game.pieces,
                             serialize game.currentTurn,
                             serialize game.turnCycle)

    let updatePlayer (player : Player) =
        Commands.updatePlayer (player.gameId,
                               player.id,
                               player.colorId,
                               player.startingTurnNumber,
                               player.startingRegion,
                               player.status)

    let createEvent (gameId : int, request : CreateEventRequest) =
        Commands.createEvent (gameId,
                              request.kind,
                              request.createdByUserId,
                              request.actingPlayerId,
                              serialize request.effects)

    let getEvents (gameId : int, query : EventsQuery) =
        (Commands.getEvents (None,
                            gameId,
                            mapResultsDirectionToAscendingBool query.direction,
                            query.maxResults,
                            query.thresholdTime,
                            query.thresholdEventId))
            .returnsMany<EventSqlModel>()

    let getEvent (gameId : int, eventId : int) =
        (Commands.getEvents (Some eventId,
                            gameId,
                            mapResultsDirectionToAscendingBool ListSortDirection.Ascending,
                            None,
                            None,
                            None))
            .returnsSingle<EventSqlModel>()

    let getSnapshots (gameId : int) =
        Commands.getSnapshots(None, Some gameId)
            .returnsMany<SnapshotSqlModel>()

    let getSnapshot (snapshotId : int) =
        Commands.getSnapshots(Some snapshotId, None)
            .returnsSingle<SnapshotSqlModel>()

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