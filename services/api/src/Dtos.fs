namespace djambi.api.Dtos

open Djambi.Model

open BoardGeometry

[<CLIMutable>]
type PlaceHolderMessageDto =
    {
        text : string
    }

//Use for POST and PATCH /users
type CreateUserRequestDto =
    {
        name : string
    }

//Use for GET /users
type UserDto =
    {
        id : int
        name : string
    }

//Use for POST /games
type CreateGameRequestDto =
    {
        boardRegionCount : int    
    }

type GameStatus =
    | Open = 1
    | Started = 2
    | Complete = 3
    | Cancelled = 4

//Use for GET /games
type GameMetadataDto = 
    {
        id : int
        status : GameStatus
        boardRegionCount : int
        players : UserDto list
    }

type GameDetailsDto =
    {
        id : int
        status : GameStatus
        boardRegionCount : int
        players : UserDto list 
        pieces: int list
        //TODO: Add details like player color, alive/dead state, etc
        //TODO: Add pieces and their positions
        //TODO: Add current turn selections
        selectionOptions : Cell list
    }

[<CLIMutable>]
type LocationDto =
    {
        region : int
        x : int
        y : int
    }

[<CLIMutable>]
type CreateSelectionDto =
    {
        location : LocationDto
    }

type CreateMessageDto = 
    {
        body : string
        //TODO: Maybe add UserId for direct messages
    }