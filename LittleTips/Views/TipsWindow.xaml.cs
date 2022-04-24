using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using LittleTips.Utils;
using LittleTips.ViewModels;

namespace LittleTips.Views
{
    public partial class TipsWindow : Window
    {
        private readonly MainModel _mainModel;
        private readonly MainWindow _mainWindow;

        public TipsWindow(MainModel mainModel, MainWindow mainWindow)
        {
            InitializeComponent();
            this._mainModel = mainModel;
            this._mainWindow = mainWindow;
            this.DataContext = mainModel;
            WindowBlur.SetIsEnabled(this, true);
            SwitchMode(LittleTips.Properties.Settings.Default.LightMode == true);
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

        public static void SwitchMode(bool lightMode)
        {
            ResourceDictionary rd = new ResourceDictionary
            {
                Source = lightMode
                    ? new Uri("Styles/LightColors.xaml", UriKind.Relative)
                    : new Uri("Styles/DarkColors.xaml", UriKind.Relative)
            };
            Application.Current.Resources.MergedDictionaries[0] = rd;
            LittleTips.Properties.Settings.Default.LightMode = lightMode;
        }

        private void ModeButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton button)
            {
                SwitchMode(button.IsChecked == true);
            }
        }

        private void LanguageButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton button)
            {
                object bak = DataContext;
                DataContext = null;
                DataContext = bak;
            }
        }

        private void StarButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton button)
            {
                string id = (string) button.Tag;
                if (button.IsChecked == true)
                {
                    foreach (var shortcut in _mainModel.Shortcuts)
                    {
                        if (!string.IsNullOrEmpty(shortcut.Id) && shortcut.Id.Equals(id))
                        {
                            _mainModel.Favorites.Add(shortcut);
                            break;
                        }
                    }
                }
                else
                {
                    foreach (var shortcut in _mainModel.Favorites)
                    {
                        if (!string.IsNullOrEmpty(shortcut.Id) && shortcut.Id.Equals(id))
                        {
                            _mainModel.Favorites.Remove(shortcut);
                            break;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(MainWindow.ShowingAppKey))
                {
                    if (MainWindow.FavoriteKeys.ContainsKey(MainWindow.ShowingAppKey))
                    {
                        List<string> favoriteKeys = MainWindow.FavoriteKeys[MainWindow.ShowingAppKey];
                        if (button.IsChecked == true)
                        {
                            if (!favoriteKeys.Contains(id))
                            {
                                favoriteKeys.Add(id);
                            }
                        }
                        else
                        {
                            if (favoriteKeys.Contains(id))
                            {
                                favoriteKeys.Remove(id);
                            }
                        }
                    }
                }
            }
        }
    }
}