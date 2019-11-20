[<AutoOpen>]
module Apex.Api.Model.GameModel

open Apex.ClientGenerator.Annotations

[<ClientType(ClientSection.Player)>]
type PlayerKind =
    | User
    | Guest
    | Neutral

[<ClientType(ClientSection.Player)>]
type PlayerStatus =
    | Pending
    | Alive
    | Eliminated
    | Conceded
    | WillConcede
    | AcceptsDraw
    | Victorious

[<ClientType(ClientSection.Player)>]
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

[<ClientType(ClientSection.Game)>]
type PieceKind =
    | Conduit
    | Thug
    | Scientist
    | Hunter
    | Diplomat
    | Reaper
    | Corpse

[<ClientType(ClientSection.Game)>]
type Piece =
    {
        id : int
        kind : PieceKind
        playerId : int option
        originalPlayerId : int
        cellId : int
    }

[<ClientType(ClientSection.Turn)>]
type SelectionKind =
    | Subject
    | Move
    | Target
    | Drop
    | Vacate

[<ClientType(ClientSection.Turn)>]
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

[<ClientType(ClientSection.Turn)>]
type TurnStatus =
    | AwaitingSelection
    | AwaitingCommit
    | DeadEnd

[<ClientType(ClientSection.Turn)>]
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
            selections = []
            selectionOptions = []
            requiredSelectionKind = Some Subject
        }

    let deadEnd (selections) =
        {
            status = DeadEnd
            selections = selections
            selectionOptions = []
            requiredSelectionKind = None
        }

[<ClientType(ClientSection.Game)>]
type GameStatus =
    | Pending
    | InProgress
    | Canceled
    | Over

[<CLIMutable>]
[<ClientType(ClientSection.Game)>]
type GameParameters =
    {
        description : string option
        regionCount : int
        isPublic : bool
        allowGuests : bool
    }

[<ClientType(ClientSection.Game)>]
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