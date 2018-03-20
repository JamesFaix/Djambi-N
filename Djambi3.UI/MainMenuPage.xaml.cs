using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Djambi.Engine;

namespace Djambi.UI
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        private readonly Validator _validator = new Validator();

        public Page1()
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
