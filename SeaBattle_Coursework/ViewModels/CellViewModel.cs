using System;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using SeaBattle_Coursework.Models;

namespace SeaBattle_Coursework.ViewModels
{
    public class CellViewModel : INotifyPropertyChanged
    {
        public Cell Model { get; }
        private Board _logicBoard;

        public int X => Model.X;
        public int Y => Model.Y;

        private bool _isHidden;
        public bool IsHidden
        {
            get => _isHidden;
            set { _isHidden = value; RefreshView(); }
        }

        private bool _isPreview;
        public bool IsPreview
        {
            get => _isPreview;
            set { _isPreview = value; RefreshView(); }
        }

        private bool _isPreviewInvalid;
        public bool IsPreviewInvalid
        {
            get => _isPreviewInvalid;
            set { _isPreviewInvalid = value; RefreshView(); }
        }

        public CellViewModel(Cell model, Board logicBoard)
        {
            Model = model;
            _logicBoard = logicBoard;
        }

        // ЗАХИСТ ВІД NULL: Перевіряємо, чи існують взагалі кораблі
        private Ship? GetMyShip()
        {
            if (_logicBoard == null || _logicBoard.Ships == null) return null;
            return _logicBoard.Ships.FirstOrDefault(s => s.Cells.Any(c => c.X == X && c.Y == Y));
        }

        // 1. ПОВЕРТАЄМО ГОТОВИЙ ОБ'ЄКТ КАРТИНКИ (Надійна генерація)
        public ImageSource ShipImageSource
        {
            get
            {
                var ship = GetMyShip();
                string path = "pack://application:,,,/Assets/Images/boat.png";

                if (ship != null)
                {
                    path = ship.Cells.Count switch
                    {
                        4 => "pack://application:,,,/Assets/Images/battleship.png",
                        3 => "pack://application:,,,/Assets/Images/cruiser.png",
                        2 => "pack://application:,,,/Assets/Images/destroyer.png",
                        _ => "pack://application:,,,/Assets/Images/boat.png"
                    };
                }

                // Генеруємо справжню картинку, яку XAML 100% прочитає
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(path, UriKind.Absolute);
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();
                bmp.Freeze(); // Захист від витоків пам'яті та помилок потоків
                return bmp;
            }
        }

        public Rect ShipViewbox
        {
            get
            {
                var ship = GetMyShip();
                if (ship == null || ship.Cells.Count <= 1) return new Rect(0, 0, 1, 1);

                var sortedCells = ship.Cells.OrderBy(c => c.X).ThenBy(c => c.Y).ToList();

                // ВИПРАВЛЕНО: Шукаємо за координатами X та Y, а не за посиланням
                int myIndex = sortedCells.FindIndex(c => c.X == X && c.Y == Y);
                if (myIndex < 0) myIndex = 0; // Запобіжник від крашу математики

                double pieceWidth = 1.0 / ship.Cells.Count;
                return new Rect(myIndex * pieceWidth, 0, pieceWidth, 1.0);
            }
        }

        public double ShipRotation
        {
            get
            {
                var ship = GetMyShip();
                if (ship == null || ship.Cells.Count <= 1) return 0;

                bool isVertical = ship.Cells[0].X == ship.Cells[1].X;
                return isVertical ? 90 : 0;
            }
        }

        public bool HasShip
        {
            get
            {
                if (IsHidden && Model.State == CellState.Ship) return false;
                return Model.State == CellState.Ship || Model.State == CellState.Hit;
            }
        }

        public bool IsMiss => Model.State == CellState.Miss;
        public bool IsHit => Model.State == CellState.Hit;

        public Brush PreviewBackground
        {
            get
            {
                if (IsPreviewInvalid) return new SolidColorBrush(Color.FromArgb(100, 255, 0, 0));
                if (IsPreview) return new SolidColorBrush(Color.FromArgb(100, 0, 255, 0));
                return Brushes.Transparent;
            }
        }

        public void RefreshView()
        {
            OnPropertyChanged(nameof(HasShip));
            OnPropertyChanged(nameof(IsMiss));
            OnPropertyChanged(nameof(IsHit));
            OnPropertyChanged(nameof(PreviewBackground));
            OnPropertyChanged(nameof(ShipImageSource));
            OnPropertyChanged(nameof(ShipViewbox));
            OnPropertyChanged(nameof(ShipRotation));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}