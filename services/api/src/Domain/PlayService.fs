namespace Djambi.Api.Domain

open Giraffe
open System.Threading.Tasks
open Djambi.Api.Persistence
open Djambi.Api.Domain.PlayModels

type PlayService(repository : PlayRepository) =

    member this.getGameState(gameId : int) : GameState Task =
        repository.getCurrentGameState gameId

    member this.getSelectableCells(gameId : int, playerId : int) : int list Task =
        task {
            let! currentState = repository.getCurrentGameState gameId

            if currentState.turnCycle.Head <> playerId
            then return List.empty
            else 

            return List.empty
        }