using System.Windows;
using SeaBattle_Coursework.Services;

namespace SeaBattle_Coursework
{
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            SoundManager.DisposeAll();
            base.OnExit(e);
        }
    }
}