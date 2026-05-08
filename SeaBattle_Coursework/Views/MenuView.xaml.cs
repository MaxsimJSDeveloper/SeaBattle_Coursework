using System.Windows;
using System.Windows.Controls;
using SeaBattle_Coursework.Services;

namespace SeaBattle_Coursework.Views
{
    public partial class MenuView : UserControl
    {
        private readonly INavigationService _navigationService;
        public MenuView(INavigationService navService)
        {
            InitializeComponent();
            _navigationService = navService;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.PlaySound("click.wav");
            _navigationService.NavigateTo(new SetupView(_navigationService));
        }
    }
}