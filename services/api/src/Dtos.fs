namespace djambi.api.Dtos

[<CLIMutable>]
type PlaceHolderMessageDto =
    {
        Text : string
    }

//Use for POST and PATCH /users
type CreateUserRequestDto =
    {
        Name : string
    }

//Use for GET /users
type UserDto =
    {
        Id : int
        Name : string
    }

//Use for POST /games
type CreateGameRequestDto =
    {
        BoardRegionCount : int    
    }

type GameStatus =
    | Open = 1
    | Started = 2
    | Complete = 3
    | Cancelled = 4

//Use for GET /games
type GameMetadataDto = 
    {
        Id : int
        Status : GameStatus
        BoardRegionCount : int
        Players : UserDto list
    }

type GameDetailsDto =
    {
        Id : int
        Status : GameStatus
        BoardRegionCount : int
        Players : UserDto list 
        Pieces: int list
        //TODO: Add details like player color, alive/dead state, etc
        //TODO: Add pieces and their positions
        //TODO: Add current turn selections
    }

type CreateSelectionDto =
    {
        Description : string
        //TODO: Add piece, location, type, etc
    }

type CreateMessageDto = 
    {
        Body : string
        //TODO: Maybe add UserId for direct messages
    }