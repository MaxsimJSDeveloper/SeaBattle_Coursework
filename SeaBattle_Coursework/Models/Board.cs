namespace SeaBattle_Coursework.Models
{
    public class Board
    {
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
    }
}