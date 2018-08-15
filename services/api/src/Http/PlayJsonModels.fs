namespace Djambi.Api.Http

module PlayJsonModels =

    open Djambi.Api.Common.Enums

    [<CLIMutable>]
    type LocationJsonModel =
        {
            region : int
            x : int
            y : int
        }

    [<CLIMutable>]
    type CreateSelectionJsonModel =
        {
            cellId : int
        }

    [<CLIMutable>]
    type CreateMessageJsonModel = 
        {
            body : string
            //TODO: Maybe add UserId for direct messages
        }

    type PlayerJsonModel =
        {
            id : int
            userId : int option
            name : string
        }

    type CellJsonModel =
        {
            id : int
            locations : LocationJsonModel list
        }

    type GameDetailsJsonModel =
        {
            id : int
            status : GameStatus
            boardRegionCount : int
            players : PlayerJsonModel list 
            pieces: int list
            //TODO: Add details like player color, alive/dead state, etc
            //TODO: Add pieces and their positions
            //TODO: Add current turn selections
            selectionOptions : CellJsonModel list
        }