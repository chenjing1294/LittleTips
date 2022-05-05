using System.Windows;
using System.Windows.Input;
using LittleTips.Utils;

namespace LittleTips.Views
{
    public partial class DonateWindow : Window
    {
        private readonly MainWindow _mainWindow;

        public DonateWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this._mainWindow = mainWindow;
            WindowBlur.SetIsEnabled(this, true);
            TipsWindow.SwitchMode(LittleTips.Properties.Settings.Default.LightMode == true);
        }

        private void Donate_OnClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start($"{App.Host}/donate");
            e.Handled = true;
        }

        private void NotAdaptedWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (_mainWindow != null)
                {
                    _mainWindow.HiddenWidow();
                    e.Handled = true;
                }
            }
        }
    }
}