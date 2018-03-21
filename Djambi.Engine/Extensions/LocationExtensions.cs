using Djambi.Model;

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
    }
}
