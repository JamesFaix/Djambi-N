namespace Apex.Api.Enums

type EffectKind =
    | CurrentTurnChanged = 1
    | GameStatusChanged = 2
    | NeutralPlayerAdded = 3
    | ParametersChanged = 4
    | PieceAbandoned = 5
    | PieceDropped = 6
    | PieceEnlisted = 7
    | PieceKilled = 8
    | PieceMoved = 9
    | PieceVacated = 10
    | PlayerAdded = 11
    | PlayerOutOfMoves = 12
    | PlayerRemoved = 13
    | PlayerStatusChanged = 14
    | TurnCycleAdvanced = 15
    | TurnCyclePlayerFellFromPower = 16
    | TurnCyclePlayerRemoved = 17
    | TurnCyclePlayerRoseToPower = 18

type EventKind =
    | GameParametersChanged = 1uy
    | GameCanceled = 2uy
    | PlayerJoined = 3uy
    | PlayerRemoved = 4uy
    | GameStarted = 5uy
    | TurnCommitted = 6uy
    | TurnReset = 7uy
    | CellSelected  = 8uy
    | PlayerStatusChanged = 9uy

type GameStatus = 
    | Pending = 1uy
    | InProgress = 2uy
    | Canceled = 3uy
    | Over = 4uy
    
type PieceKind = 
    | Conduit = 1uy
    | Thug = 2uy
    | Scientist = 3uy
    | Hunter = 4uy
    | Diplomat = 5uy
    | Reaper = 6uy
    | Corpse = 7uy

type PlayerKind =
    | User = 1uy
    | Guest = 2uy
    | Neutral = 3uy
    
type PlayerStatus = 
    | Pending = 1uy
    | Alive = 2uy
    | Eliminated = 3uy
    | Conceded = 4uy
    | WillConcede = 5uy
    | AcceptsDraw = 6uy
    | Victorious = 7uy
    
type Privilege =
    | EditUsers = 1uy
    | EditPendingGames = 2uy
    | OpenParticipation = 3uy
    | ViewGames = 4uy
    | Snapshots = 5uy

type SelectionKind = 
    | Subject = 1uy
    | Move = 2uy
    | Target = 3uy
    | Drop = 4uy
    | Vacate = 5uy

type TurnStatus =
    | AwaitingSelection = 1uy
    | AwaitingCommit = 2uy
    | DeadEnd = 3uy