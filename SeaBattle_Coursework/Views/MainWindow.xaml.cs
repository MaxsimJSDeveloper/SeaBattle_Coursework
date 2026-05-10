using SeaBattle_Coursework.Services;
using SeaBattle_Coursework.Views;
using System.Windows;

namespace SeaBattle_Coursework
{
    public partial class MainWindow : Window, INavigationService
    {
        public MainWindow()
        {
            InitializeComponent();
            SoundManager.PlayMusic("start_menu_music.mp3");
            NavigateTo(new MenuView(this));
        }
        public void NavigateTo(object view)
        {
            MainContent.Content = view;
        }
        private void ToggleMusic_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.ToggleMusic();
        }
        private void ToggleEffects_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.ToggleEffects();
        }
    }
}