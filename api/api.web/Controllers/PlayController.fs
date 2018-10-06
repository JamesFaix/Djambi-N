namespace Djambi.Api.Web.Controllers

open System
open System.Threading.Tasks
open Djambi.Api.Common
open Djambi.Api.Logic.ModelExtensions
open Djambi.Api.Logic.ModelExtensions.BoardModelExtensions
open Djambi.Api.Logic.Services
open Djambi.Api.Web.HttpUtility
open Djambi.Api.Web.Mappings.PlayWebMapping

module PlayController =

    let startGame(gameId: int) =
        let func ctx = 
            GameStartService.startGame gameId
            |> Task.map mapGameStartResponseToJson
        handle func

//Board
    let getBoard(regionCount : int) =
        let func ctx =
            BoardModelUtility.getBoard regionCount
            |> Task.FromResult
        handle func
            
    let getCellPaths(regionCount : int, cellId : int) =
        let func ctx = 
            let board = BoardModelUtility.getBoardMetadata(regionCount)
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