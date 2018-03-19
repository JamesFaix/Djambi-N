namespace Djambi.Model
{
    public class Piece
    {
        public int Id { get; }

        public PieceType Type { get; }

        public int? Owner { get; }

        public int? OriginalOwner { get; }

        public bool IsAlive { get; }

        public Location Location { get; }

        private Piece(
            int id, 
            PieceType type, 
            int? owner, 
            int? originalOwner,
            bool isAlive, 
            Location location)
        {
            Id = id;
            Type = type;
            Owner = owner;
            OriginalOwner = originalOwner;
            IsAlive = isAlive;
            Location = location;
        }

        public static Piece Create(
            int id, 
            PieceType type, 
            int? owner, 
            int? originalOwner, 
            bool isAlive, 
            Location location) =>
            new Piece(id, type, owner, originalOwner, isAlive, location);
    }
}
