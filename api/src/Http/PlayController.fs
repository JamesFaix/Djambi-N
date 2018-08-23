namespace Djambi.Api.Http

open Microsoft.AspNetCore.Http

open Giraffe

open Djambi.Api.Http.LobbyJsonModels
open Djambi.Api.Http.PlayJsonModels
open Djambi.Api.Domain
open Djambi.Api.Domain.BoardsExtensions
open Djambi.Api.Http.PlayJsonMappings

type PlayController(gameStartService : GameStartService, 
                    playService : PlayService) =

    member this.startGame(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! response = gameStartService.startGame(gameId)
                let responseJson = response |> mapGameStartResponseToJson
                return! json responseJson next ctx
            }
//Board
    member this.getBoard(regionCount : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let board = BoardUtility.getBoard(regionCount)
                return! json board next ctx
            }
            
    member this.getCellPaths(regionCount : int, cellId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let board = BoardUtility.getBoardMetadata(regionCount)
                let cell = board.cells() |> Seq.find(fun c -> c.id = cellId)
                let paths = board.pathsFromCell(cell)
                            |> List.map (fun path -> 
                                path |> List.map (fun c -> c.id))
                return! json paths next ctx
            }

//Gameplay
    member this.getGameState(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! gameState = playService.getGameState(gameId)
                let response = gameState |> mapGameStateToJsonModel
                return! json response next ctx
            }

    member this.selectCell(gameId : int, cellId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! result = playService.selectCell(gameId, cellId)
                match result with 
                | Error httpError -> 
                    ctx.SetStatusCode httpError.statusCode
                    return! json httpError.message next ctx
                | Ok response ->
                    let responseJson = response |> mapTurnStateToJsonModel
                    return! json responseJson next ctx
            }

    member this.resetTurn(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! response = playService.resetTurn gameId
                let responseJson = response |> mapTurnStateToJsonModel
                return! json responseJson next ctx
            }

    member this.commitTurn(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! result = playService.commitTurn gameId

                match result with
                | Error httpError ->
                    ctx.SetStatusCode httpError.statusCode
                    return! json httpError.message next ctx
                | Ok response ->
                    let responseJson = response |> mapCommitTurnResponseToJsonModel
                    return! json responseJson next ctx
            }

    member this.sendMessage(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! request = ctx.BindModelAsync<CreateMessageJsonModel>()

                let response = {
                    text = sprintf "Send message %i not yet implemented" gameId
                }
                return! json response next ctx
            }            