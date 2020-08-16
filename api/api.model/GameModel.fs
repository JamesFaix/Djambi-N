[<AutoOpen>]
module Djambi.Api.Model.GameModel

open Djambi.Api.Enums

type Player =
    {
        id : int
        gameId : int
        userId : int option
        kind : PlayerKind
        name : string
        status : PlayerStatus
        colorId : int option
        startingRegion : int option
        startingTurnNumber : int option
    }

type Piece =
    {
        id : int
        kind : PieceKind
        playerId : int option
        originalPlayerId : int
        cellId : int
    }

type Selection =
    {
        kind : SelectionKind
        cellId : int
        pieceId : int option
    }

module Selection =
    let subject(cellId, pieceId) =
        {
            kind = SelectionKind.Subject
            cellId = cellId
            pieceId = Some pieceId
        }

    let move(cellId) =
        {
            kind = SelectionKind.Move
            cellId = cellId
            pieceId = None
        }

    let moveWithTarget(cellId, pieceId) =
        {
            kind = SelectionKind.Move
            cellId = cellId
            pieceId = Some pieceId
        }

    let target(cellId, pieceId) =
        {
            kind = SelectionKind.Target
            cellId = cellId
            pieceId = Some pieceId
        }

    let drop(cellId) =
        {
            kind = SelectionKind.Drop
            cellId = cellId
            pieceId = None
        }

    let vacate(cellId) =
        {
            kind = SelectionKind.Vacate
            cellId = cellId
            pieceId = None
        }

type Turn =
    {
        status : TurnStatus
        selections : Selection list
        selectionOptions : int list
        requiredSelectionKind : SelectionKind option
    }

module Turn =
    let empty =
        {
            status = TurnStatus.AwaitingSelection
            selections = []
            selectionOptions = []
            requiredSelectionKind = Some SelectionKind.Subject
        }

    let deadEnd (selections) =
        {
            status = TurnStatus.DeadEnd
            selections = selections
            selectionOptions = []
            requiredSelectionKind = None
        }

type GameParameters =
    {
        description : string option
        regionCount : int
        isPublic : bool
        allowGuests : bool
    }

type Game =
    {
        id : int
        createdBy : CreationSource
        parameters : GameParameters
        status : GameStatus
        players : Player list
        pieces : Piece list
        turnCycle : int list
        currentTurn : Turn option
    }