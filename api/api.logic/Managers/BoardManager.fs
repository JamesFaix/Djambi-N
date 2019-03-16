namespace Djambi.Api.Logic.Managers

open Djambi.Api.Logic.Services
open Djambi.Api.Logic.Interfaces

type BoardManager() =
    interface IBoardManager with
        member x.getBoard regionCount session =
            BoardService.getBoard regionCount session
            
        member x.getCellPaths (regionCount, cellId) session =
            BoardService.getCellPaths (regionCount, cellId) session