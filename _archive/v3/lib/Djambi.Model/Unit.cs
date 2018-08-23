using System;

namespace Djambi.Model
{
    public sealed class Unit : IEquatable<Unit>
    {
        public static Unit Value => null;

        public bool Equals(Unit other) => true;

        public override bool Equals(object obj) => obj.GetType() == typeof(Unit);

        public override int GetHashCode() => 0;
    }
}
