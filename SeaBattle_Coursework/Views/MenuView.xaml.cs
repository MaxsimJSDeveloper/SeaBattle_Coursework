using SeaBattle_Coursework.Services;
using System.Windows;
using System.Windows.Controls;

namespace SeaBattle_Coursework.Views
{
    public partial class MenuView : UserControl
    {
        public MenuView()
        {
            InitializeComponent();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            SoundManager.PlaySound("click.wav");
            mainWindow.SwitchScreen(new SetupView());
        }
    }
}
