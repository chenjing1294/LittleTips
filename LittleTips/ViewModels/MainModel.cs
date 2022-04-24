using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace LittleTips.ViewModels
{
    public class MainModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Visibility _keyGridVisibility = Visibility.Collapsed;

        public Visibility KeyGridVisibility
        {
            get => _keyGridVisibility;
            set
            {
                _keyGridVisibility = value;
                OnPropertyChanged(nameof(KeyGridVisibility));
            }
        }


        private ObservableCollection<Dto.App> _supportApps = new ObservableCollection<Dto.App>();

        public ObservableCollection<Dto.App> SupportApps
        {
            get => _supportApps;
            set
            {
                _supportApps = value;
                OnPropertyChanged(nameof(SupportApps));
            }
        }

        private Visibility _supportAppsGridVisibility = Visibility.Collapsed;

        public Visibility SupportAppsGridVisibility
        {
            get => _supportAppsGridVisibility;
            set
            {
                _supportAppsGridVisibility = value;
                OnPropertyChanged(nameof(SupportAppsGridVisibility));
            }
        }

        private Visibility _settingGridVisibility = Visibility.Collapsed;

        public Visibility SettingGridVisibility
        {
            get => _settingGridVisibility;
            set
            {
                _settingGridVisibility = value;
                OnPropertyChanged(nameof(SettingGridVisibility));
            }
        }

        private Visibility _debugGridVisibility = Visibility.Collapsed;

        public Visibility DebugGridVisibility
        {
            get => _debugGridVisibility;
            set
            {
                _debugGridVisibility = value;
                OnPropertyChanged(nameof(DebugGridVisibility));
            }
        }

        private Visibility _mainGridVisibility = Visibility.Visible;

        public Visibility MainGridVisibility
        {
            get => _mainGridVisibility;
            set
            {
                _mainGridVisibility = value;
                OnPropertyChanged(nameof(MainGridVisibility));
            }
        }

        private ObservableCollection<Shortcut> _shortcuts = new ObservableCollection<Shortcut>();

        public ObservableCollection<Shortcut> Shortcuts
        {
            get => _shortcuts;
            set
            {
                _shortcuts = value;
                OnPropertyChanged(nameof(Shortcuts));
            }
        }

        private ObservableCollection<Shortcut> _favorites = new ObservableCollection<Shortcut>();

        public ObservableCollection<Shortcut> Favorites
        {
            get => _favorites;
            set
            {
                _favorites = value;
                OnPropertyChanged(nameof(Favorites));
            }
        }

        public string FavoritesString
        {
            get
            {
                if (LittleTips.Properties.Settings.Default.ChineseMode)
                {
                    return "收藏夹";
                }
                else
                {
                    return "Favorites";
                }
            }
        }


        private string _appIcon;

        public string AppIcon
        {
            get => _appIcon;
            set
            {
                _appIcon = value;
                OnPropertyChanged(nameof(AppIcon));
            }
        }

        private string _appName;

        public string AppName
        {
            get => _appName;
            set
            {
                _appName = value;
                OnPropertyChanged(nameof(AppName));
            }
        }


        private int _screenWidth;

        public int ScreenWidth
        {
            get => _screenWidth;
            set
            {
                _screenWidth = value;
                OnPropertyChanged(nameof(ScreenWidth));
            }
        }

        private int _screenHeight;

        public int ScreenHeight
        {
            get => _screenHeight;
            set
            {
                _screenHeight = value;
                OnPropertyChanged(nameof(ScreenHeight));
            }
        }
    }
}