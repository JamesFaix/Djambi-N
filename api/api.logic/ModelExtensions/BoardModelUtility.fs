namespace Apex.Api.Logic.ModelExtensions

open Apex.Api.Logic.ModelExtensions.BoardModelExtensions
open Apex.Api.Model

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

