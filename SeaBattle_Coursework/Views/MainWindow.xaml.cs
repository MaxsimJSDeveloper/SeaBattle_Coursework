using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SeaBattle_Coursework
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MediaPlayer bgMusic = new MediaPlayer();

        public MainWindow()
        {
            InitializeComponent();

            bgMusic.Open(new Uri("Assets/Sounds/start_menu_music.mp3", UriKind.Relative));
            bgMusic.Play();
        }
    }
}