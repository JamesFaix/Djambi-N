namespace Djambi.Api.Domain

open Djambi.Api.Common.Enums

module PlayModels =

    type Player =
        {
            id : int
            userId : int option
            name : string
            isAlive : bool
        }

    type PieceType =
        | Chief
        | Thug
        | Reporter
        | Assassin
        | Diplomat
        | Gravedigger
        //Corpse is not a piecetype; Piece.isAlive = false makes a Corpse

    type Piece = 
        {
            id : int
            pieceType : PieceType
            playerId : int option
            originalPlayerId : int
            isAlive : bool           
            cellId : int
        }

    type GameState =
        {
            players : Player list
            pieces : Piece list
            turnCycle : int list
            log : string list
        }

    type Selection = 
        | Subject of pieceId : int 
        | Move of cellId : int
        | MoveWithTarget of cellId : int * pieceId : int
        | Target of pieceId : int
        | Drop of cellId : int

    type TurnStatus =
        | AwaitingSelection
        | AwaitingConfirmation

    type TurnState =
        {
            status : TurnStatus
            selections : Selection list
        }
       
    type PlayerStartConditions =
        {
            playerId : int
            region : int
            turnNumber : int
            color : int
        }

    type UpdateGameForStartRequest =
        {
            id : int
            startingConditions : PlayerStartConditions list
            currentState : GameState
        }