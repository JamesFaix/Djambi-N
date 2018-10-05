namespace Djambi.Api.Domain

module BoardUtility =

    open Djambi.Api.Model.Board
    open BoardsExtensions

    let getBoardMetadata(regionCount : int) : BoardMetadata =
        let standardRegionSize = 5
        {
            regionCount = regionCount
            regionSize = standardRegionSize
        }

    let getBoard(regionCount : int) : Board =
        let metadata = getBoardMetadata(regionCount)
        {
            regionCount = metadata.regionCount
            regionSize = metadata.regionSize
            cells = metadata.cells()
        }

