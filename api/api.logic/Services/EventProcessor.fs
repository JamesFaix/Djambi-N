//This module applies the effects of a given event to the game state
module Djambi.Api.Logic.Services.EventProcessor

open Djambi.Api.Model
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories

let processEvent (game : Game) (event : Event) : StateAndEventResponse AsyncHttpResult =    
    let newGame = EventService.applyEvent game event
    EventRepository.persistEvent (game, newGame)
    |> thenMap (fun persistedGame -> { game = persistedGame; event = event })