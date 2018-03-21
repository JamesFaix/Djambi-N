using Djambi.Model;
using static System.Math;

namespace Djambi.Engine.Extensions
{
    static class LocationExtensions
    {
        public static bool IsValid(this Location @this) =>
            @this.X >= 1 &&
            @this.Y >= 1 &&
            @this.X <= Constants.BoardSize &&
            @this.Y <= Constants.BoardSize;

        public static Location Offset(this Location @this, int x, int y) =>
            Location.Create(@this.X + x, @this.Y + y);

        public static bool IsColinear(this Location @this, Location other)
        {
            //Vertical or horizontal
            if (@this.X == other.X || @this.Y == other.Y)
            {
                return true;
            }

            //Diagonal
            if (Abs(@this.X - other.X) == Abs(@this.Y - other.Y))
            {
                return true;
            }

            return false;
        }
    }
}
