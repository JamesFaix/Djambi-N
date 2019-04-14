namespace Djambi.Api.Logic.Managers

open Djambi.Api.Logic.Services
open Djambi.Api.Logic.Interfaces

type BoardManager(boardServ : BoardService) =
    interface IBoardManager with
        member x.getBoard regionCount session =
            boardServ.getBoard regionCount session

        member x.getCellPaths (regionCount, cellId) session =
            boardServ.getCellPaths (regionCount, cellId) session