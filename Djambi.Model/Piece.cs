namespace Djambi.Model
{
    public class Piece
    {
        public int Id { get; }

        public PieceType Type { get; }

        public int Faction { get; }

        public bool IsAlive { get; }

        public Location Location { get; }

        private Piece(
            int id, 
            PieceType type, 
            int faction, 
            bool isAlive, 
            Location location)
        {
            Id = id;
            Type = type;
            Faction = faction;
            IsAlive = isAlive;
            Location = location;
        }

        public override string ToString() =>
            $"Id: {Id}, Type: {Type}, Faction: {Faction}, IsAlive: {IsAlive}, Location: {Location}";

        public static Piece Create(
            int id, 
            PieceType type, 
            int faction, 
            bool isAlive, 
            Location location) =>
            new Piece(id, type, faction, isAlive, location);
    }
}
