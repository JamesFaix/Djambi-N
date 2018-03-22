using System;

namespace Djambi.Model
{
    public class Location : IEquatable<Location>
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

        #region Equality

        public bool Equals(Location other)
        {
            if (other == null) return false;
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj) => Equals(obj as Location);

        public override int GetHashCode() => X + (Y << 16);

        public static bool operator == (Location a, Location b)
        {
            if (Equals(a, null)) return Equals(b, null);
            return a.Equals(b);
        }

        public static bool operator != (Location a, Location b)
        {
            if (Equals(a, null)) return !Equals(b, null);
            return !a.Equals(b);
        }

        #endregion
    }
}
