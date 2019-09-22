[<AutoOpen>]
module Djambi.Api.Model.SearchModel

open System
open Djambi.ClientGenerator.Annotations

[<CLIMutable>]
[<ClientType(ClientSection.Search)>]
type GamesQuery =
    {
        gameId : int option
        descriptionContains : string option
        createdByUserName : string option
        playerUserName : string option
        isPublic : bool option
        allowGuests : bool option
        status : GameStatus option
    }

module GamesQuery =

    let empty : GamesQuery =
        {
            gameId = None
            descriptionContains = None
            createdByUserName = None
            playerUserName = None
            isPublic = None
            allowGuests = None
            status = None
        }

[<ClientType(ClientSection.Search)>]
type SearchGame = {
    id : int
    parameters : GameParameters
    createdBy : CreationSource
    status : GameStatus
    lastEventOn : DateTime
    currentPlayerName : string option
}