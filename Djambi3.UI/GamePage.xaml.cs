using System;
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
        private readonly Brush _redBrush;
        private readonly Brush _yellowBrush;
        private readonly Brush _greenBrush;
        private readonly Brush _blueBrush;

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
            _redBrush = new SolidColorBrush(Colors.Red);
            _yellowBrush = new SolidColorBrush(Colors.Yellow);
            _greenBrush = new SolidColorBrush(Colors.Green);
            _blueBrush = new SolidColorBrush(Colors.Blue);

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

            foreach (var piece in state.Pieces)
            {
                var ell = new Ellipse
                {
                    Fill = GetPieceBrush(piece.Faction),
                    Stretch = Stretch.Uniform,
                };

                gridBoard.Children.Add(ell);
                //-1 because grid is 0-based but game is 1-based
                Grid.SetColumn(ell, piece.Location.X - 1);
                Grid.SetRow(ell, piece.Location.Y - 1);
                Grid.SetZIndex(ell, 1);

                var image = new Image
                {
                    Source = new BitmapImage(new Uri(GetPieceImagePath(piece), UriKind.Relative)),
                    Height = 30,
                    Width = 30                    
                };

                gridBoard.Children.Add(image);
                Grid.SetColumn(image, piece.Location.X - 1);
                Grid.SetRow(image, piece.Location.Y - 1);
                Grid.SetZIndex(image, 2);
            }

            var turnCycleText = 
                state.TurnCycle
                    .Join(state.Players,
                        tc => tc,
                        p => p.Id,
                        (tc, p) => p)
                    .Select((p, n) => $"{n+1}. {p.Name}")
                    .ToDelimitedString(Environment.NewLine + "  ");

            turnCycleText = $"Turns: {Environment.NewLine}  {turnCycleText}";

            lblTurnCycle.Content = turnCycleText;
        }

        private Brush GetPieceBrush(int factionId)
        {
            switch (factionId)
            {
                case 1: return _redBrush;
                case 2: return _yellowBrush;
                case 3: return _greenBrush;
                case 4: return _blueBrush;
                default: throw new Exception("Invalid factionId");
            }
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
