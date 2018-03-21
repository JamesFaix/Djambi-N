using System;
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
        
        public static Location Offset(this Location @this, Directions direction, int value)
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

        public static int Distance(this Location @this, Location other) =>
            Max(Abs(@this.X - other.X), Abs(@this.Y - other.Y));

        public static bool IsMaze(this Location @this) =>
            @this.X == Constants.BoardCenter && @this.Y == Constants.BoardCenter;
    }
}
