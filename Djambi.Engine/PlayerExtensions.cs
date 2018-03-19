using System.Linq;
using Djambi.Model;

namespace Djambi.Engine
{
    static class PlayerExtensions
    {
        public static Player AddFaction(this Player @this, int factionId) =>
            Player.Create(@this.Id, @this.Name, @this.IsAlive,
                @this.Factions.Concat(new[] { factionId }).Distinct());
    }
}
