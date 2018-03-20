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
        public GamePage()
        {
            InitializeComponent();
            this.Loaded += (sender, e) =>
            { 
                Window window = Window.GetWindow(this);
                window.SetBinding(Window.MinHeightProperty, new Binding() { Source = this.MinHeight });
                window.SetBinding(Window.MinWidthProperty, new Binding() { Source = this.MinWidth });
            };

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
                    var color = (c + r) % 2 == 0
                        ? Colors.Black
                        : Colors.White;

                    var rect = new Rectangle
                    {
                        Fill = new SolidColorBrush(color),
                        Stretch = Stretch.Uniform,
                    };

                    gridBoard.Children.Add(rect);
                    Grid.SetColumn(rect, c);
                    Grid.SetRow(rect, r);
                }
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
