namespace Djambi.Api.Domain

module LobbyModels =

    open Djambi.Api.Common.Enums

    type User = 
        {
            id : int
            name : string
        }
        
    type CreateUserRequest =
        {
            name : string
        }

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