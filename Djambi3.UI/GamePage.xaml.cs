using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MoreLinq;
using Djambi.Engine;
using Djambi.Model;

namespace Djambi.UI
{
    /// <summary>
    /// Interaction logic for Page2.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        private readonly Brush _whiteBrush;
        private readonly Brush _blackBrush;
        private readonly Brush _grayBrush;
        private readonly Dictionary<PlayerColor, Brush> _playerColorBrushes;

        public GamePage()
        {
            InitializeComponent();
            this.Loaded += (sender, e) =>
            { 
                Window window = Window.GetWindow(this);
                window.SetBinding(Window.MinHeightProperty, new Binding() { Source = this.MinHeight });
                window.SetBinding(Window.MinWidthProperty, new Binding() { Source = this.MinWidth });
            };

            _whiteBrush = new SolidColorBrush(Colors.White);
            _blackBrush = new SolidColorBrush(Colors.Black);
            _grayBrush = new SolidColorBrush(Colors.Gray);
            _playerColorBrushes = new Dictionary<PlayerColor, Brush>
            {
                [PlayerColor.Red] = new SolidColorBrush(Colors.Red),
                [PlayerColor.Orange] = new SolidColorBrush(Colors.Orange),
                [PlayerColor.Yellow] = new SolidColorBrush(Colors.Yellow),
                [PlayerColor.Green] = new SolidColorBrush(Colors.Green),
                [PlayerColor.Blue] = new SolidColorBrush(Colors.Blue),
                [PlayerColor.Purple] = new SolidColorBrush(Colors.Purple),
                [PlayerColor.Dead] = new SolidColorBrush(Colors.Gray)
            };

            DrawBoard();
            DrawGameState();
        }

        private void DrawBoard()
        {
            for (var i = 0; i < Constants.BoardSize; i++)
            {
                gridBoard.RowDefinitions.Add(new RowDefinition
                {
                    Name = $"gridBoardRow{i}"
                });
                gridBoard.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Name = $"gridBoardColumn{i}"
                });
            }

            for (var r = 0; r < Constants.BoardSize; r++)
            {
                for (var c = 0; c < Constants.BoardSize; c++)
                {
                    var rect = new Rectangle
                    {
                        Fill = GetCellBrush(r, c),
                        Stretch = Stretch.Uniform
                    };

                    gridBoard.Children.Add(rect);
                    Grid.SetColumn(rect, c);
                    Grid.SetRow(rect, r);
                }
            }
        }

        private Brush GetCellBrush(int row, int column)
        {
            //-1 because WPF grids are 0-based, but the game is 1-based
            if (row == Constants.BoardCenter - 1
            && column == Constants.BoardCenter - 1)
            {
                return _grayBrush;
            }
            else
            {
                return (row + column) % 2 == 0
                    ? _blackBrush
                    : _whiteBrush;
            }
        }

        private void DrawGameState()
        {
            var state = StateManager.Current;

            var piecesWithColors = state.Pieces
                .LeftOuterJoin(state.Players,
                    piece => piece.PlayerId,
                    player => player.Id,
                    (piece, player) => new
                    {
                        Piece = piece,
                        Color = player.Color
                    },
                    piece => new
                    {
                        Piece = piece,
                        Color = PlayerColor.Dead
                    });

            foreach (var piece in piecesWithColors)
            {
                var ell = new Ellipse
                {
                    Fill = _playerColorBrushes[piece.Color],
                    Stretch = Stretch.Uniform,
                };

                gridBoard.Children.Add(ell);
                //-1 because grid is 0-based but game is 1-based
                Grid.SetColumn(ell, piece.Piece.Location.X - 1);
                Grid.SetRow(ell, piece.Piece.Location.Y - 1);
                Grid.SetZIndex(ell, 1);

                var image = new Image
                {
                    Source = new BitmapImage(new Uri(GetPieceImagePath(piece.Piece), UriKind.Relative)),
                    Height = 30,
                    Width = 30                    
                };

                gridBoard.Children.Add(image);
                Grid.SetColumn(image, piece.Piece.Location.X - 1);
                Grid.SetRow(image, piece.Piece.Location.Y - 1);
                Grid.SetZIndex(image, 2);
            }
            
            lblTurnCycle.Content =
                $"Turns: {Environment.NewLine}  " +
                state.TurnCycle
                    .Join(state.Players,
                        tc => tc,
                        p => p.Id,
                        (tc, p) => p)
                    .Select((p, n) => $"{n+1}. {p.Name}")
                    .ToDelimitedString(Environment.NewLine + "  ");
        }

        private string GetPieceImagePath(Piece piece)
        {
            if (!piece.IsAlive)
            {
                return "Images/corpse.png";
            }

            switch (piece.Type)
            {
                case PieceType.Assassin:
                    return "Images/assassin.png";

                case PieceType.Chief:
                    return "Images/chief.png";

                case PieceType.Diplomat:
                    return "Images/diplomat.png";

                case PieceType.Militant:
                    return "Images/militant.png";

                case PieceType.Necromobile:
                    return "Images/necromobile.png";

                case PieceType.Reporter:
                    return "Images/reporter.png";

                default:
                    throw new Exception("Invalid PieceType");
            }
        }

        private void btnQuitToMenu_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to quit and return to the main menu?",
                "Quit Game",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                this.NavigationService.Navigate(new Uri("MainMenuPage.xaml", UriKind.Relative));
            }
        }
    }
}
