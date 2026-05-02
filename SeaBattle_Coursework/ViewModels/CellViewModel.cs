namespace SeaBattle_Coursework.ViewModels
{
    public class CellViewModel
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Color { get; set; } = "LightBlue";

        public CellViewModel(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}