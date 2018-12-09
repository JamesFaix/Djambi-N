[<AutoOpen>]
module Djambi.Api.Model.GameModel

type PlayerState =
    {
        id : int
        isAlive : bool
    }

type PieceKind =
    | Chief
    | Thug
    | Reporter
    | Assassin
    | Diplomat
    | Gravedigger
    | Corpse

type Piece =
    {
        id : int
        kind : PieceKind
        playerId : int option
        originalPlayerId : int
        cellId : int
    }

type GameState =
    {
        players : PlayerState list
        pieces : Piece list
        turnCycle : int list
    }

type SelectionKind =
    | Subject
    | Move
    | Target
    | Drop
    | Vacate

type Selection =
    {
        kind : SelectionKind
        cellId : int
        pieceId : int option
    }

module Selection =
    let subject(cellId, pieceId) =
        {
            kind = Subject
            cellId = cellId
            pieceId = Some pieceId
        }

    let move(cellId) =
        {
            kind = Move
            cellId = cellId
            pieceId = None
        }

    let moveWithTarget(cellId, pieceId) =
        {
            kind = Move
            cellId = cellId
            pieceId = Some pieceId
        }

    let target(cellId, pieceId) =
        {
            kind = Target
            cellId = cellId
            pieceId = Some pieceId
        }

    let drop(cellId) =
        {
            kind = Drop
            cellId = cellId
            pieceId = None
        }

    let vacate(cellId) =
        {
            kind = Vacate
            cellId = cellId
            pieceId = None
        }

[<CLIMutable>]
type SelectionRequest =
    {
        cellId : int
    }

type TurnStatus =
    | AwaitingSelection
    | AwaitingConfirmation

type TurnState =
    {
        status : TurnStatus
        selections : Selection list
        selectionOptions : int list
        requiredSelectionKind : SelectionKind option
    }

module TurnState =
    let empty =
        {
            status = AwaitingSelection
            selections = List.empty
            selectionOptions = List.empty
            requiredSelectionKind = Some Subject
        }

type PlayerStartConditions =
    {
        playerId : int
        region : int
        turnNumber : int option
        colodId : int
    }
    
type StartGameResponse =
    {
        gameId : int
        startingConditions : PlayerStartConditions list
        gameState : GameState
        turnState : TurnState
    }

type Game =
    {
        id : int
        regionCount : int
        gameState : GameState
        turnState : TurnState
    }

type CommitTurnResponse =
    {
        gameState : GameState
        turnState : TurnState
    }

type GameStatus =
    | Open
    | Started
    | Complete
    | Cancelled
