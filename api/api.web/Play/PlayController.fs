namespace Djambi.Api.Http

open Djambi.Api.Domain
open Djambi.Api.Domain.BoardsExtensions
open Djambi.Api.Http.PlayJsonMappings
open Djambi.Api.Common
open System
open System.Threading.Tasks
open Djambi.Api.Http.HttpUtility

module PlayController =

    let startGame(gameId: int) =
        let func ctx = 
            GameStartService.startGame gameId
            |> Task.map mapGameStartResponseToJson
        handle func

//Board
    let getBoard(regionCount : int) =
        let func ctx =
            BoardUtility.getBoard regionCount
            |> Task.FromResult
        handle func
            
    let getCellPaths(regionCount : int, cellId : int) =
        let func ctx = 
            let board = BoardUtility.getBoardMetadata(regionCount)
            let cell = board.cells() |> Seq.find(fun c -> c.id = cellId)
            board.pathsFromCell(cell)
            |> List.map (List.map (fun c -> c.id))
            |> Task.FromResult
        handle func

//Gameplay
    let getGameState(gameId : int) =
        let func ctx =
            PlayService.getGameState(gameId)
            |> Task.map mapGameStateToJsonModel
        handle func

    let selectCell(gameId : int, cellId : int) =
        let func ctx = 
            PlayService.selectCell(gameId, cellId)
            |> Task.map mapTurnStateToJsonModel
        handle func

    let resetTurn(gameId : int) =
        let func ctx =
            PlayService.resetTurn gameId
            |> Task.map mapTurnStateToJsonModel
        handle func

    let commitTurn(gameId : int) =
        let func ctx =
            PlayService.commitTurn gameId
            |> Task.map  mapCommitTurnResponseToJsonModel
        handle func

    let sendMessage(gameId : int) =
        let func ctx = 
            raise (NotImplementedException "")
        handle func