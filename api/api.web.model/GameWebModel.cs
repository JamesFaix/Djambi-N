namespace Apex.Api.Web.Model
{
    public enum PlayerKindDto
    {
        User = 1,
        Guest = 2,
        Neutral = 3
    }

    public enum PlayerStatusDto
    {
        Pending = 1,
        Alive = 2,
        Eliminated = 3,
        Conceded = 4,
        WillConcede = 5,
        AcceptsDraw = 6,
        Victorious = 7
    }

    public class PlayerDto
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int? UserId { get; set; }
        public string Name { get; set; }
        public PlayerStatusDto Status { get; set; }
        public int? ColorId { get; set; }
        public int? StartingRegion { get; set; }
        public int? StartingTurnNumber { get; set; }
    }

    public enum PieceKindDto
    {
        Conduit = 1,
        Thug = 2,
        Scientist = 3,
        Hunter = 4,
        Diplomat = 5,
        Reaper = 6,
        Corpse = 7
    }

    public class PieceDto
    {
        public int Id { get; set; }
        public PieceKindDto Kind { get; set; }
        public int? PlayerId { get; set; }
        public int OriginalPlayerId { get; set; }
        public int CellId { get; set; }
    }

    public enum SelectionKindDto
    {
        Subject = 1,
        Move = 2,
        Target = 3,
        Drop = 4,
        Vacate = 5
    }

    public class SelectionDto
    {
        public SelectionKindDto Kind { get; set; }
        public int CellId { get; set; }
        public int? PieceId { get; set; }
    }

    public enum TurnStatusDto
    {
        AwaitingSelection = 1,
        AwaitingCommit = 2,
        DeadEnd = 3 
    }

    public class TurnDto
    {
        public TurnStatusDto Status { get; set; }
        public SelectionDto[] Selections { get; set; }
        public int[] SelectionOptions { get; set; }
        public SelectionKindDto? RequiresSelectionKind { get; set; }
    }

    public enum GameStatusDto
    {
        Pending = 1,
        InProgress = 2,
        Canceled = 3,
        Over = 4
    }

    public class GameParametersDto
    {
        public string Description { get; set; }
        public int RegionCount { get; set; }
        public bool IsPublic { get; set; }
        public bool AllowGuests { get; set; }
    }

    public class GameDto
    {
        public int Id { get; set; }
        public CreationSourceDto CreatedBy { get; set; }
        public GameParametersDto Parameters { get; set; }
        public GameStatusDto Status { get; set; }
        public PlayerDto[] Players { get; set; }
        public PieceDto[] Pieces { get; set; }
        public int[] TurnCycle { get; set; }
        public TurnDto CurrentTurn { get; set; }
    }
}
