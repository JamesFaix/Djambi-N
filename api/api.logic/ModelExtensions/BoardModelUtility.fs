namespace Djambi.Api.Logic.ModelExtensions

open Djambi.Api.Logic.ModelExtensions.BoardModelExtensions
open Djambi.Api.Model.BoardModel

module BoardModelUtility =

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

