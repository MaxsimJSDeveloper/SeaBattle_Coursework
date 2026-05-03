using System.ComponentModel;
using System.Runtime.CompilerServices;
using SeaBattle_Coursework.Models;

namespace SeaBattle_Coursework.ViewModels
{
    public class CellViewModel : INotifyPropertyChanged
    {
        public Cell Model { get; }
        public int X => Model.X;
        public int Y => Model.Y;

        private bool _isHidden;
        public bool IsHidden
        {
            get => _isHidden;
            set { _isHidden = value; OnPropertyChanged(nameof(Color)); }
        }

        private bool _isPreview;
        public bool IsPreview
        {
            get => _isPreview;
            set { _isPreview = value; OnPropertyChanged(nameof(Color)); }
        }

        private bool _isPreviewInvalid;
        public bool IsPreviewInvalid
        {
            get => _isPreviewInvalid;
            set { _isPreviewInvalid = value; OnPropertyChanged(nameof(Color)); }
        }
        public string Color
        {
            get
            {
                if (IsHidden) return "LightBlue";
                if (IsPreviewInvalid) return "LightCoral";
                if (IsPreview) return "LightGreen";

                return Model.State switch
                {
                    CellState.Ship => "Gray",
                    CellState.Hit => "Red",
                    CellState.Miss => "White",
                    _ => "LightBlue"
                };
            }
        }

        public CellViewModel(Cell model)
        {
            Model = model;
        }

        public void RefreshView()
        {
            OnPropertyChanged(nameof(Color));
        }

        // --- INotifyPropertyChanged ---
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}