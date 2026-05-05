using SeaBattle_Coursework.Services;
using SeaBattle_Coursework.Views;
using System.Windows;

namespace SeaBattle_Coursework
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Вмикаємо музику при старті
            SoundManager.PlayMusic("start_menu_music.mp3");

            MainContent.Content = new MenuView();
        }

        public void SwitchScreen(object view)
        {
            MainContent.Content = view;
        }

        // Обробник для галочки "Музика"
        private void ToggleMusic_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.ToggleMusic();
        }

        // Обробник для галочки "Звуки"
        private void ToggleEffects_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.ToggleEffects();
        }
    }
}