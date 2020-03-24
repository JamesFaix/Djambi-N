using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apex.Api.Web.Model
{
    #region Events

    public class EventDto
    {
        public int Id { get; set; }
        public CreationSourceDto CreatedBy { get; set; }
        public int? ActingPlayerId { get; set; }
        public EventKindDto Kind { get; set; }
        public EffectDto[] Effects { get; set; }
    }

    public enum EventKindDto
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

    public abstract class EffectDto
    {
        public abstract EffectKindDto Kind { get; }
    }

    public enum EffectKindDto
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

    public abstract class ChangeEffectDto<T> : EffectDto
    {
        public T OldValue { get; set; }
        public T NewValue { get; set; }
    }

    public class CurrentTurnChangedEffectDto : ChangeEffectDto<TurnDto>
    {
        public override EffectKindDto Kind => EffectKindDto.CurrentTurnChanged;
    }

    public class GameStatusChangedEffectDto : ChangeEffectDto<GameStatusDto>
    {
        public override EffectKindDto Kind => EffectKindDto.GameStatusChanged;
    }

    public class NeutralPlayerAddedEffectDto : EffectDto
    {
        public override EffectKindDto Kind => EffectKindDto.NeutralPlayerAdded;
        public string Name { get; set; }
        public int PlaceholderPlayerId { get; set; }
    }

    public class ParametersChangedEffectDto : ChangeEffectDto<GameParametersDto>
    {
        public override EffectKindDto Kind => EffectKindDto.ParametersChanged;
    }

    public class PieceDroppedEffectDto : ChangeEffectDto<PieceDto>
    {
        public override EffectKindDto Kind => EffectKindDto.PieceDropped;
    }

    public class PieceEnlistedEffectDto : EffectDto
    {
        public override EffectKindDto Kind => EffectKindDto.PieceEnlisted;
        public PieceDto OldPiece { get; set; }
        public int NewPlayerId { get; set; }
    }

    public class PieceAbandonedEffectDto : EffectDto
    {
        public override EffectKindDto Kind => EffectKindDto.PieceAbandoned;
        public PieceDto OldPiece { get; set; }
    }

    public class PieceKilledEffectDto : EffectDto
    {
        public override EffectKindDto Kind => EffectKindDto.PieceKilled;
        public PieceDto OldPiece { get; set; }
    }

    public class PieceMovedEffectDto : EffectDto
    {
        public override EffectKindDto Kind => EffectKindDto.PieceMoved;
        public PieceDto OldPiece { get; set; }
        public int NewCellId { get; set; }
    }

    public class PieceVacatedEffectDto : EffectDto
    {
        public override EffectKindDto Kind => EffectKindDto.PieceVacated;
        public PieceDto OldPiece { get; set; }
        public int NewCellId { get; set; }
    }

    public class PlayerAddedEffectDto : EffectDto
    {
        public override EffectKindDto Kind => EffectKindDto.PlayerAdded;
        public string Name { get; set; }
        public int UserId { get; set; }
        public PlayerKindDto PlayerKind { get; set; }
    }

    public class PlayerOutOfMovesEffectDto : EffectDto
    {
        public override EffectKindDto Kind => EffectKindDto.PlayerOutOfMoves;
        public int PlayerId { get; set; }
    }

    public class PlayerRemovedEffectDto : EffectDto
    {
        public override EffectKindDto Kind => EffectKindDto.PlayerRemoved;
        public PlayerDto OldPlayer { get; set; }
    }

    public class PlayerStatusChangedEffectDto : ChangeEffectDto<PlayerStatusDto>
    {
        public override EffectKindDto Kind => EffectKindDto.PlayerStatusChanged;
        public int PlayerId { get; set; }
    }

    public class TurnCycleAdvancedEffectDto : ChangeEffectDto<int[]>
    {
        public override EffectKindDto Kind => EffectKindDto.TurnCycleAdvanced;
    }

    public class TurnCyclePlayerFellFromPowerEffectDto : ChangeEffectDto<int[]>
    {
        public override EffectKindDto Kind => EffectKindDto.TurnCyclePlayerFellFromPower;
        public int PlayerId { get; set; }
    }

    public class TurnCyclePlayerRemovedEffectDto : ChangeEffectDto<int[]>
    {
        public override EffectKindDto Kind => EffectKindDto.TurnCyclePlayerRemoved;
        public int PlayerId { get; set; }
    }

    public class TurnCyclePlayerRoseToPowerEffectDto : ChangeEffectDto<int[]>
    {
        public override EffectKindDto Kind => EffectKindDto.TurnCyclePlayerRoseToPower;
        public int PlayerId { get; set; }
    }

    #endregion

    public class StateAndEventResponseDto
    {
        public GameDto Game { get; set; }
        public EventDto Event { get; set; }
    }
}
