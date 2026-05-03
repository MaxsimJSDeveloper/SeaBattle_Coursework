using System;

namespace SeaBattle_Coursework.Models
{
    public class Board
    {
        private readonly int[] _maxShips = { 0, 4, 3, 2, 1 };

        public Cell[,] Grid { get; private set; }

        public Board()
        {
            Grid = new Cell[10, 10];

            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    Grid[x, y] = new Cell(x, y);
                }
            }
        }

        public int GetRemainingShips(int size)
        {
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
            if (isHorizontal && startX + size > 10) return false;
            if (!isHorizontal && startY + size > 10) return false;

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
            var rand = new Random();
            int[] shipsToPlace = { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };

            foreach (int size in shipsToPlace)
            {
                bool placed = false;
                while (!placed)
                {
                    int x = rand.Next(0, 10);
                    int y = rand.Next(0, 10);
                    bool isHorizontal = rand.Next(0, 2) == 0;

                    var ship = new Ship(size);
                    placed = PlaceShip(ship, x, y, isHorizontal);
                }
            }
        }
    }
}