namespace Apex.Api.Web.Model
{
    public enum PlayerKind
    {
        User = 1,
        Guest = 2,
        Neutral = 3
    }

    public enum PlayerStatus
    {
        Pending = 1,
        Alive = 2,
        Eliminated = 3,
        Conceded = 4,
        WillConcede = 5,
        AcceptsDraw = 6,
        Victorious = 7
    }

    public class Player
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int? UserId { get; set; }
        public string Name { get; set; }
        public PlayerStatus Status { get; set; }
        public int? ColorId { get; set; }
        public int? StartingRegion { get; set; }
        public int? StartingTurnNumber { get; set; }
    }

    public enum PieceKind
    {
        Conduit = 1,
        Thug = 2,
        Scientist = 3,
        Hunter = 4,
        Diplomat = 5,
        Reaper = 6,
        Corpse = 7
    }

    public class Piece
    {
        public int Id { get; set; }
        public PieceKind Kind { get; set; }
        public int? PlayerId { get; set; }
        public int OriginalPlayerId { get; set; }
        public int CellId { get; set; }
    }

    public enum SelectionKind
    {
        Subject = 1,
        Move = 2,
        Target = 3,
        Drop = 4,
        Vacate = 5
    }

    public class Selection
    {
        public SelectionKind Kind { get; set; }
        public int CellId { get; set; }
        public int? PieceId { get; set; }
    }

    public enum TurnStatus
    {
        AwaitingSelection = 1,
        AwaitingCommit = 2,
        DeadEnd = 3 
    }

    public class Turn
    {
        public TurnStatus Status { get; set; }
        public Selection[] Selections { get; set; }
        public int[] SelectionOptions { get; set; }
        public SelectionKind? RequiresSelectionKind { get; set; }
    }

    public enum GameStatus
    {
        Pending = 1,
        InProgress = 2,
        Canceled = 3,
        Over = 4
    }

    public class GameParameters
    {
        public string Description { get; set; }
        public int RegionCount { get; set; }
        public bool IsPublic { get; set; }
        public bool AllowGuests { get; set; }
    }

    public class Game
    {
        public int Id { get; set; }
        public CreationSource CreatedBy { get; set; }
        public GameParameters Parameters { get; set; }
        public GameStatus Status { get; set; }
        public Player[] Players { get; set; }
        public Piece[] Pieces { get; set; }
        public int[] TurnCycle { get; set; }
        public Turn CurrentTurn { get; set; }
    }
}
