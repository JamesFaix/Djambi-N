using System.Linq;
using Djambi.Model;

namespace Djambi.Engine
{
    static class PlayerExtensions
    {
        public static Player ConquerPlayer(this Player @this, int playerId) =>
            Player.Create(@this.Id, @this.Name, @this.IsAlive, @this.IsVirtual,
                @this.ConqueredPlayerIds
                    .Concat(new[] { playerId })
                    .Distinct()
                    .OrderBy(id => id));

        internal static Player SetId(this Player @this, int playerId) =>
            Player.Create(playerId, @this.Name, @this.IsAlive, @this.IsVirtual, @this.ConqueredPlayerIds);
    }
}
