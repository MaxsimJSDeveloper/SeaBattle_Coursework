using System.Collections.ObjectModel;
using SeaBattle_Coursework.Models;

namespace SeaBattle_Coursework.ViewModels
{
    public class BoardViewModel
    {
        public Board LogicBoard { get; }
        public ObservableCollection<CellViewModel> Cells { get; set; }

        public BoardViewModel()
        {
            LogicBoard = new Board();
            Cells = new ObservableCollection<CellViewModel>();

            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    Cells.Add(new CellViewModel(LogicBoard.Grid[x, y]));
                }
            }
        }
    }
}