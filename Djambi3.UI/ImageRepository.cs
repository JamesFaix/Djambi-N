using System;
using System.Windows.Media.Imaging;
using Djambi.Engine.Extensions;
using Djambi.Model;

namespace Djambi.UI.Images
{
    class ImageRepository
    {
        private static BitmapImage GetImage(string imageName) =>
            new BitmapImage(new Uri($"/Djambi.UI;component/Images/{imageName}.png", UriKind.Relative));

        #region Board

        public BitmapImage WhiteCell { get; } = GetImage("whiteCell");

        public BitmapImage BlackCell { get; } = GetImage("blackCell");

        public BitmapImage MazeCell { get; } = GetImage("mazeCell");

        public BitmapImage GetCellImage(Location location)
        {
            if (location.IsMaze())
            {
                return MazeCell;
            }
            else
            {
                return (location.X + location.Y) % 2 == 0
                    ? BlackCell
                    : WhiteCell;
            }
        }

        public BitmapImage RowLabel { get; } = GetImage("rowLabel");

        public BitmapImage ColumnLabel { get; } = GetImage("columnLabel");

        #endregion

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
