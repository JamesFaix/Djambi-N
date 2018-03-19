namespace Djambi.Model
{
    public class Location
    {
        public int X { get; }

        public int Y { get; }

        private Location(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() =>
            $"({X}, {Y})";

        public static Location Create(int x, int y) =>
            new Location(x, y);
    }
}
