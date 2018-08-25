namespace Djambi.Api.Http

open Microsoft.AspNetCore.Http

open Giraffe

open Djambi.Api.Http.LobbyJsonModels
open Djambi.Api.Http.LobbyJsonMappings
open Djambi.Api.Persistence
open Djambi.Api.Common
open System.Threading.Tasks
open System
open Djambi.Api.Http.HttpUtility

module LobbyController =
    
//Users
    let createUser : HttpHandler =
        let func (ctx : HttpContext) =            
            ctx.BindModelAsync<CreateUserJsonModel>()
            |> Task.map mapCreateUserRequest
            |> Task.bind LobbyRepository.createUser
            |> Task.map mapUserResponse
        handle func

    let deleteUser(userId : int) =
        let func ctx =
            LobbyRepository.deleteUser(userId)
        handle func

    let getUser(userId : int) =
        let func ctx =
            LobbyRepository.getUser userId
            |> Task.map mapUserResponse
        handle func

    let getUsers : HttpFunc -> HttpContext -> HttpContext option Task =
        let func ctx =
            LobbyRepository.getUsers()
            |> Task.map (Seq.map mapUserResponse)
        handle func

    let updateUser(userId : int) =
        let func ctx = 
            raise (NotImplementedException "")
        handle func

//Game lobby
    let getOpenGames : HttpHandler =
        let func ctx =
            LobbyRepository.getOpenGames()
            |> Task.map (List.map mapLobbyGameResponse)
        handle func
            
    let getGames : HttpHandler =
        let func ctx =
            LobbyRepository.getGames()
            |> Task.map (List.map mapLobbyGameResponse)
        handle func

    let createGame : HttpHandler =
        let func (ctx : HttpContext) =
            ctx.BindModelAsync<CreateGameJsonModel>()
            |> Task.map mapCreateGameRequest
            |> Task.bind LobbyRepository.createGame
            |> Task.map mapLobbyGameResponse
        handle func        
    
    let deleteGame(gameId : int) =
        let func ctx = 
            LobbyRepository.deleteGame gameId
        handle func
 
    let addPlayerToGame(gameId : int, userId : int) =
        let func ctx =
            LobbyRepository.addPlayerToGame(gameId, userId)
        handle func

    let removePlayerFromGame(gameId : int, userId : int) =
        let func ctx =
            LobbyRepository.removePlayerFromGame(gameId, userId)
        handle func