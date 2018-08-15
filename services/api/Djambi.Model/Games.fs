namespace Djambi.Model

module Games =
    
    open Djambi.Model.Users

    type GameStatus =
        | Open = 1
        | Started = 2
        | Complete = 3
        | Cancelled = 4

    type CreateGameRequest = 
        {
            description : string option
            boardRegionCount : int
        }

    type Player =
        {
            id : int
            userId : int option
            name : string
        }
        
    type GameMetadata = 
        {
            id : int
            status : GameStatus
            boardRegionCount : int
            description : string option
            players : Player list
        }