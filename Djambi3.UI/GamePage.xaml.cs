using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Djambi.Engine;
using Djambi.Engine.Extensions;
using Djambi.Model;

namespace Djambi.UI
{
    public partial class GamePage : Page
    {
        private readonly Dictionary<PlayerColor, Brush> _playerColorBrushes;
        private readonly Brush _selectionOptionBrush;
        private readonly Brush _selectionBrush;
        private readonly Brush _boardLabelBrush;

        private const double _selectionOpacity = 0.5;
        private const string _selectionOptionRectName = "SelectionOption";
        private const string _selectionRectName = "Selection";
        private const string _pieceElementName = "Piece";
        private const string _turnCycleElementName = "Turn";
        private const string _playerElementName = "Player";

        public GamePage()
        {
            InitializeComponent();

            _playerColorBrushes = new Dictionary<PlayerColor, Brush>
            {
                [PlayerColor.Red] = new SolidColorBrush(Colors.Maroon),
                [PlayerColor.Green] = new SolidColorBrush(Colors.ForestGreen),
                [PlayerColor.Blue] = new SolidColorBrush(Colors.MediumBlue),
                [PlayerColor.Purple] = new SolidColorBrush(Colors.Purple),
                [PlayerColor.Dead] = new SolidColorBrush(Colors.Gray)
            };
            _selectionOptionBrush = new SolidColorBrush(Colors.Yellow);
            _selectionBrush = new SolidColorBrush(Colors.Green);
            _boardLabelBrush = new SolidColorBrush(Colors.Silver);

            DrawBoard();
            RedrawGameState(Controller.GameState);
            AddLogEntries(Controller.GameState.Log);
            Controller.GetValidSelections()
                .OnValue(DrawSelectionOptions)
                .OnError(error => ShowError(error.Message));
        }

        #region Drawing

        private void DrawBoard()
        {
            #region Draw labels

            gridBoard.RowDefinitions.Add(new RowDefinition
            {
                Name = $"{nameof(gridBoard)}LabelRow",
                Height = new GridLength(1, GridUnitType.Star)
            });

            gridBoard.ColumnDefinitions.Add(new ColumnDefinition
            {
                Name = $"{nameof(gridBoard)}LabelColumn",
                Width = new GridLength(1, GridUnitType.Star)
            });

            for (var i = 1; i <= Constants.BoardSize; i++)
            {
                var rowBackground = new Image
                {
                    Source = new BitmapImage(new Uri("Images/rowLabel.png", UriKind.Relative)),
                    Stretch = Stretch.Uniform
                };

                var rowLabel = new Label
                {
                    Content = $"{i}",
                    Foreground = _boardLabelBrush,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                gridBoard.Children.Add(rowBackground);
                Grid.SetColumn(rowBackground, 0);
                Grid.SetRow(rowBackground, i);

                gridBoard.Children.Add(rowLabel);
                Grid.SetColumn(rowLabel, 0);
                Grid.SetRow(rowLabel, i);

                var colBackground = new Image
                {
                    Source = new BitmapImage(new Uri("Images/columnLabel.png", UriKind.Relative)),
                    Stretch = Stretch.Uniform
                };

                var colLabel = new Label
                {
                    Content = $"{i}",
                    Foreground = _boardLabelBrush,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                gridBoard.Children.Add(colBackground);
                Grid.SetColumn(colBackground, i);
                Grid.SetRow(colBackground, 0);

                gridBoard.Children.Add(colLabel);
                Grid.SetColumn(colLabel, i);
                Grid.SetRow(colLabel, 0);
            }

            #endregion

            #region Draw cells

            for (var i = 1; i <= Constants.BoardSize; i++)
            {
                gridBoard.RowDefinitions.Add(new RowDefinition
                {
                    Name = $"{nameof(gridBoard)}Row{i}",
                    Height = new GridLength(2, GridUnitType.Star)

                });
                gridBoard.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Name = $"{nameof(gridBoard)}Column{i}",
                    Width = new GridLength(2, GridUnitType.Star)
                });
            }

            for (var r = 1; r <= Constants.BoardSize; r++)
            {
                for (var c = 1; c <= Constants.BoardSize; c++)
                {
                    var loc = Location.Create(c, r);

                    var image = new Image
                    {
                        Source = new BitmapImage(new Uri(GetCellImagePath(loc), UriKind.Relative)),
                        Stretch = Stretch.Uniform
                    };
                    image.InputBindings.Add(GetCellClickedInputBinding(loc));

                    gridBoard.Children.Add(image);
                    Grid.SetColumn(image, c);
                    Grid.SetRow(image, r);
                }
            }

            #endregion
        }

        private string GetCellImagePath(Location location)
        {
            if (location.IsMaze())
            {
                return "Images/mazeCell.png";
            }
            else
            {
                return (location.X + location.Y) % 2 == 0
                    ? "Images/whiteCell.png"
                    : "Images/blackCell.png";
            }
        }
        
        private void RedrawGameState(GameState game)
        {
            RedrawPieces(game);
            RedrawTurnCycle(game);
            RedrawPlayers(game);
            Controller.GetValidSelections()
                .OnValue(DrawSelectionOptions)
                .OnError(error => ShowError(error.Message));
        }

        private void RedrawPieces(GameState state)
        {
            //Remove old elements
            var pieceElements = gridBoard.Children
                .OfType<FrameworkElement>()
                .Where(element => element.Name == _pieceElementName)
                .ToList();

            foreach (var element in pieceElements)
            {
                gridBoard.Children.Remove(element);
            }

            //Draw new elements
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
                    Name = _pieceElementName,
                    Fill = _playerColorBrushes[piece.Color],
                    Stretch = Stretch.Uniform,
                };
                var clickBinding = GetCellClickedInputBinding(piece.Piece.Location);
                ell.InputBindings.Add(clickBinding);

                gridBoard.Children.Add(ell);
                //-1 because grid is 0-based but game is 1-based
                Grid.SetColumn(ell, piece.Piece.Location.X);
                Grid.SetRow(ell, piece.Piece.Location.Y);
                Grid.SetZIndex(ell, 1);

                var image = new Image
                {
                    Source = new BitmapImage(new Uri(GetPieceImagePath(piece.Piece), UriKind.Relative)),
                    Name = _pieceElementName,
                    Height = 45,
                    Width = 45
                };
                image.InputBindings.Add(clickBinding);

                gridBoard.Children.Add(image);
                Grid.SetColumn(image, piece.Piece.Location.X);
                Grid.SetRow(image, piece.Piece.Location.Y);
                Grid.SetZIndex(image, 2);

                var label = new Label
                {
                    Content = piece.Piece.Id.ToString(),
                    Foreground = _boardLabelBrush,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(20, 20, 0, 0)
                };
                label.InputBindings.Add(clickBinding);

                gridBoard.Children.Add(label);
                Grid.SetColumn(label, piece.Piece.Location.X);
                Grid.SetRow(label, piece.Piece.Location.Y);
                Grid.SetZIndex(label, 3);
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

                case PieceType.Journalist:
                    return "Images/journalist.png";

                case PieceType.Thug:
                    return "Images/thug.png";

                case PieceType.Undertaker:
                    return "Images/undertaker.png";

                default:
                    throw new Exception("Invalid PieceType");
            }
        }

        private void RedrawTurnCycle(GameState state)
        {
            //Remove old elements
            var turnElements = gridTurnCycle.Children
                .OfType<FrameworkElement>()
                .Where(element => element.Name == _turnCycleElementName)
                .ToList();

            foreach (var element in turnElements)
            {
                gridTurnCycle.Children.Remove(element);
            }

            //Draw new elements
            var turnDetails = state.TurnCycle
                .Join(state.Players,
                    turnPlayerId => turnPlayerId,
                    player => player.Id,
                    (t, p) => new
                    {
                        Name = p.Name,
                        Color = p.Color
                    })
                .ToList();

            for (var i = 0; i < turnDetails.Count; i++)
            {
                gridTurnCycle.RowDefinitions.Add(new RowDefinition
                {
                    Name = $"{nameof(gridTurnCycle)}Row{i}"
                });

                var turn = turnDetails[i];

                var rect = new Rectangle
                {
                    Name = _turnCycleElementName,
                    Fill = _playerColorBrushes[turn.Color],
                    Stretch = Stretch.Uniform
                };

                gridTurnCycle.Children.Add(rect);
                Grid.SetColumn(rect, 0);
                Grid.SetRow(rect, i);

                var indexLabel = new Label
                {
                    Name = _turnCycleElementName,
                    Content = (i+1).ToString()
                };

                gridTurnCycle.Children.Add(indexLabel);
                Grid.SetColumn(indexLabel, 0);
                Grid.SetRow(indexLabel, i);

                var nameLabel = new Label
                {
                    Name = _turnCycleElementName,
                    Content = turn.Name
                };

                gridTurnCycle.Children.Add(nameLabel);
                Grid.SetColumn(nameLabel, 1);
                Grid.SetRow(nameLabel, i);
            }
        }

        private void RedrawPlayers(GameState state)
        {
            //Remove old elements
            var playerElements = gridPlayers.Children
                .OfType<FrameworkElement>()
                .Where(element => element.Name == _playerElementName)
                .ToList();

            foreach (var element in playerElements)
            {
                gridPlayers.Children.Remove(element);
            }

            //Draw new elements
            var players = state.Players;

            for (var i = 0; i < players.Count; i++)
            {
                gridPlayers.RowDefinitions.Add(new RowDefinition
                {
                    Name = $"{nameof(gridPlayers)}Row{i}"
                });

                var player = players[i];

                var rect = new Rectangle
                {
                    Name = _playerElementName,
                    Fill = _playerColorBrushes[player.Color],
                    Stretch = Stretch.Uniform
                };

                gridPlayers.Children.Add(rect);
                Grid.SetColumn(rect, 0);
                Grid.SetRow(rect, i);
                
                var nameLabel = new Label
                {
                    Name = _playerElementName,
                    Content = GetPlayerLabelText(player)
                };

                gridPlayers.Children.Add(nameLabel);
                Grid.SetColumn(nameLabel, 1);
                Grid.SetRow(nameLabel, i);
            }
        }

        private string GetPlayerLabelText(Player player)
        {
            /*
             * Formatting will look like one of the following
             *     Player1
             *     Player2 (Neutral)
             *     Player3 (Eliminated)
             *     Player4 (Neutral, Eliminated)
             */

            var sb = new StringBuilder(player.Name);

            if (!player.IsAlive || player.IsVirtual)
            {
                sb.Append(" (");

                if (player.IsVirtual)
                {
                    sb.Append("Neutral");

                    if (!player.IsAlive)
                    {
                        sb.Append(", ");
                    }
                }
                if (!player.IsAlive)
                {
                    sb.Append("Eliminated");
                }

                sb.Append(")");
            }

            return sb.ToString();
        }

        private void DrawSelectionOptions(IEnumerable<Selection> selectionOptions)
        {
            ClearError();

            foreach (var s in selectionOptions)
            {
                var rect = new Rectangle
                {
                    Name = _selectionOptionRectName,
                    Fill = _selectionOptionBrush,
                    Opacity = _selectionOpacity,
                    Stretch = Stretch.Uniform
                };

                var clickBinding = GetCellClickedInputBinding(s.Location);
                rect.InputBindings.Add(clickBinding);

                gridBoard.Children.Add(rect);
                Grid.SetColumn(rect, s.Location.X);
                Grid.SetRow(rect, s.Location.Y);
            }
        }

        private void ClearSelectionOptions()
        {
            var selectionOptions = gridBoard.Children
                .OfType<Rectangle>()
                .Where(r => r.Name == _selectionOptionRectName)
                .ToList();

            foreach (var rect in selectionOptions)
            {
                gridBoard.Children.Remove(rect);
            }
        }

        private void ClearSelections()
        {
            var selections = gridBoard.Children
                .OfType<Rectangle>()
                .Where(r => r.Name == _selectionRectName)
                .ToList();

            foreach (var rect in selections)
            {
                gridBoard.Children.Remove(rect);
            }
        }

        private void DrawSelection(Selection selection)
        {
            var rect = new Rectangle
            {
                Name = _selectionRectName,
                Fill = _selectionBrush,
                Opacity = _selectionOpacity,
                Stretch = Stretch.Uniform
            };
            var clickBinding = GetCellClickedInputBinding(selection.Location);
            rect.InputBindings.Add(clickBinding);

            gridBoard.Children.Add(rect);
            Grid.SetColumn(rect, selection.Location.X);
            Grid.SetRow(rect, selection.Location.Y);
        }

        private void EnableOrDisableConfirmButtons()
        {
            btnConfirm.IsEnabled = Controller.TurnState.Status == TurnStatus.AwaitingConfirmation;
            btnCancel.IsEnabled = Controller.TurnState.Selections.Any();
        }
        
        public void ShowError(string errorMessage)
        {
            lblError.Content = errorMessage;
        }

        private void ClearError()
        {
            lblError.Content = "";
        }

        private void AddLogEntries(IEnumerable<string> messages)
        {
            var items = listGameLog.Items;
            foreach (var m in messages)
            {
                items.Add(m);
            }
            listGameLog.ScrollIntoView(items[items.Count - 1]);
        }

        #endregion

        #region Event handlers

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

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            var previousLogCount = Controller.GameState.Log.Count;

            Controller.ConfirmTurn()
                .OnValue(game =>
                {
                    ClearSelectionOptions();
                    ClearSelections();
                    EnableOrDisableConfirmButtons();
                    RedrawGameState(game);
                    AddLogEntries(game.Log.Skip(previousLogCount));
                })
                .OnError(error => ShowError(error.Message));
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Controller.CancelTurn()
                .OnValue(_ =>
                {
                    ClearSelectionOptions();
                    ClearSelections();
                    EnableOrDisableConfirmButtons();
                    Controller.GetValidSelections()
                        .OnValue(DrawSelectionOptions)
                        .OnError(error => ShowError(error.Message));
                })
                .OnError(error => ShowError(error.Message));
        }

        private InputBinding GetCellClickedInputBinding(Location location)
        {
            var gest = new MouseGesture(MouseAction.LeftClick);
            return new InputBinding(new CellClickedCommand(this, location), gest);
        } 

        public void OnSelectionMade()
        {
            ClearSelectionOptions();
            DrawSelection(Controller.TurnState.Selections.Last());
            Controller.GetValidSelections()
                .OnValue(DrawSelectionOptions)
                .OnError(error => ShowError(error.Message));
            EnableOrDisableConfirmButtons();
        }

        #endregion
    }
}
