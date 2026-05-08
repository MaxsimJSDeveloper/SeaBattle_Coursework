using SeaBattle_Coursework.Services;
using SeaBattle_Coursework.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SeaBattle_Coursework.Views
{
    public partial class GameView : UserControl
    {
        private GameViewModel _vm;
        private readonly INavigationService _navigationService;

        public GameView(GameViewModel gameState, INavigationService navService)
        {
            InitializeComponent();
            _vm = gameState;
            this.DataContext = _vm;
            _navigationService = navService;
        }

        private async void Grid_CellClicked(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Button btn && btn.DataContext is CellViewModel cell)
            {
                if (!_vm.Player2Board.Cells.Contains(cell)) return;
                await _vm.HandlePlayerShotAsync(cell.X, cell.Y);
            }
        }
        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            _vm.IsPaused = true;
            PauseModal.Visibility = Visibility.Visible;
        }
        private void ResumeButton_Click(object sender, RoutedEventArgs e)
        {
            _vm.IsPaused = false;
            PauseModal.Visibility = Visibility.Collapsed;
        }
        private void BackToMenu_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.NavigateTo(new MenuView(_navigationService));
        }
    }
}