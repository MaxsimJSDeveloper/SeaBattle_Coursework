using SeaBattle_Coursework.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SeaBattle_Coursework.Views
{
    /// <summary>
    /// Interaction logic for SetupView.xaml
    /// </summary>
    public partial class SetupView : UserControl
    {
        public SetupView()
        {
            InitializeComponent();
        }

        private void StartBattle_Click(object sender, RoutedEventArgs e)
        {
            // 1. Беремо наш поточний стан (з розставленими кораблями)
            var currentGameState = (GameViewModel)this.DataContext;

            // 2. Отримуємо доступ до головного вікна
            var mainWindow = (MainWindow)Application.Current.MainWindow;

            // 3. Створюємо екран бою і ПЕРЕДАЄМО йому наш стан!
            var gameView = new GameView(currentGameState);
            mainWindow.SwitchScreen(gameView);
        }
    }
}
