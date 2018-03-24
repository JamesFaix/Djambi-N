using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Djambi.Engine;
using Djambi.Engine.Extensions;
using Djambi.Engine.Services;
using System.Windows.Input;


namespace Djambi.UI.Pages
{
    public partial class MainMenuPage : Page
    {
        private readonly ValidationService _validationService;
        private readonly GameInitializationService _gameInitializationService;

        public MainMenuPage()
        {
            _validationService = new ValidationService();
            _gameInitializationService = new GameInitializationService();
            InitializeComponent();            
#if DEBUG
            listPlayerNames.Items.Add("Mario");
            listPlayerNames.Items.Add("Luigi");
            OnPlayerCountChanged();
#endif
        }

        private IEnumerable<string> GetPlayerNames() =>
            listPlayerNames.Items.OfType<object>()
                .Select(o => o.ToString());

        private void btnAddPlayer_Click(object sender, RoutedEventArgs e)
        {
            var name = textPlayerName.Text;
            var existingNames = GetPlayerNames();

            var validationResult = _validationService.ValidateNewPlayerName(existingNames, name);

            if (validationResult.HasValue)
            {
                listPlayerNames.Items.Add(name);
                lblNameValidation.Content = string.Empty;
                OnPlayerCountChanged();
            }
            else
            {
                lblNameValidation.Content = validationResult.Error.Message;
            }
        }

        private void btnRemovePlayer_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = listPlayerNames.SelectedIndex;
            if (selectedIndex >= 0)
            {
                listPlayerNames.Items.RemoveAt(selectedIndex);
            }

            OnPlayerCountChanged();
        }

        private void btnStartGame_Click(object sender, RoutedEventArgs e)
        {
            Controller.StartGame(GetPlayerNames())
                .OnValue(state =>
                {
                    var win = Window.GetWindow(this) as MainWindow;
                    win.NavigateToGame();
                })
                .OnError(error =>
                {
                    lblNameValidation.Content = error.Message;
                    btnStartGame.IsEnabled = false;
                });
        }

        private void txtEnterName_Click(object sender, MouseButtonEventArgs e)
        {
            textPlayerName.Text = "";
        }

        private void OnPlayerCountChanged()
        {
            var playerCount = listPlayerNames.Items.Count;
            btnAddPlayer.IsEnabled = playerCount < Constants.MaxPlayerCount;
            btnRemovePlayer.IsEnabled = playerCount > 0;
            btnStartGame.IsEnabled = playerCount >= Constants.MinPlayerCount && playerCount <= Constants.MaxPlayerCount;
        }
    }
}
