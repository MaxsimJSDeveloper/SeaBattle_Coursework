using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SeaBattle_Coursework.Models;
using SeaBattle_Coursework.ViewModels;

namespace SeaBattle_Coursework.Views
{
    public partial class GameView : UserControl
    {
        private int _botX = 0;
        private int _botY = 0;
        private List<Cell> _botTargets = new List<Cell>();
        private bool _isGameOver = false; // Блокуємо кліки, якщо гра закінчилась

        public GameView(GameViewModel gameState)
        {
            InitializeComponent();
            this.DataContext = gameState;
        }

        private void Grid_CellClicked(object sender, RoutedEventArgs e)
        {
            if (_isGameOver) return; // Якщо гра завершена - ігноруємо кліки

            if (e.OriginalSource is Button btn && btn.DataContext is CellViewModel cell)
            {
                var vm = (GameViewModel)this.DataContext;
                if (!vm.Player2Board.Cells.Contains(cell)) return;

                var result = vm.Player2Board.LogicBoard.Shoot(cell.X, cell.Y);
                if (result == ShotResult.AlreadyFired) return;

                foreach (var c in vm.Player2Board.Cells)
                {
                    if (c.Model.State == CellState.Hit || c.Model.State == CellState.Miss)
                        c.IsHidden = false;
                    c.RefreshView();
                }

                // ПЕРЕВІРКА ПЕРЕМОГИ ГРАВЦЯ
                if (vm.Player2Board.LogicBoard.IsDefeated)
                {
                    ShowGameOver(true);
                    return;
                }

                // ПРАВИЛО 1: Якщо гравець ПРОМАХНУВСЯ — хід переходить боту.
                // Якщо влучив/вбив — цей блок пропускається, і гравець клікає знову!
                if (result == ShotResult.Miss)
                {
                    BotTurn(vm);
                }
            }
        }

        private void BotTurn(GameViewModel vm)
        {
            var board = vm.Player1Board.LogicBoard;
            bool keepShooting = true;

            // Бот буде стріляти, поки не промахнеться (або поки не виграє)
            while (keepShooting && !_isGameOver)
            {
                int targetX = -1, targetY = -1;

                // Режим 1: Полювання
                if (_botTargets.Count > 0)
                {
                    var target = _botTargets[0];
                    _botTargets.RemoveAt(0);
                    targetX = target.X;
                    targetY = target.Y;
                }
                // Режим 2: Розвідка
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
                    else // ПЛАН "Б" для бота: якщо шахматка закінчилась, просто б'є у першу вільну клітинку
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
                        if (!found) return; // Усі клітинки розстріляні (теоретично неможливо, якщо гра не закінчилась)
                    }
                }

                var result = board.Shoot(targetX, targetY);
                if (result == ShotResult.AlreadyFired) continue; // Бот спробує ще раз миттєво

                if (result == ShotResult.Hit) AddBotTargets(targetX, targetY, board);
                else if (result == ShotResult.Sunk) _botTargets.Clear();

                foreach (var c in vm.Player1Board.Cells) c.RefreshView();

                // ПЕРЕВІРКА ПЕРЕМОГИ БОТА
                if (board.IsDefeated)
                {
                    ShowGameOver(false);
                    return;
                }

                // Якщо бот промахнувся — він віддає хід
                if (result == ShotResult.Miss)
                {
                    keepShooting = false;
                }
            }
        }

        // ... Залиш свій старий метод AddBotTargets тут без змін ...
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

        // --- ЛОГІКА МОДАЛЬНОГО ВІКНА ---
        private void ShowGameOver(bool playerWon)
        {
            _isGameOver = true;
            GameOverText.Text = playerWon ? "ВИ ПЕРЕМОГЛИ!" : "ВОРОГ ЗНИЩИВ ВАШ ФЛОТ";
            GameOverText.Foreground = playerWon ? new SolidColorBrush(Colors.MediumSeaGreen) : new SolidColorBrush(Colors.IndianRed);
            GameOverModal.Visibility = Visibility.Visible;
        }

        private void BackToMenu_Click(object sender, RoutedEventArgs e)
        {
            // Повертаємось у меню
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.SwitchScreen(new MenuView());
        }
    }
}