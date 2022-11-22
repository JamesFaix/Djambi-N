using System.Windows;
using Djambi.UI.Pages;
using Djambi.UI.Resources;

namespace Djambi.UI
{
    public partial class MainWindow : Window
    {
        private readonly IImageRepository _imageRepository;
        private readonly PageFactory _pageFactory;

        public MainWindow(
            IImageRepository imageRepository,
            PageFactory pageFactory)
        {
            _imageRepository = imageRepository;
            _pageFactory = pageFactory;

            InitializeComponent();
            NavigateToMainMenu();

            Icon = _imageRepository.AppIcon;
        }

        private void NavigateToPage(string pageName) =>
            frameMain.NavigationService.Navigate(_pageFactory(pageName));

        public void NavigateToMainMenu() => 
            NavigateToPage(nameof(MainMenuPage));

        public void NavigateToGame() =>
            NavigateToPage(nameof(GamePage));
        
    }
}
