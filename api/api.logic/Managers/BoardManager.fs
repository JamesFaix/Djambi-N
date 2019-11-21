namespace Apex.Api.Logic.Managers

open Apex.Api.Logic.Services
open Apex.Api.Logic.Interfaces

type BoardManager(boardServ : BoardService) =
    interface IBoardManager with
        member x.getBoard regionCount session =
            boardServ.getBoard regionCount session

        member x.getCellPaths (regionCount, cellId) session =
            boardServ.getCellPaths (regionCount, cellId) session