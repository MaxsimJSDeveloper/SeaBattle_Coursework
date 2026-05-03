using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SeaBattle_Coursework.ViewModels;
using SeaBattle_Coursework.Models;

namespace SeaBattle_Coursework.Views
{
    public partial class SetupView : UserControl
    {
        private List<CellViewModel> _currentPreview = new List<CellViewModel>();
        public SetupView()
        {
            InitializeComponent();
        }

        private int GetSelectedShipSize()
        {
            if (Rb4.IsChecked == true) return 4;
            if (Rb3.IsChecked == true) return 3;
            if (Rb2.IsChecked == true) return 2;
            return 1;
        }

        private void UpdateFleetUI(Board board)
        {
            Rb4.Content = $"Лінкор (4 кл) - Залишилось: {board.GetRemainingShips(4)}";
            Rb3.Content = $"Крейсер (3 кл) - Залишилось: {board.GetRemainingShips(3)}";
            Rb2.Content = $"Есмінець (2 кл) - Залишилось: {board.GetRemainingShips(2)}";
            Rb1.Content = $"Катер (1 кл) - Залишилось: {board.GetRemainingShips(1)}";

            PlayButton.IsEnabled = board.Ships.Count == 10;
        }

        private void ClearPreview()
        {
            foreach (var c in _currentPreview)
            {
                c.IsPreview = false;
                c.IsPreviewInvalid = false;
                c.RefreshView();
            }
            _currentPreview.Clear();
        }
        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.OriginalSource is Button btn && btn.DataContext is CellViewModel cell)
            {
                var vm = (GameViewModel)this.DataContext;
                if (!vm.Player1Board.Cells.Contains(cell)) return;

                int size = GetSelectedShipSize();
                bool isHorizontal = ChkHorizontal.IsChecked == true;
                var board = vm.Player1Board.LogicBoard;

                bool canPlace = board.CanPlaceShip(cell.X, cell.Y, size, isHorizontal);
                bool hasStock = board.GetRemainingShips(size) > 0;
                bool isValid = canPlace && hasStock;

                for (int i = 0; i < size; i++)
                {
                    int x = isHorizontal ? cell.X + i : cell.X;
                    int y = isHorizontal ? cell.Y : cell.Y + i;

                    if (x < 10 && y < 10)
                    {
                        var targetCell = vm.Player1Board.Cells.First(c => c.X == x && c.Y == y);
                        targetCell.IsPreview = isValid;
                        targetCell.IsPreviewInvalid = !isValid;
                        targetCell.RefreshView();
                        _currentPreview.Add(targetCell);
                    }
                }
            }
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            ClearPreview();
        }

        private void Grid_CellClicked(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Button btn && btn.DataContext is CellViewModel cell)
            {
                var vm = (GameViewModel)this.DataContext;
                if (!vm.Player1Board.Cells.Contains(cell)) return;

                int size = GetSelectedShipSize();
                var board = vm.Player1Board.LogicBoard;

                if (board.GetRemainingShips(size) <= 0) return;

                var newShip = new Ship(size);
                if (board.PlaceShip(newShip, cell.X, cell.Y, ChkHorizontal.IsChecked == true))
                {
                    foreach (var c in vm.Player1Board.Cells) c.RefreshView();
                    UpdateFleetUI(board);

                    ClearPreview();
                }
            }
        }

        private void Grid_RightClick(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement element && element.DataContext is CellViewModel cell)
            {
                var vm = (GameViewModel)this.DataContext;
                var board = vm.Player1Board.LogicBoard;

                var shipToRemove = board.Ships.FirstOrDefault(s => s.Cells.Any(c => c.X == cell.X && c.Y == cell.Y));

                if (shipToRemove != null)
                {
                    board.RemoveShip(shipToRemove);
                    foreach (var c in vm.Player1Board.Cells) c.RefreshView();
                    UpdateFleetUI(board);

                    ClearPreview();
                }
            }
        }

        private void StartBattle_Click(object sender, RoutedEventArgs e)
        {
            var currentGameState = (GameViewModel)this.DataContext;

            var botBoard = currentGameState.Player2Board.LogicBoard;
            botBoard.AutoPlaceAllShips();

            foreach (var cell in currentGameState.Player2Board.Cells)
            {
                cell.RefreshView();
                cell.IsHidden = true;
            }

            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.SwitchScreen(new GameView(currentGameState));
        }
    }
}