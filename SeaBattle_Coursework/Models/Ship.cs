using System.Collections.Generic;
using System.Linq;

namespace SeaBattle_Coursework.Models
{
    public class Ship
    {
        public int Size { get; }
        public int Health { get; private set; }
        public bool IsSunk => Health <= 0;

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