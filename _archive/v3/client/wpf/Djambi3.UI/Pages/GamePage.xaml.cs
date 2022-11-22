using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Djambi.Engine;
using Djambi.Engine.Extensions;
using Djambi.Model;
using Djambi.UI.Resources;

namespace Djambi.UI.Pages
{
    public partial class GamePage : Page
    {
        private readonly IImageRepository _imageRepository;
        private readonly IBrushRepository _brushRepository;
        private readonly IThemeLoader _themeLoader;

        private const double _selectionOpacity = 0.5;
        private const string _selectionOptionRectName = "SelectionOption";
        private const string _selectionRectName = "Selection";
        private const string _pieceElementName = "Piece";
        private const string _turnCycleElementName = "Turn";
        private const string _playerElementName = "Player";

        public GamePage(
            IThemeLoader themeLoader,
            IImageRepository imageRepository,
            IBrushRepository brushRepository)
        {
            _themeLoader = themeLoader;
            _imageRepository = imageRepository;
            _brushRepository = brushRepository;

            InitializeComponent();

            RedrawGameState(Controller.GameState);
            AddLogEntries(Controller.GameState.Log);
            Controller.GetValidSelections()
                .OnValue(DrawSelectionOptions)
                .OnError(error => ShowError(error.Message));
        }

        #region Drawing

        private void AddToGrid(Grid grid, UIElement element, int row, int column)
        {
            grid.Children.Add(element);
            Grid.SetColumn(element, column);
            Grid.SetRow(element, row);
        }

        private void AddToGrid(Grid grid, UIElement element, Location location)
        {
            grid.Children.Add(element);
            Grid.SetColumn(element, location.X);
            Grid.SetRow(element, location.Y);
        }

        private void DrawBoard()
        {
            gridBoard.RowDefinitions.Clear();
            gridBoard.ColumnDefinitions.Clear();
            gridBoard.Children.Clear();

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

            for (var i = 1; i <= Constants.BoardSize; i++)
            {
                AddBoardLabel(i, 0, i);
                AddBoardLabel(i, i, 0);
            }

            for (var r = 1; r <= Constants.BoardSize; r++)
            {
                for (var c = 1; c <= Constants.BoardSize; c++)
                {
                    var loc = Location.Create(c, r);

                    var rect = new Rectangle
                    {
                        Stretch = Stretch.Uniform,
                        Fill = _brushRepository.GetCellBrush(loc)
                    };
                    rect.InputBindings.Add(GetCellClickedInputBinding(loc));
                    AddToGrid(gridBoard, rect, r, c);
                }
            }
        }
        
        private void AddBoardLabel(int number, int row, int column)
        {
            var background = new Rectangle
            {
                Fill = _brushRepository.BoardLabelBackgroundBrush,
                Stretch = Stretch.UniformToFill
            };

            var label = new Label
            {
                Content = $"{number}",
                Foreground = _brushRepository.BoardLabelBrush,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            AddToGrid(gridBoard, background, row, column);
            AddToGrid(gridBoard, label, row, column);
        }

        private void RedrawGameState(GameState game)
        {
            DrawBoard();
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
                var background = new Ellipse
                {
                    Name = _pieceElementName,
                    Fill = _brushRepository.GetPlayerBrush(piece.Color),
                    Stretch = Stretch.Uniform,
                };

                var image = new Image
                {
                    Source = _imageRepository.GetPieceImage(piece.Piece),
                    Name = _pieceElementName,
                    Height = 45,
                    Width = 45
                };
#if DEBUG
                var label = new Label
                {
                    Name = _pieceElementName,
                    Content = piece.Piece.Id.ToString(),
                    Foreground = _brushRepository.BoardLabelBrush,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(20, 20, 0, 0)
                };
#endif
                var clickBinding = GetCellClickedInputBinding(piece.Piece.Location);
                background.InputBindings.Add(clickBinding);
                image.InputBindings.Add(clickBinding);
#if DEBUG
                label.InputBindings.Add(clickBinding);
#endif
                AddToGrid(gridBoard, background, piece.Piece.Location);
                Grid.SetZIndex(background, 1);              

                AddToGrid(gridBoard, image, piece.Piece.Location);
                Grid.SetZIndex(image, 2);
#if DEBUG
                AddToGrid(gridBoard, label, piece.Piece.Location);
                Grid.SetZIndex(label, 3);
#endif
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

                var indexBackground = new Rectangle
                {
                    Name = _turnCycleElementName,
                    Fill = _brushRepository.GetPlayerBrush(turn.Color),
                    Stretch = Stretch.Uniform
                };
                
                var indexLabel = new Label
                {
                    Name = _turnCycleElementName,
                    Foreground = _brushRepository.BoardLabelBrush,
                    Content = (i+1).ToString()
                };

                var nameLabel = new Label
                {
                    Name = _turnCycleElementName,
                    Content = turn.Name
                };
                
                AddToGrid(gridTurnCycle, indexBackground, i, 0);
                AddToGrid(gridTurnCycle, indexLabel, i, 0);
                AddToGrid(gridTurnCycle, nameLabel, i, 1);
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

                var colorSquare = new Rectangle
                {
                    Name = _playerElementName,
                    Fill = _brushRepository.GetPlayerBrush(player.Color),
                    Stretch = Stretch.Uniform
                };
                                
                var nameLabel = new Label
                {
                    Name = _playerElementName,
                    Content = GetPlayerLabelText(player)
                };

                AddToGrid(gridPlayers, colorSquare, i, 0);
                AddToGrid(gridPlayers, nameLabel, i, 1);
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
                    Fill = _brushRepository.SelectionOptionBrush,
                    Opacity = _selectionOpacity,
                    Stretch = Stretch.Uniform
                };

                var clickBinding = GetCellClickedInputBinding(s.Location);
                rect.InputBindings.Add(clickBinding);

                AddToGrid(gridBoard, rect, s.Location);
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
                Fill = _brushRepository.SelectionOptionBrush,
                Opacity = _selectionOpacity,
                Stretch = Stretch.Uniform
            };
            var clickBinding = GetCellClickedInputBinding(selection.Location);
            rect.InputBindings.Add(clickBinding);

            AddToGrid(gridBoard, rect, selection.Location);
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
            //Reset to previous width so the whole window doesn't expand when long messages are added.
            var width = txtGameLog.Width;
            var newText = string.Join("\n", messages);            
            txtGameLog.Text = $"{txtGameLog.Text}\n{newText}".Trim();
            txtGameLog.Width = width;
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
                var win = Window.GetWindow(this) as MainWindow;
                win.NavigateToMainMenu();
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

        private void cmbTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (ComboBoxItem)cmbTheme.SelectedItem;
            var themeName = (string)selectedItem.Content;

            if (themeName != null) // null during page initialization
            {
                _themeLoader.LoadTheme(themeName);
                RedrawGameState(Controller.GameState);
            }
        }

        #endregion
    }
}
