namespace SeaBattle_Coursework.Models
{
    public class Ship
    {
        public int Size { get; private set; }
        public List<Cell> OccupiedCells { get; private set; }

        public Ship(int size)
        {
            Size = size;
            OccupiedCells = new List<Cell>();
        }
        public bool IsSunk()
        {
            foreach (var cell in OccupiedCells)
            {
                if (cell.State != CellState.Hit)
                    return false;
            }
            return true;
        }
    }
}