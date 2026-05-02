using System.Collections.ObjectModel;

namespace SeaBattle_Coursework.ViewModels
{
    public class BoardViewModel
    {
        public ObservableCollection<CellViewModel> Cells { get; set; }

        public BoardViewModel()
        {
            Cells = new ObservableCollection<CellViewModel>();

            // Генеруємо сітку 10х10 (100 клітинок)
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    Cells.Add(new CellViewModel(x, y));
                }
            }
        }
    }
}