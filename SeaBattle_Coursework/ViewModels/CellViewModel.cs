using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using SeaBattle_Coursework.Models;

namespace SeaBattle_Coursework.ViewModels
{
    public class CellViewModel : INotifyPropertyChanged
    {
        public Cell Model { get; }
        private readonly Board _logicBoard;
        public int X => Model.X;
        public int Y => Model.Y;

        private bool _isHidden;
        public bool IsHidden
        {
            get => _isHidden;
            set
            {
                if (_isHidden != value)
                {
                    _isHidden = value;
                    OnPropertyChanged(nameof(HasShip));
                }
            }
        }

        private bool _isPreview;
        public bool IsPreview
        {
            get => _isPreview;
            set => _isPreview = value; 
        }

        private bool _isPreviewInvalid;
        public bool IsPreviewInvalid
        {
            get => _isPreviewInvalid;
            set => _isPreviewInvalid = value;
        }

        public CellViewModel(Cell model, Board logicBoard)
        {
            Model = model;
            _logicBoard = logicBoard;
        }

        public void SetPreviewState(bool isPreview, bool isInvalid)
        {
            _isPreview = isPreview;
            _isPreviewInvalid = isInvalid;
            OnPropertyChanged(nameof(PreviewBackground));
        }

        private Ship? _cachedShip;
        private int _lastShipCount = -1;
        private Ship? GetMyShip()
        {
            if (_logicBoard == null || _logicBoard.Ships == null) return null;

            if (_lastShipCount != _logicBoard.Ships.Count)
            {
                _cachedShip = _logicBoard.Ships.FirstOrDefault(s => s.Cells.Any(c => c.X == X && c.Y == Y));
                _lastShipCount = _logicBoard.Ships.Count;
            }

            return _cachedShip;
        }

        private static readonly Dictionary<string, BitmapImage> _imageCache = new Dictionary<string, BitmapImage>();

        private static BitmapImage GetCachedImage(string path)
        {
            if (_imageCache.TryGetValue(path, out var cachedImage))
                return cachedImage;

            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(path, UriKind.Absolute);
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.EndInit();
            bmp.Freeze();

            _imageCache[path] = bmp;
            return bmp;
        }
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

                return GetCachedImage(path);
            }
        }

        public Rect ShipViewbox
        {
            get
            {
                var ship = GetMyShip();
                if (ship == null || ship.Cells.Count <= 1) return new Rect(0, 0, 1, 1);

                var sortedCells = ship.Cells.OrderBy(c => c.X).ThenBy(c => c.Y).ToList();

                int myIndex = sortedCells.FindIndex(c => c.X == X && c.Y == Y);
                if (myIndex < 0) myIndex = 0;

                double pieceWidth = 1.0 / ship.Cells.Count;
                return new Rect(myIndex * pieceWidth, 0, pieceWidth, 1.0);
            }
        }

        public double ShipRotation
        {
            get
            {
                var ship = GetMyShip();
                if (ship == null) return 0;

                if (ship.Cells.Count == 1) return ship.IsHorizontal ? 0 : 90;

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