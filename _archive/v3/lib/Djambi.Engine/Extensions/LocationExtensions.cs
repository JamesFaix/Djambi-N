using System;
using System.Collections.Generic;
using System.Linq;
using Djambi.Model;
using static System.Math;

namespace Djambi.Engine.Extensions
{
    public static class LocationExtensions
    {
        internal static bool IsValid(this Location @this) =>
            @this.X >= 1 &&
            @this.Y >= 1 &&
            @this.X <= Constants.BoardSize &&
            @this.Y <= Constants.BoardSize;

        internal static Location Offset(this Location @this, int x, int y) =>
            Location.Create(@this.X + x, @this.Y + y);

        internal static Location Offset(this Location @this, Directions direction, int value)
        {
            switch (direction)
            {
                case Directions.Left:
                    return Location.Create(@this.X - value, @this.Y);

                case Directions.UpLeft:
                    return Location.Create(@this.X - value, @this.Y + value);

                case Directions.Up:
                    return Location.Create(@this.X, @this.Y + value);

                case Directions.UpRight:
                    return Location.Create(@this.X + value, @this.Y + value);
                    
                case Directions.Right:
                    return Location.Create(@this.X + value, @this.Y);

                case Directions.DownRight:
                    return Location.Create(@this.X + value, @this.Y - value);

                case Directions.Down:
                    return Location.Create(@this.X, @this.Y - value);

                case Directions.DownLeft:
                    return Location.Create(@this.X - value, @this.Y - value);

                default:
                    throw new Exception($"Invalid {nameof(Directions)} value ({direction}).");
            }
        }

        internal static int Distance(this Location @this, Location other) =>
            Max(Abs(@this.X - other.X), Abs(@this.Y - other.Y));

        public static bool IsSeat(this Location @this) =>
            @this.X == Constants.BoardCenter && @this.Y == Constants.BoardCenter;

        public static IEnumerable<Location> AdjacentLocations(this Location @this) =>
            EnumUtility.GetValues<Directions>()
                .Select(d => @this.Offset(d, 1))
                .Where(loc => loc.IsValid());
    }
}
