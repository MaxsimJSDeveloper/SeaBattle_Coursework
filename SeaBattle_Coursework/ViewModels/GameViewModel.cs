using SeaBattle_Coursework.Models;
using SeaBattle_Coursework.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace SeaBattle_Coursework.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        public BoardViewModel Player1Board { get; } = new BoardViewModel();
        public BoardViewModel Player2Board { get; } = new BoardViewModel();
        public bool IsGameOver { get; private set; }
        public bool IsBotTurn { get; private set; }
        public bool IsPaused { get; set; }

        private int _botX = 0;
        private int _botY = 0;
        private List<Cell> _botTargets = new List<Cell>();
        private List<Cell> _currentShipHits = new List<Cell>();

        private string _gameOverMessage = "";
        public string GameOverMessage
        {
            get => _gameOverMessage;
            set { _gameOverMessage = value; OnPropertyChanged(); }
        }

        private Brush _gameOverColor = Brushes.White;
        public Brush GameOverColor
        {
            get => _gameOverColor;
            set { _gameOverColor = value; OnPropertyChanged(); }
        }

        private Visibility _gameOverVisibility = Visibility.Collapsed;
        public Visibility GameOverVisibility
        {
            get => _gameOverVisibility;
            set { _gameOverVisibility = value; OnPropertyChanged(); }
        }

        public async Task HandlePlayerShotAsync(int x, int y)
        {
            if (IsGameOver || IsBotTurn || IsPaused) return;

            var result = Player2Board.LogicBoard.Shoot(x, y);
            if (result == ShotResult.AlreadyFired) return;

            SoundManager.PlaySound("cannon_shot.wav");
            if (result == ShotResult.Hit) SoundManager.PlaySound("hit.wav");
            else if (result == ShotResult.Sunk) SoundManager.PlaySound("sunk.wav");
            else if (result == ShotResult.Miss) SoundManager.PlaySound("miss.wav");

            foreach (var c in Player2Board.Cells)
            {
                if (c.Model.State == CellState.Hit || c.Model.State == CellState.Miss)
                    c.IsHidden = false;
                c.RefreshView();
            }

            if (Player2Board.LogicBoard.IsDefeated)
            {
                IsGameOver = true;
                GameOverMessage = "YOU WON!";
                GameOverColor = new SolidColorBrush(Colors.MediumSeaGreen);
                GameOverVisibility = Visibility.Visible;
                return;
            }

            if (result == ShotResult.Miss)
            {
                await RunBotTurnAsync();
            }
        }
        private async Task RunBotTurnAsync()
        {
            IsBotTurn = true;
            var board = Player1Board.LogicBoard;
            bool keepShooting = true;

            while (keepShooting && !IsGameOver)
            {
                while (IsPaused) await Task.Delay(100);
                await Task.Delay(600);
                if (IsPaused) continue;

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

                SoundManager.PlaySound("cannon_shot.wav");
                if (result == ShotResult.Hit) SoundManager.PlaySound("hit.wav");
                else if (result == ShotResult.Sunk) SoundManager.PlaySound("sunk.wav");
                else if (result == ShotResult.Miss) SoundManager.PlaySound("miss.wav");

                if (result == ShotResult.Hit)
                {
                    _currentShipHits.Add(board.Grid[targetX, targetY]);
                    UpdateBotTargets(board);
                }
                else if (result == ShotResult.Sunk)
                {
                    _currentShipHits.Clear();
                    _botTargets.Clear();
                }

                foreach (var c in Player1Board.Cells) c.RefreshView();

                if (board.IsDefeated)
                {
                    IsGameOver = true;
                    GameOverMessage = "ENEMY DESTROYED YOUR FLEET!";
                    GameOverColor = new SolidColorBrush(Colors.IndianRed);
                    GameOverVisibility = Visibility.Visible;
                    return;
                }

                if (result == ShotResult.Miss)
                {
                    keepShooting = false;
                }
            }

            IsBotTurn = false;
        }

        private void UpdateBotTargets(Board board)
        {
            _botTargets.Clear();

            if (_currentShipHits.Count == 1)
            {
                var cell = _currentShipHits[0];
                (int dx, int dy)[] directions = { (0, -1), (0, 1), (-1, 0), (1, 0) };

                foreach (var (dx, dy) in directions)
                {
                    AddTargetIfValid(cell.X + dx, cell.Y + dy, board);
                }
            }
            else if (_currentShipHits.Count > 1)
            {
                var sortedHits = _currentShipHits.OrderBy(c => c.X).ThenBy(c => c.Y).ToList();
                var first = sortedHits.First();
                var last = sortedHits.Last();

                bool isHorizontal = first.Y == last.Y;

                if (isHorizontal)
                {
                    AddTargetIfValid(first.X - 1, first.Y, board);
                    AddTargetIfValid(last.X + 1, last.Y, board);
                }
                else
                {
                    AddTargetIfValid(first.X, first.Y - 1, board);
                    AddTargetIfValid(last.X, last.Y + 1, board);
                }
            }
        }

        private void AddTargetIfValid(int x, int y, Board board)
        {
            if (x >= 0 && x < 10 && y >= 0 && y < 10)
            {
                var cell = board.Grid[x, y];
                if (cell.State != CellState.Hit && cell.State != CellState.Miss)
                {
                    if (!_botTargets.Any(c => c.X == x && c.Y == y))
                    {
                        _botTargets.Add(cell);
                    }
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}