namespace Djambi.Api.Http

open Microsoft.AspNetCore.Http

open Giraffe

open Djambi.Api.Domain
open Djambi.Api.Domain.BoardsExtensions
open Djambi.Api.Http.PlayJsonMappings
open Djambi.Api.Common
open System
open System.Threading.Tasks

type PlayController(gameStartService : GameStartService, 
                    playService : PlayService) =

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

    member this.startGame(gameId: int) =
        let func ctx = 
            gameStartService.startGame gameId
            |> Task.map mapGameStartResponseToJson
        this.handle func

//Board
    member this.getBoard(regionCount : int) =
        let func ctx =
            BoardUtility.getBoard regionCount
            |> Task.FromResult
        this.handle func
            
    member this.getCellPaths(regionCount : int, cellId : int) =
        let func ctx = 
            let board = BoardUtility.getBoardMetadata(regionCount)
            let cell = board.cells() |> Seq.find(fun c -> c.id = cellId)
            board.pathsFromCell(cell)
            |> List.map (List.map (fun c -> c.id))
            |> Task.FromResult
        this.handle func

//Gameplay
    member this.getGameState(gameId : int) =
        let func ctx =
            playService.getGameState(gameId)
            |> Task.map mapGameStateToJsonModel
        this.handle func

    member this.selectCell(gameId : int, cellId : int) =
        let func ctx = 
            playService.selectCell(gameId, cellId)
            |> Task.map mapTurnStateToJsonModel
        this.handle func

    member this.resetTurn(gameId : int) =
        let func ctx =
            playService.resetTurn gameId
            |> Task.map mapTurnStateToJsonModel
        this.handle func

    member this.commitTurn(gameId : int) =
        let func ctx =
            playService.commitTurn gameId
            |> Task.map  mapCommitTurnResponseToJsonModel
        this.handle func

    member this.sendMessage(gameId : int) =
        let func ctx = 
            raise (NotImplementedException "")
        this.handle func