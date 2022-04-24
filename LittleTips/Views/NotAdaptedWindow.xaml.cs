using System.Windows;
using System.Windows.Input;
using LittleTips.Utils;
using LittleTips.ViewModels;

namespace LittleTips.Views
{
    public partial class NotAdaptedWindow : Window
    {
        public readonly NotAdaptedModel NotAdaptedModel;
        private readonly MainWindow _mainWindow;

        public NotAdaptedWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this._mainWindow = mainWindow;
            this.NotAdaptedModel = (NotAdaptedModel) this.DataContext;
            WindowBlur.SetIsEnabled(this, true);
            TipsWindow.SwitchMode(LittleTips.Properties.Settings.Default.LightMode == true);
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