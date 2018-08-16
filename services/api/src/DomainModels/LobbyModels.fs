namespace Djambi.Api.Domain

module LobbyModels =

    open Djambi.Api.Common.Enums

    type User = 
        {
            id : int
            name : string
        }
        
    type LobbyPlayer =
        {
            id : int
            userId : int option
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
                
    type LobbyGameMetadata = 
        {
            id : int
            status : GameStatus
            boardRegionCount : int
            description : string option
            players : LobbyPlayer list
        }

    type UpdateGameRequest =
        {
            id : int
            status : GameStatus
            description : string option
        }