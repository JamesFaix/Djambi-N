using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apex.Api.Web.Model
{
    #region Events

    public class Event
    {
        public int Id { get; set; }
        public CreationSource CreatedBy { get; set; }
        public int? ActingPlayerId { get; set; }
        public EventKind Kind { get; set; }
        public Effect[] Effects { get; set; }
    }

    public enum EventKind
    {
        GameParametersChanged = 1,
        GameCanceled = 2,
        PlayerJoined = 3,
        PlayerRemoved = 4,
        GameStarted = 5,
        TurnCommitted = 6,
        TurnReset = 7,
        CellSelected = 8,
        PlayerStatusChanged = 9
    }

    #endregion

    #region Effects

    public abstract class Effect
    {
        public abstract EffectKind Kind { get; }
    }

    public enum EffectKind
    {
        CurrentTurnChanged = 1,
        GameStatusChanged = 2,
        NeutralPlayerAdded = 3,
        ParametersChanged = 4,
        PieceAbandoned = 5,
        PieceDropped = 6,
        PieceEnlisted = 7,
        PieceKilled = 8,
        PieceMoved = 9,
        PieceVacated = 10,
        PlayerAdded = 11,
        PlayerOutOfMoves = 12,
        PlayerRemoved = 13,
        PlayerStatusChanged = 14,
        TurnCycleAdvanced = 15,
        TurnCyclePlayerFellFromPower = 16,
        TurnCyclePlayerRemoved = 17,
        TurnCyclePlayerRoseToPower = 18
    }

    public abstract class ChangeEffect<T> : Effect
    {
        public T OldValue { get; set; }
        public T NewValue { get; set; }
    }

    public class CurrentTurnChangedEffect : ChangeEffect<Turn>
    {
        public override EffectKind Kind => EffectKind.CurrentTurnChanged;
    }

    public class GameStatusChangedEffect : ChangeEffect<GameStatus>
    {
        public override EffectKind Kind => EffectKind.GameStatusChanged;
    }

    public class NeutralPlayerAddedEffect : Effect
    {
        public override EffectKind Kind => EffectKind.NeutralPlayerAdded;
        public string Name { get; set; }
        public int PlaceholderPlayerId { get; set; }
    }

    public class ParametersChangedEffect : ChangeEffect<GameParameters>
    {
        public override EffectKind Kind => EffectKind.ParametersChanged;
    }

    public class PieceEnlistedEffect : Effect
    {
        public override EffectKind Kind => EffectKind.PieceEnlisted;
        public Piece OldPiece { get; set; }
        public int NewPlayerId { get; set; }
    }

    public class PieceAbandonedEffect : Effect
    {
        public override EffectKind Kind => EffectKind.PieceAbandoned;
        public Piece OldPiece { get; set; }
    }

    public class PieceDroppedEffect : ChangeEffect<Piece>
    {
        public override EffectKind Kind => EffectKind.PieceDropped;
    }

    public class PieceKilledEffect : Effect
    {
        public override EffectKind Kind => EffectKind.PieceKilled;
        public Piece OldPiece { get; set; }
    }

    public class PieceMovedEffect : Effect
    {
        public override EffectKind Kind => EffectKind.PieceMoved;
        public Piece OldPiece { get; set; }
        public int NewCellId { get; set; }
    }

    public class PieceVacatedEffect : Effect
    {
        public override EffectKind Kind => EffectKind.PieceVacated;
        public Piece OldPiece { get; set; }
        public int NewCellId { get; set; }
    }

    public class PlayerAddedEffect : Effect
    {
        public override EffectKind Kind => EffectKind.PlayerAdded;
        public string Name { get; set; }
        public int UserId { get; set; }
        public PlayerKind PlayerKind { get; set; }
    }

    public class PlayerOutOfMovesEffect : Effect
    {
        public override EffectKind Kind => EffectKind.PlayerOutOfMoves;
        public int PlayerId { get; set; }
    }

    public class PlayerRemovedEffect : Effect
    {
        public override EffectKind Kind => EffectKind.PlayerRemoved;
        public Player OldPlayer { get; set; }
    }

    public class PlayerStatusChangedEffect : ChangeEffect<PlayerStatus>
    {
        public override EffectKind Kind => EffectKind.PlayerStatusChanged;
        public int PlayerId { get; set; }
    }

    public class TurnCycleAdvancedEffect : ChangeEffect<int[]>
    {
        public override EffectKind Kind => EffectKind.TurnCycleAdvanced;
    }

    public class TurnCyclePlayerFellFromPowerEffect : ChangeEffect<int[]>
    {
        public override EffectKind Kind => EffectKind.TurnCyclePlayerFellFromPower;
        public int PlayerId { get; set; }
    }

    public class TurnCyclePlayerRemovedEffect : ChangeEffect<int[]>
    {
        public override EffectKind Kind => EffectKind.TurnCyclePlayerRemoved;
        public int PlayerId { get; set; }
    }

    public class TurnCyclePlayerRoseToPowerEffect : ChangeEffect<int[]>
    {
        public override EffectKind Kind => EffectKind.TurnCyclePlayerRoseToPower;
        public int PlayerId { get; set; }
    }

    #endregion

    public class StateAndEventResponse
    {
        public Game Game { get; set; }
        public Event Event { get; set; }
    }
}
