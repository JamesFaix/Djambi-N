namespace Djambi.Api.Http

open Microsoft.AspNetCore.Http

open Giraffe

open Djambi.Api.Http.LobbyJsonModels
open Djambi.Api.Http.LobbyJsonMappings
open Djambi.Api.Persistence
open Djambi.Api.Common
open System.Threading.Tasks
open System

type LobbyController(repository : LobbyRepository) =
    
    member private this.handle<'a> (func : HttpContext -> 'a Task) :
        HttpFunc -> HttpContext -> HttpContext option Task =

        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                try 
                    let! result = func ctx
                    return! json result next ctx
                with
                | :? HttpException as ex -> 
                    ctx.SetStatusCode ex.statusCode
                    return! json ex.Message next ctx
                | _ as ex -> 
                    ctx.SetStatusCode 500
                    return! json ex.Message next ctx
            }
//Users
    member this.createUser =
        let func (ctx : HttpContext) =            
            ctx.BindJsonAsync<CreateUserJsonModel>()
            |> Task.map mapCreateUserRequest
            |> Task.bind repository.createUser
            |> Task.map mapUserResponse
        this.handle func

    member this.deleteUser(userId : int) =
        let func ctx =
            repository.deleteUser(userId)
        this.handle func

    member this.getUser(userId : int) =
        let func ctx =
            repository.getUser userId
            |> Task.map mapUserResponse
        this.handle func

    member this.getUsers =
        let func ctx =
            repository.getUsers()
            |> Task.map (Seq.map mapUserResponse)
        this.handle func

    member this.updateUser(userId : int) =
        let func ctx = 
            raise (NotImplementedException "")
        this.handle func

//Game lobby
    member this.getOpenGames =
        let func ctx =
            repository.getOpenGames()
            |> Task.map (List.map mapLobbyGameResponse)
        this.handle func
            
    member this.getGames =
        let func ctx =
            repository.getGames()
            |> Task.map (List.map mapLobbyGameResponse)
        this.handle func

    member this.createGame =
        let func (ctx : HttpContext) =
            ctx.BindModelAsync<CreateGameJsonModel>()
            |> Task.map mapCreateGameRequest
            |> Task.bind repository.createGame
            |> Task.map mapLobbyGameResponse
        this.handle func        
    
    member this.deleteGame(gameId : int) =
        let func ctx = 
            repository.deleteGame gameId
        this.handle func
 
    member this.addPlayerToGame(gameId : int, userId : int) =
        let func ctx =
            repository.addPlayerToGame(gameId, userId)
        this.handle func

    member this.removePlayerFromGame(gameId : int, userId : int) =
        let func ctx =
            repository.removePlayerFromGame(gameId, userId)
        this.handle func