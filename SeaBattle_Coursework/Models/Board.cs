namespace SeaBattle_Coursework.Models
{
    public class Board
    {
        public int[] _maxShips = { 0, 4, 3, 2, 1 };
        public const int Size = 10;
        public Cell[,] Grid { get; private set; }

        public Board()
        {
            Grid = new Cell[Size, Size];

            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    Grid[x, y] = new Cell(x, y);
                }
            }
        }

        public int GetRemainingShips(int size)
        {
            if (size < 1 || size >= _maxShips.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(size), $"Розмір корабля має бути від 1 до {_maxShips.Length - 1}.");
            }

            return _maxShips[size] - Ships.Count(s => s.Size == size);
        }

        public void RemoveShip(Ship ship)
        {
            Ships.Remove(ship);
            foreach (var cell in ship.Cells)
            {
                cell.State = CellState.Empty;
            }
            ship.Cells.Clear();
        }
        public List<Ship> Ships { get; } = new List<Ship>();

        public bool CanPlaceShip(int startX, int startY, int size, bool isHorizontal)
        {
            if (isHorizontal && startX + size > Size) return false;
            if (!isHorizontal && startY + size > Size) return false;

            int minX = Math.Max(0, startX - 1);
            int maxX = Math.Min(9, isHorizontal ? startX + size : startX + 1);
            int minY = Math.Max(0, startY - 1);
            int maxY = Math.Min(9, isHorizontal ? startY + 1 : startY + size);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (Grid[x, y].State == CellState.Ship) return false;
                }
            }

            return true;
        }

        public bool PlaceShip(Ship ship, int startX, int startY, bool isHorizontal)
        {
            if (!CanPlaceShip(startX, startY, ship.Size, isHorizontal))
                return false;

            for (int i = 0; i < ship.Size; i++)
            {
                int x = isHorizontal ? startX + i : startX;
                int y = isHorizontal ? startY : startY + i;

                Grid[x, y].State = CellState.Ship;

                ship.Cells.Add(Grid[x, y]);
            }

            Ships.Add(ship);
            return true;
        }

        public void AutoPlaceAllShips()
        {
            Random rand = new Random();
            int[] shipsToPlace = { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };

            const int MaxAttempts = 1000;

            foreach (int size in shipsToPlace)
            {
                bool placed = false;

                for (int attempt = 0; attempt < MaxAttempts && !placed; attempt++)
                {
                    int x = rand.Next(0, 10);
                    int y = rand.Next(0, 10);
                    bool isHorizontal = rand.Next(2) == 0;

                    var newShip = new Ship(size) { IsHorizontal = isHorizontal };
                    placed = PlaceShip(newShip, x, y, isHorizontal);
                }

                if (!placed)
                {
                    throw new InvalidOperationException($"Critical Error: Could not place ship of size {size} after {MaxAttempts} attempts. The board is blocked.");
                }
            }
        }

        public ShotResult Shoot(int x, int y)
        {
            var cell = Grid[x, y];

            if (cell.State == CellState.Hit || cell.State == CellState.Miss)
                return ShotResult.AlreadyFired;

            if (cell.State == CellState.Ship)
            {
                cell.State = CellState.Hit;

                var ship = Ships.FirstOrDefault(s => s.Cells.Contains(cell));
                if (ship != null)
                {
                    ship.Hit();
                    if (ship.IsSunk)
                    {
                        MarkWaterAroundSunkShip(ship);
                        return ShotResult.Sunk;
                    }
                }
                return ShotResult.Hit;
            }
            else
            {
                cell.State = CellState.Miss;
                return ShotResult.Miss;
            }
        }
        private void MarkWaterAroundSunkShip(Ship ship)
        {
            foreach (var cell in ship.Cells)
            {
                int minX = Math.Max(0, cell.X - 1);
                int maxX = Math.Min(9, cell.X + 1);
                int minY = Math.Max(0, cell.Y - 1);
                int maxY = Math.Min(9, cell.Y + 1);

                for (int x = minX; x <= maxX; x++)
                {
                    for (int y = minY; y <= maxY; y++)
                    {
                        if (Grid[x, y].State == CellState.Empty)
                        {
                            Grid[x, y].State = CellState.Miss;
                        }
                    }
                }
            }
        }
        public bool IsDefeated => Ships.All(s => s.IsSunk);
    }
}