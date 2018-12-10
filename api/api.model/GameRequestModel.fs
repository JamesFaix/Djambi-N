[<AutoOpen>]
module Djambi.Api.Model.GameRequestModel

[<CLIMutable>]
type CreatePlayerRequest = 
    {
        kind : PlayerKind
        userId : int option
        name : string option
    }

module CreatePlayerRequest =
    
    let user (userId : int) : CreatePlayerRequest =
        {
            kind = PlayerKind.User
            userId = Some userId
            name = None
        }

    let guest (userId : int, name : string) : CreatePlayerRequest =
        {
            kind = PlayerKind.Guest
            userId = Some userId
            name = Some name
        }

    let neutral (name : string) : CreatePlayerRequest =
        {
            kind = PlayerKind.Neutral
            userId = None
            name = Some name
        }   
        
[<CLIMutable>]
type CreateGameRequest =
    {
        description : string option
        regionCount : int
        isPublic : bool
        allowGuests : bool
    }
    
[<CLIMutable>]
type GamesQuery =
    {
        gameId : int option
        descriptionContains : string option
        createdByUserId : int option
        playerUserId : int option
        isPublic : bool option
        allowGuests : bool option
    }

module GamesQuery =

    let empty : GamesQuery =
        {
            gameId = None
            descriptionContains = None
            createdByUserId = None
            playerUserId = None
            isPublic = None
            allowGuests = None
        }

[<CLIMutable>]
type SelectionRequest =
    {
        cellId : int
    }