using Djambi.Model;

namespace Djambi.Engine.Extensions
{
    static class PieceExtensions
    {
        public static Piece Move(this Piece @this, Location location) =>
            Piece.Create(
                @this.Id, 
                @this.Type, 
                @this.PlayerId, 
                @this.OriginalPlayerId, 
                @this.IsAlive, 
                location);

        public static Piece Kill(this Piece @this) =>
            Piece.Create(
                @this.Id,
                @this.Type,
                @this.PlayerId,
                @this.OriginalPlayerId,
                false,
                @this.Location);

        public static Piece Capture(this Piece @this, int playerId) =>
            Piece.Create(
                @this.Id,
                @this.Type,
                playerId,
                @this.OriginalPlayerId,
                @this.IsAlive,
                @this.Location);

        public static Piece Abandon(this Piece @this) =>
            Piece.Create(
                @this.Id,
                @this.Type,
                null,
                @this.OriginalPlayerId,
                @this.IsAlive,
                @this.Location);
    }
}
