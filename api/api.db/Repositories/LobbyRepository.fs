namespace Djambi.Api.Db.Repositories

open Dapper
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Common.Enums
open Djambi.Api.Db.Mappings.LobbyDbMapping
open Djambi.Api.Db.Model.LobbyDbModel
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model.LobbyModel
    
module LobbyRepository =

//Games
    let createGame(request : CreateGameRequest) : LobbyGameMetadata AsyncHttpResult =
        let param = DynamicParameters()
                        .add("BoardRegionCount", request.boardRegionCount)
                        .addOption("Description", request.description)

        let cmd = proc("Lobby.CreateGame", param)

        let mapGame (gameId : int) =
            {
                id = gameId 
                description = request.description
                status = GameStatus.Open
                boardRegionCount = request.boardRegionCount
                players = List.empty
            }

        querySingle<int>(cmd, "Game")
        |> thenMap mapGame
        
    let deleteGame(gameId : int) : Unit AsyncHttpResult =
        let param = DynamicParameters()
                        .add("GameId", gameId)
        let cmd = proc("Lobby.DeleteGame", param)
        queryUnit(cmd, "Game")

    let private getGamesInner(gameId : int option, userId : int option, status : GameStatus option) : LobbyGameMetadata list AsyncHttpResult =
        let param = DynamicParameters()
                        .addOption("GameId", gameId)
                        .addOption("UserId", userId)
                        .addOption("StatusId", status |> Option.map mapGameStatusToId)
        let cmd = proc("Lobby.GetGamesWithPlayers", param)

        queryMany<LobbyGamePlayerSqlModel>(cmd, "Game")
        |> thenMap mapLobbyGamesResponse
        
    let getGame(gameId : int) : LobbyGameMetadata AsyncHttpResult =
        getGamesInner(Some gameId, None, None)
        |> thenMap List.head

    let getGames() : LobbyGameMetadata list AsyncHttpResult =
        getGamesInner(None, None, None)

    let getOpenGames() : LobbyGameMetadata list AsyncHttpResult =
        getGamesInner(None, None, Some Open)

    let getUserGames(userId : int) : LobbyGameMetadata list AsyncHttpResult =
        getGamesInner(None, Some userId, None)

//Players
    let addPlayerToGame(gameId : int, userId : int) : Unit AsyncHttpResult =
        let param = DynamicParameters()
                        .add("GameId", gameId)
                        .add("UserId", userId)
        
        let cmd = proc("Lobby.AddPlayerToGame", param)

        querySingle<int>(cmd, "Player")
        |> thenMap ignore  //Id not currently used

    let addVirtualPlayerToGame(gameId : int, name : string) : Unit AsyncHttpResult =
        let param = DynamicParameters()
                        .add("GameId", gameId)
                        .add("Name", name)

        let cmd = proc("Lobby.AddVirtualPlayerToGame", param)
                
        querySingle<int>(cmd, "Player")
        |> thenMap ignore  //Id not currently used

    let removePlayerFromGame(gameId : int, userId : int) : Unit AsyncHttpResult =
        let param = DynamicParameters()
                        .add("GameId", gameId)
                        .add("UserId", userId)
        let cmd = proc("Lobby.RemovePlayerFromGame", param)
        queryUnit(cmd, "Player")

    let getVirtualPlayerNames() : string list AsyncHttpResult =
        let param = new DynamicParameters()
        let cmd = proc("Lobby.GetVirtualPlayerNames", param)
        queryMany<string>(cmd, "Virtual player names")