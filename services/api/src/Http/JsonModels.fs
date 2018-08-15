namespace Djambi.Api

module JsonModels =

    open Djambi.Model
    open Djambi.Model.Games
    open Boards

    [<CLIMutable>]
    type PlaceHolderJsonModel =
        {
            text : string
        }

    //Use for POST and PATCH /users
    [<CLIMutable>]
    type CreateUserJsonModel =
        {
            name : string
        }

    //Use for GET /users
    [<CLIMutable>]
    type UserJsonModel =
        {
            id : int
            name : string
        }

    //Use for POST /games
    type CreateGameJsonModel =
        {
            boardRegionCount : int    
        }

    //Use for GET /games
    type GameJsonModel = 
        {
            id : int
            status : GameStatus
            boardRegionCount : int
            players : UserJsonModel list
        }

    type GameDetailsJsonModel =
        {
            id : int
            status : GameStatus
            boardRegionCount : int
            players : UserJsonModel list 
            pieces: int list
            //TODO: Add details like player color, alive/dead state, etc
            //TODO: Add pieces and their positions
            //TODO: Add current turn selections
            selectionOptions : Cell list
        }

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

    type CreateMessageJsonModel = 
        {
            body : string
            //TODO: Maybe add UserId for direct messages
        }