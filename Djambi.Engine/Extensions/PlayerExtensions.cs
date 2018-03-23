using Djambi.Model;

namespace Djambi.Engine.Extensions
{
    static class PlayerExtensions
    {
        public static Player Kill(this Player @this) =>
            Player.Create(
                @this.Id,
                @this.Name,
                false,
                @this.IsVirtual,
                @this.Color);
    }
}
