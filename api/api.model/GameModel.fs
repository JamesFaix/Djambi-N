[<AutoOpen>]
module Djambi.Api.Model.GameModel

open System

type PlayerKind =
    | User
    | Guest
    | Neutral

type PlayerStatus =
    | Pending
    | Alive
    | Eliminated

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

type TurnStatus =
    | AwaitingSelection
    | AwaitingConfirmation

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
            status = AwaitingSelection
            selections = List.empty
            selectionOptions = List.empty
            requiredSelectionKind = Some Subject
        }

type GameStatus =
    | Pending
    | AbortedWhilePending
    | Started
    | Aborted
    | Finished
    
[<CLIMutable>]
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
        createdOn : DateTime
        createdByUserId : int
        parameters : GameParameters
        status : GameStatus
        players : Player list
        pieces : Piece list
        turnCycle : int list
        currentTurn : Turn option
    }