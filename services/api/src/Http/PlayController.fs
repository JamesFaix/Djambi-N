namespace Djambi.Api.Http

open Microsoft.AspNetCore.Http

open Giraffe

open Djambi.Api.Http.LobbyJsonModels
open Djambi.Api.Http.PlayJsonModels
open Djambi.Api.Persistence
open Djambi.Api.Domain
open Djambi.Api.Domain.BoardsExtensions
open Djambi.Api.Http.PlayJsonMappings

type PlayController(gameStartService : GameStartService, repository : PlayRepository) =

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
                let paths = board.paths(cell)
                            |> List.map (fun path -> 
                                path |> List.map (fun c -> c.id))
                return! json paths next ctx
            }

//Gameplay
    member this.getGameState(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! gameState = repository.getGameState(gameId)
                let response = gameState |> mapGameStateToJson
                return! json response next ctx
            }

    member this.makeSelection(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! request = ctx.BindJsonAsync<CreateSelectionJsonModel>()

                //let response : GameDetailsDto = {
                //    id = gameId
                //    status = GameStatus.Open
                //    boardRegionCount = 3
                //    players = List.empty
                //    pieces = List.empty
                //    selectionOptions = landscape.paths(location) |> List.collect id
                //}
                
                let placeHolderResponse = {
                    text = sprintf "Make selection %i not yet implemented" gameId
                }
                return! json placeHolderResponse next ctx
            }

    member this.resetTurn(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                //let response : GameDetailsJsonModel = {
                //    id = gameId
                //    status = GameStatus.Open
                //    boardRegionCount = 3
                //    players = List.empty
                //    pieces = List.empty
                //    selectionOptions = List.empty
                //}

                let placeHolderResponse = {
                    text = sprintf "Reset turn %i not yet implemented" gameId
                } 
                return! json placeHolderResponse next ctx
            }

    member this.commitTurn(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                //let response : GameDetailsJsonModel = {
                //    id = gameId
                //    status = GameStatus.Open
                //    boardRegionCount = 3
                //    players = List.empty
                //    pieces = List.empty
                //    selectionOptions = List.empty
                //}

                let placeHolderResponse = {
                    text = sprintf "Commit turn %i not yet implemented" gameId
                }
                return! json placeHolderResponse next ctx
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