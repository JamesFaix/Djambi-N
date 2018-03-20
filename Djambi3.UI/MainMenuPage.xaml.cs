using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Djambi.Engine;

namespace Djambi.UI
{
    public partial class MainMenuPage : Page
    {
        private readonly Validator _validator = new Validator();

        public MainMenuPage()
        {
            InitializeComponent();
        }

        private void btnAddPlayer_Click(object sender, RoutedEventArgs e)
        {
            var name = textPlayerName.Text;
            var existingNames = listPlayerNames.Items.OfType<object>()
                .Select(o => o.ToString());

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
            this.NavigationService.Navigate(new Uri("GamePage.xaml", UriKind.Relative));
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
