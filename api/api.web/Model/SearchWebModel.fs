namespace Apex.Api.Web.Model

open System
open System.ComponentModel.DataAnnotations

[<CLIMutable>]
type GamesQueryDto = {
    gameId : Nullable<int>
    descriptionContains : string
    createdByUserName : string
    playerUserName : string
    containsMe : Nullable<bool>
    isPublic : Nullable<bool>
    allowGuests : Nullable<bool>
    statuses : List<GameStatusDto>
    createdBefore : Nullable<DateTime>
    createdAfter : Nullable<DateTime>
    lastEventBefore : Nullable<DateTime>
    lastEventAfter : Nullable<DateTime>
}

type SearchGameDto = {
    id : int

    [<Required>]
    parameters : GameParametersDto

    [<Required>]
    createdBy : CreationSourceDto
    
    status : GameStatusDto
    lastEventOn : DateTime
    playerCount : int
    containsMe : bool
    //TODO: Add current player name
}