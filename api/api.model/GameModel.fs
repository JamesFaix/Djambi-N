module Djambi.Api.Model.GameModel

type PlayerState =
    {
        id : int
        isAlive : bool
    }

type PieceType =
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
        pieceType : PieceType
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

type TurnStatus =
    | AwaitingSelection
    | AwaitingConfirmation

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
        turnNumber : int option
        color : int
    }

type StartGameRequest =
    {
        lobbyId : int
        startingConditions : PlayerStartConditions list
        gameState : GameState
        turnState : TurnState
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
