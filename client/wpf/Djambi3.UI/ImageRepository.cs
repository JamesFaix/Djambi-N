using System;
using System.Windows.Media.Imaging;
using Djambi.Model;

namespace Djambi.UI
{
    class ImageRepository
    {
        private static BitmapImage GetImage(string imageName) =>
            new BitmapImage(new Uri($"/Djambi.UI;component/Images/{imageName}.png", UriKind.Relative));

        public BitmapImage AppIcon { get; } = new BitmapImage(new Uri("pack://application:,,,/Images/chief.png", UriKind.RelativeOrAbsolute));
        
        #region Pieces

        public BitmapImage Assassin { get; } = GetImage("assassin");

        public BitmapImage Chief { get; } = GetImage("chief");

        public BitmapImage Diplomat { get; } = GetImage("diplomat");

        public BitmapImage Journalist { get; } = GetImage("journalist");

        public BitmapImage Thug { get; } = GetImage("thug");

        public BitmapImage Undertaker { get; } = GetImage("undertaker");

        public BitmapImage Corpse { get; } = GetImage("corpse");

        public BitmapImage GetPieceImage(Piece piece)
        {
            if (!piece.IsAlive)
            {
                return Corpse;
            }

            switch (piece.Type)
            {
                case PieceType.Assassin: return Assassin;
                case PieceType.Chief: return Chief;
                case PieceType.Diplomat: return Diplomat;
                case PieceType.Journalist: return Journalist;
                case PieceType.Thug: return Thug;
                case PieceType.Undertaker: return Undertaker;
                default:
                    throw new Exception($"Invalid {nameof(PieceType)} value ({piece.Type}).");
            }
        }

        #endregion
    }
}
