using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Djambi.Engine;

namespace Djambi.UI
{
    public partial class MainMenuPage : Page
    {
        private readonly Validator _validator;
        private readonly GameStateInitializer _modelFactory;

        public MainMenuPage()
        {
            _validator = new Validator();
            _modelFactory = new GameStateInitializer();
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

            var validationResult = _validator.ValidateNewPlayerName(existingNames, name);

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
            _modelFactory
                .InitializeGame(GetPlayerNames())
                .OnValue(state => 
                {
                    StateManager.SetGameState(state);
                    StateManager.SetInteractionState(InteractionState.AwaitingSubjectSelection);
                    this.NavigationService.Navigate(new Uri("GamePage.xaml", UriKind.Relative));
                })
                .OnError(error =>
                {
                    lblNameValidation.Content = error.Message;
                    btnStartGame.IsEnabled = false;
                });
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
