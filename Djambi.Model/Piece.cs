namespace Djambi.Model
{
    public class Piece
    {
        public int Id { get; }

        public PieceType Type { get; }

        public int? PlayerId { get; }

        public int OriginalPlayerId { get; }

        public bool IsAlive { get; }

        public Location Location { get; }

        private Piece(
            int id, 
            PieceType type, 
            int? playerId,
            int originalPlayerId,
            bool isAlive, 
            Location location)
        {
            Id = id;
            Type = type;
            PlayerId = playerId;
            OriginalPlayerId = originalPlayerId;
            IsAlive = isAlive;
            Location = location;
        }

        public override string ToString() =>
            $"Id: {Id}, " +
            $"Type: {Type}, " +
            $"PlayerId: {(PlayerId != null ? PlayerId.ToString() : "Neutral")}, " +
            $"OriginalPlayerId: {OriginalPlayerId}, " + 
            $"IsAlive: {IsAlive}, " + 
            $"Location: {Location}";

        public static Piece Create(
            int id,
            PieceType type,
            int? playerId,
            int originalPlayerId,
            bool isAlive,
            Location location) =>
            new Piece(id, type, playerId, originalPlayerId, isAlive, location);
    }
}
