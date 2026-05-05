using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // Обов'язково для async/await
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SeaBattle_Coursework.Models;
using SeaBattle_Coursework.Services;
using SeaBattle_Coursework.ViewModels;

namespace SeaBattle_Coursework.Views
{
    public partial class GameView : UserControl
    {
        private int _botX = 0;
        private int _botY = 0;
        private List<Cell> _botTargets = new List<Cell>();

        private bool _isGameOver = false;
        private bool _isBotTurn = false; // Блокуємо UI, поки бот стріляє

        public GameView(GameViewModel gameState)
        {
            InitializeComponent();
            this.DataContext = gameState;
        }

        // Робимо метод async, щоб мати змогу викликати await
        private async void Grid_CellClicked(object sender, RoutedEventArgs e)
        {
            // Якщо гра закінчена або зараз хід бота — ігноруємо кліки!
            if (_isGameOver || _isBotTurn) return;

            if (e.OriginalSource is Button btn && btn.DataContext is CellViewModel cell)
            {
                var vm = (GameViewModel)this.DataContext;
                if (!vm.Player2Board.Cells.Contains(cell)) return;

                var result = vm.Player2Board.LogicBoard.Shoot(cell.X, cell.Y);
                if (result == ShotResult.AlreadyFired) return;

                // Звуки гравця
                SoundManager.PlaySound("cannon_shot.wav");
                if (result == ShotResult.Hit) SoundManager.PlaySound("hit.wav");
                else if (result == ShotResult.Sunk) SoundManager.PlaySound("sunk.wav");
                else if (result == ShotResult.Miss) SoundManager.PlaySound("miss.wav");

                foreach (var c in vm.Player2Board.Cells)
                {
                    if (c.Model.State == CellState.Hit || c.Model.State == CellState.Miss)
                        c.IsHidden = false;
                    c.RefreshView();
                }

                if (vm.Player2Board.LogicBoard.IsDefeated)
                {
                    ShowGameOver(true);
                    return;
                }

                // Якщо гравець промахнувся — черга бота
                if (result == ShotResult.Miss)
                {
                    await BotTurnAsync(vm); // Викликаємо асинхронно!
                }
            }
        }

        // Переробили на async Task
        private async Task BotTurnAsync(GameViewModel vm)
        {
            _isBotTurn = true; // Блокуємо кліки гравця
            var board = vm.Player1Board.LogicBoard;
            bool keepShooting = true;

            while (keepShooting && !_isGameOver)
            {
                // СИМУЛЯЦІЯ "ДУМАННЯ" БОТА (600 мілісекунд)
                await Task.Delay(600);

                int targetX = -1, targetY = -1;

                if (_botTargets.Count > 0)
                {
                    var target = _botTargets[0];
                    _botTargets.RemoveAt(0);
                    targetX = target.X;
                    targetY = target.Y;
                }
                else
                {
                    if (_botY <= 9)
                    {
                        targetX = _botX;
                        targetY = _botY;
                        _botX += 2;
                        if (_botX > 9)
                        {
                            _botY++;
                            _botX = _botY % 2 == 0 ? 0 : 1;
                        }
                    }
                    else
                    {
                        bool found = false;
                        for (int y = 0; y < 10 && !found; y++)
                        {
                            for (int x = 0; x < 10 && !found; x++)
                            {
                                if (board.Grid[x, y].State != CellState.Hit && board.Grid[x, y].State != CellState.Miss)
                                {
                                    targetX = x; targetY = y;
                                    found = true;
                                }
                            }
                        }
                        if (!found) break;
                    }
                }

                var result = board.Shoot(targetX, targetY);
                if (result == ShotResult.AlreadyFired) continue;

                // --- ЗВУКИ БОТА ---
                SoundManager.PlaySound("cannon_shot.wav");
                if (result == ShotResult.Hit) SoundManager.PlaySound("hit.wav");
                else if (result == ShotResult.Sunk) SoundManager.PlaySound("sunk.wav");
                else if (result == ShotResult.Miss) SoundManager.PlaySound("miss.wav");

                if (result == ShotResult.Hit) AddBotTargets(targetX, targetY, board);
                else if (result == ShotResult.Sunk) _botTargets.Clear();

                foreach (var c in vm.Player1Board.Cells) c.RefreshView();

                if (board.IsDefeated)
                {
                    ShowGameOver(false);
                    return;
                }

                if (result == ShotResult.Miss)
                {
                    keepShooting = false;
                }
            }

            _isBotTurn = false; // Розблоковуємо UI, хід повертається гравцю
        }

        private void AddBotTargets(int x, int y, Board board)
        {
            int[][] directions = { new[] { 0, -1 }, new[] { 0, 1 }, new[] { -1, 0 }, new[] { 1, 0 } };
            foreach (var dir in directions)
            {
                int nx = x + dir[0]; int ny = y + dir[1];
                if (nx >= 0 && nx < 10 && ny >= 0 && ny < 10)
                {
                    var cell = board.Grid[nx, ny];
                    if (cell.State != CellState.Hit && cell.State != CellState.Miss)
                    {
                        if (!_botTargets.Any(c => c.X == nx && c.Y == ny)) _botTargets.Add(cell);
                    }
                }
            }
        }

        private void ShowGameOver(bool playerWon)
        {
            _isGameOver = true;
            GameOverText.Text = playerWon ? "ВИ ПЕРЕМОГЛИ!" : "ВОРОГ ЗНИЩИВ ВАШ ФЛОТ!";
            GameOverText.Foreground = playerWon ? new SolidColorBrush(Colors.MediumSeaGreen) : new SolidColorBrush(Colors.IndianRed);
            GameOverModal.Visibility = Visibility.Visible;
        }

        private void BackToMenu_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.SwitchScreen(new MenuView());
        }
    }
}