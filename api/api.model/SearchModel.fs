[<AutoOpen>]
module Apex.Api.Model.SearchModel

open System
open Apex.Api.Enums

type GamesQuery =
    {
        gameId : int option
        descriptionContains : string option
        createdByUserName : string option
        playerUserName : string option
        containsMe : bool option
        isPublic : bool option
        allowGuests : bool option
        statuses : GameStatus list
        createdBefore : DateTime option
        createdAfter : DateTime option
        lastEventBefore : DateTime option
        lastEventAfter : DateTime option
    }

module GamesQuery =

    let empty : GamesQuery =
        {
            gameId = None
            descriptionContains = None
            createdByUserName = None
            playerUserName = None
            containsMe = None
            isPublic = None
            allowGuests = None
            statuses = []
            createdBefore = None
            createdAfter = None
            lastEventBefore = None
            lastEventAfter = None
        }

type SearchGame = {
    id : int
    parameters : GameParameters
    createdBy : CreationSource
    status : GameStatus
    lastEventOn : DateTime
    playerCount : int
    containsMe : bool
    //TODO: Add current player name
}