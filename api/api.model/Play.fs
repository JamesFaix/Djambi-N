namespace Djambi.Api.Model

open Djambi.Api.Common.Enums

module Play =

    type Player =
        {
            id : int
            userId : int option
            name : string
            isAlive : bool
        }

    type Piece = 
        {
            id : int
            pieceType : PieceType
            playerId : int option
            originalPlayerId : int
            cellId : int
        }

    type GameState =
        {
            players : Player list
            pieces : Piece list
            turnCycle : int list
        }

    type SelectionType =
        | Subject
        | Move
        | Target
        | Drop
        | Vacate

    type Selection = 
        {
            selectionType : SelectionType
            cellId : int
            pieceId : int option
        }

    module Selection =
        let subject(cellId, pieceId) = 
            {   
                selectionType = Subject
                cellId = cellId
                pieceId = Some pieceId
            }

        let move(cellId) =
            {
                selectionType = Move
                cellId = cellId
                pieceId = None
            }

        let moveWithTarget(cellId, pieceId) =
            {
                selectionType = Move
                cellId = cellId
                pieceId = Some pieceId
            }

        let target(cellId, pieceId) =
            {
                selectionType = Target
                cellId = cellId
                pieceId = Some pieceId
            }

        let drop(cellId) =
            {
                selectionType = Drop
                cellId = cellId
                pieceId = None
            }

        let vacate(cellId) =
            {
                selectionType = Vacate
                cellId = cellId
                pieceId = None
            }

    type TurnState =
        {
            status : TurnStatus
            selections : Selection list
            selectionOptions : int list
            requiredSelectionType : SelectionType option
        }

    module TurnState =
        let empty = 
            {
                status = AwaitingSelection
                selections = List.empty
                selectionOptions = List.empty
                requiredSelectionType = Some Subject
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
            currentGameState : GameState
            currentTurnState : TurnState
        }

    type GameStartResponse = 
        {
            startingConditions : PlayerStartConditions list
            gameState : GameState
            turnState : TurnState
        }

    type Game =
        {
            boardRegionCount : int
            currentGameState : GameState
            currentTurnState : TurnState
        }

    type CommitTurnResponse =
        {
            gameState : GameState
            turnState : TurnState
        }