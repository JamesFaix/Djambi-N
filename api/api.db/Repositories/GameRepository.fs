namespace Djambi.Api.Db.Repositories

open Dapper
open FSharp.Control.Tasks
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Common.Json
open Djambi.Api.Db
open Djambi.Api.Db.Mapping
open Djambi.Api.Db.Model
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model
open Djambi.Api.Db.Interfaces

type GameRepository() =
    
    let getGamesWithoutPlayers (query : GamesQuery) : Game List AsyncHttpResult =
        let param = DynamicParameters()
                        .addOption("GameId", query.gameId)
                        .addOption("DescriptionContains", query.descriptionContains)
                        .addOption("CreatedByUserName", query.createdByUserName)
                        .addOption("PlayerUserName", query.playerUserName)
                        .addOption("IsPublic", query.isPublic)
                        .addOption("AllowGuests", query.allowGuests)
                        .addOption("GameStatusId", query.status |> Option.map mapGameStatusToId)

        let cmd = proc("Games_Get", param)

        queryMany<GameSqlModel>(cmd, "Game")
        |> thenMap (List.map mapGameResponse)

    let getGameWithoutPlayers (gameId : int) : Game AsyncHttpResult =
        let param = DynamicParameters()
                        .add("GameId", gameId)
                        .add("DescriptionContains", null)
                        .add("CreatedByUserName", null)
                        .add("PlayerUserName",  null)
                        .add("IsPublic", null)
                        .add("AllowGuests", null)
                        .add("GameStatusId", null)

        let cmd = proc("Games_Get", param)

        querySingle<GameSqlModel>(cmd, "Game")
        |> thenMap mapGameResponse
        
    let getPlayersForGames (gameIds : int list) : Player List AsyncHttpResult =
        let param = DynamicParameters()
                        .add("GameIds", TableValuedParameter.int32list gameIds)
                        .add("PlayerId", null);

        let cmd = proc("Players_Get", param)

        queryMany<PlayerSqlModel>(cmd, "Player")
        |> thenMap (List.map mapPlayerResponse)

    let getPlayer (gameId : int, playerId : int) : Player AsyncHttpResult =
        let param = DynamicParameters()
                        .add("GameIds", TableValuedParameter.int32list [gameId])
                        .add("PlayerId", playerId);

        let cmd = proc("Players_Get", param)

        querySingle<PlayerSqlModel>(cmd, "Player")
        |> thenMap mapPlayerResponse
    
    member x.getCreateGameCommand (request : CreateGameRequest) : CommandDefinition =
        let param = DynamicParameters()
                        .add("RegionCount", request.parameters.regionCount)
                        .add("CreatedByUserId", request.createdByUserId)
                        .add("AllowGuests", request.parameters.allowGuests)
                        .add("IsPublic", request.parameters.isPublic)
                        .addOption("Description", request.parameters.description)
        proc("Games_Create", param)

    member x.getAddPlayerCommand (gameId : int, request : CreatePlayerRequest) : CommandDefinition =
        let param = DynamicParameters()
                        .add("GameId", gameId)
                        .add("PlayerKindId", mapPlayerKindToId request.kind)
                        .addOption("UserId", request.userId)
                        .addOption("Name", request.name)
        proc("Players_Add", param)

    member x.getAddFullPlayerCommand (player : Player) : CommandDefinition =
        let param = DynamicParameters()
                        .add("GameId", player.gameId)
                        .add("PlayerKindId", mapPlayerKindToId player.kind)
                        .addOption("UserId", player.userId)
                        .addOption("Name", if player.kind = PlayerKind.User then None else Some player.name)
                        .add("PlayerStatusId", mapPlayerStatusToId player.status)
                        .addOption("ColorId", player.colorId)
                        .addOption("StartingRegion", player.startingRegion)
                        .addOption("StartingTurnNumber", player.startingTurnNumber)
        proc("Players_Add", param)

    member x.getRemovePlayerCommand (gameId : int, playerId : int) : CommandDefinition = 
        let param = DynamicParameters()
                        .add("GameId", gameId)
                        .add("PlayerId", playerId)
        proc("Players_Remove", param)
    
    member x.getUpdateGameCommand (game : Game) : CommandDefinition =
        let param = DynamicParameters()
                        .add("GameId", game.id)
                        .addOption("Description", game.parameters.description)
                        .add("AllowGuests", game.parameters.allowGuests)
                        .add("IsPublic", game.parameters.isPublic)
                        .add("RegionCount", game.parameters.regionCount)
                        .add("GameStatusId", game.status |> mapGameStatusToId)
                        .add("PiecesJson", JsonUtility.serialize game.pieces)
                        .add("CurrentTurnJson", JsonUtility.serialize game.currentTurn)
                        .add("TurnCycleJson", JsonUtility.serialize game.turnCycle)
        proc("Games_Update", param)
    

    member x.getUpdatePlayerCommand (player : Player) : CommandDefinition =
        let param = DynamicParameters()
                        .add("GameId", player.gameId)
                        .add("PlayerId", player.id)
                        .addOption("ColorId", player.colorId)
                        .addOption("StartingTurnNumber", player.startingTurnNumber)
                        .addOption("StartingRegion", player.startingRegion)
                        .add("PlayerStatusId", player.status |> mapPlayerStatusToId)
        proc("Players_Update", param)
    
    //Exposed for test setup
    member x.updateGame(game : Game) : Unit AsyncHttpResult =
        let cmd = x.getUpdateGameCommand game
        queryUnit(cmd, "Game")

    //Exposed for test setup
    member x.updatePlayer(player : Player) : Unit AsyncHttpResult =
        let cmd = x.getUpdatePlayerCommand player
        queryUnit(cmd, "Player")

    interface IGameRepository with
        member x.getGame gameId = 
            getGameWithoutPlayers gameId
            |> thenBindAsync (fun game -> 
                getPlayersForGames [gameId]
                |> thenMap (fun players -> { game with players = players })
            )

        member x.getGames query = 
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
                                 | _ -> []
                        { g with players = ps}
                    )
                )
            )

        member x.createGame request =
            let cmd = x.getCreateGameCommand request
            querySingle<int>(cmd, "Game")

        member x.addPlayer (gameId, request) =
            let cmd = x.getAddPlayerCommand (gameId, request)
            querySingle<int>(cmd, "Player")
            |> thenBindAsync (fun pId -> getPlayer (gameId, pId))

        member x.removePlayer (gameId, playerId) =
            let cmd = x.getRemovePlayerCommand (gameId, playerId)
            queryUnit(cmd, "Player")

        member x.getNeutralPlayerNames () =
            let param = DynamicParameters()
            let cmd = proc("Players_GetNeutralNames", param)
            queryMany<string>(cmd, "Neutral player names")

        member x.createGameAndAddPlayer (gameRequest, playerRequest) =
            task {
                use conn = SqlUtility.getConnection()
                use tran = conn.BeginTransaction()
       
                try 
                    let cmd = x.getCreateGameCommand gameRequest
                              |> CommandDefinition.withTransaction tran
                    let! gameId = conn.QuerySingleAsync<int> cmd
                    let cmd = x.getAddPlayerCommand (gameId, playerRequest)
                              |> CommandDefinition.withTransaction tran
                    let! _ = conn.ExecuteAsync cmd
                    tran.Commit()
                    return Ok gameId
                with 
                | _ as ex -> return Error <| (SqlUtility.catchSqlException ex "Effect")
            }