using System;
using System.Windows.Media;
using Djambi.Engine.Extensions;
using Djambi.Model;

namespace Djambi.UI.Resources
{
    public interface IBrushRepository
    {
        Brush BoardLabelBrush { get; }
        Brush BoardLabelBackgroundBrush { get; }
        Brush SelectionOptionBrush { get; }
        Brush SelectionBrush { get; }
        Brush GetCellBrush(Location location);
        Brush GetPlayerBrush(PlayerColor playerColor);
    }

    class BrushRepository : IBrushRepository
    {
        private readonly IThemeLoader _themeLoader;

        private Brush _boardLabel;
        private Brush _boardLabelBackground;
        private Brush _selectionOption;
        private Brush _selection;
        private Brush _oddCell;
        private Brush _evenCell;
        private Brush _seatCell;
        private Brush _p1;
        private Brush _p2;
        private Brush _p3;
        private Brush _p4;
        private Brush _dead;

        public BrushRepository(IThemeLoader themeLoader)
        {
            _themeLoader = themeLoader;
            _themeLoader.ThemeLoaded += Clear;
        }

        public void Clear()
        {
            _boardLabel = null;
            _boardLabelBackground = null;
            _selectionOption = null;
            _selection = null;
            _oddCell = null;
            _evenCell = null;
            _seatCell = null;
            _p1 = null;
            _p2 = null;
            _p3 = null;
            _p4 = null;
            _dead = null;
        }

        private Color FromHex(string hex) =>
            (Color)ColorConverter.ConvertFromString(hex);

        public Brush BoardLabelBrush =>
            _boardLabel is null
                ? _boardLabel = new SolidColorBrush(FromHex(_themeLoader.Theme.Style.BoardLabelColor))
                : _boardLabel;

        public Brush BoardLabelBackgroundBrush =>
            _boardLabelBackground is null
                ? _boardLabelBackground = new SolidColorBrush(FromHex(_themeLoader.Theme.Style.BoardLabelBackgroundColor))
                : _boardLabelBackground;

        public Brush SelectionOptionBrush =>
            _selectionOption is null
                ? _selectionOption = new SolidColorBrush(FromHex(_themeLoader.Theme.Style.SelectionOptionHighlightColor))
                : _selectionOption;

        public Brush SelectionBrush =>
            _selection is null
                ? _selection = new SolidColorBrush(FromHex(_themeLoader.Theme.Style.SelectionHighlightColor))
                : _selection;

        private Brush EvenCellBrush =>
            _evenCell is null
                ? _evenCell = new SolidColorBrush(FromHex(_themeLoader.Theme.Style.CellColors[0]))
                : _evenCell;

        private Brush OddCellBrush =>
            _oddCell is null
                ? _oddCell = new SolidColorBrush(FromHex(_themeLoader.Theme.Style.CellColors[1]))
                : _oddCell;

        private Brush SeatCellBrush =>
            _seatCell is null
                ? _seatCell = new SolidColorBrush(FromHex(_themeLoader.Theme.Style.CellColors[2]))
                : _seatCell;

        private Brush P1Brush =>
            _p1 is null
                ? _p1 = new SolidColorBrush(FromHex(_themeLoader.Theme.Style.PlayerColors[0]))
                : _p1;

        private Brush P2Brush =>
            _p2 is null
                ? _p2 = new SolidColorBrush(FromHex(_themeLoader.Theme.Style.PlayerColors[1]))
                : _p2;

        private Brush P3Brush =>
            _p3 is null
                ? _p3 = new SolidColorBrush(FromHex(_themeLoader.Theme.Style.PlayerColors[2]))
                : _p3;

        private Brush P4Brush =>
            _p4 is null
                ? _p4 = new SolidColorBrush(FromHex(_themeLoader.Theme.Style.PlayerColors[3]))
                : _p4;

        private Brush DeadBrush =>
            _dead is null
                ? _dead = new SolidColorBrush(FromHex(_themeLoader.Theme.Style.PlayerColors[4]))
                : _dead;

        public Brush GetCellBrush(Location location) =>
            location.IsSeat()
                ? SeatCellBrush
                : (location.X + location.Y) % 2 == 0
                    ? EvenCellBrush
                    : OddCellBrush;

        public Brush GetPlayerBrush(PlayerColor playerColor) =>
            playerColor switch
            {
                PlayerColor.Blue => P1Brush,
                PlayerColor.Red => P2Brush,
                PlayerColor.Green => P3Brush,
                PlayerColor.Purple => P4Brush,
                PlayerColor.Dead => DeadBrush,
                _ => throw new Exception($"Invalid player color: {playerColor}"),
            };
    }
}
