namespace SeaBattle_Coursework.Models
{
    public class Ship
    {
        public int Size { get; }
        public int Health { get; private set; }
        public bool IsSunk => Health <= 0;

        public bool IsHorizontal { get; set; } = true;

        public List<Cell> Cells { get; } = new List<Cell>();

        public Ship(int size)
        {
            Size = size;
            Health = size;
        }

        public void Hit()
        {
            if (Health > 0)
            {
                Health--;
            }
        }
    }
}