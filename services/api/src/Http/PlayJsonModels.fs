namespace Djambi.Api.Http

module PlayJsonModels =

    open Djambi.Api.Common.Enums
    open System
    open Djambi.Api.Domain.PlayModels

    [<CLIMutable>]
    type LocationJsonModel =
        {
            region : int
            x : int
            y : int
        }

    [<CLIMutable>]
    type CreateSelectionJsonModel =
        {
            cellId : int
        }

    [<CLIMutable>]
    type CreateMessageJsonModel = 
        {
            body : string
            //TODO: Maybe add UserId for direct messages
        }

    type PlayerJsonModel =
        {
            id : int
            userId : Nullable<int>
            name : string
            isAlive : bool
        }

    type CellJsonModel =
        {
            id : int
            locations : LocationJsonModel list
        }

    type PieceJsonModel =
        {
            id : int
            ``type`` : string
            playerId : Nullable<int>
            originalPlayerId : int
            isAlive : bool
            cellId : int
        }

    type GameStateJsonModel =
        {
            players : PlayerJsonModel list 
            pieces : PieceJsonModel list
            turnCycle : int list
        }

    type PlayerStartConditionsJsonModel =
        {
            playerId : int
            turnNumber : int
            region : int
            color : int
        }

    type GameStartResponseJsonModel =
        {
            currentState : GameStateJsonModel
            startingConditions : PlayerStartConditionsJsonModel list
        }

    
    type SelectableCellsResponseJsonModel =
        {
            cellIds : int list
        }