using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Djambi.Engine
{
    public class InvalidPlayerNamesException : Exception
    {
        public ImmutableList<string> InvalidNames { get; }

        public InvalidPlayerNamesException(string message, IEnumerable<string> names)
            : base(message)
        {
            InvalidNames = names.ToImmutableList();
        }
    }
}
