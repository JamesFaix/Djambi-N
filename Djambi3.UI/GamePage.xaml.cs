using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Djambi.Engine;

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

            DrawBoard();
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
