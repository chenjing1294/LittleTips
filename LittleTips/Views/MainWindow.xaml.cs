using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using Assistant.Utils;
using LittleTips.Dto;
using LittleTips.Utils;
using LittleTips.ViewModels;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using WinForms = System.Windows.Forms;
using Shortcut = LittleTips.ViewModels.Shortcut;
using TextBox = System.Windows.Controls.TextBox;

namespace LittleTips.Views
{
    public partial class MainWindow
    {
        private readonly MainModel _mainModel;
        private readonly KeyboardHook _hook = new KeyboardHook();
        private bool _isShowing = false;
        private NotAdaptedWindow _notAdaptedWindow;
        private System.Windows.Window _tipsWindow;
        private System.Windows.Window _donateWindow;
        private readonly Dictionary<string, Dto.App> _dictionary = new Dictionary<string, Dto.App>();
        public static Dictionary<string, List<string>> FavoriteKeys;
        public static string ShowingAppKey;
        private static string AppShortcut = "AppShortcut";
        private static string AppIcon = "AppIcon";
        private readonly Random _random = new Random();
        private readonly int _probability = 14;

        public MainWindow()
        {
            InitializeComponent();
            this._mainModel = (MainModel) this.DataContext;
            // WindowBlur.SetIsEnabled(this, true);
            try
            {
                RegisterHotKey();
            }
            catch (Exception e)
            {
                XMessageBox.Show(e.InnerException != null ? e.InnerException.Message : e.Message, type: "warning");
            }

            InitWinForm();
            InitData();
            InitBackground();
        }

        private void InitData()
        {
#if DEBUG
            LittleTips.Properties.Settings.Default.DonateKey = null;
#endif
            if (!string.IsNullOrEmpty(LittleTips.Properties.Settings.Default.DonateKey))
            {
                TokenTextBox.Text = LittleTips.Properties.Settings.Default.DonateKey;
            }

            #region 初始化分辨率

            _mainModel.ScreenWidth = (int) (Screen.PrimaryScreen.Bounds.Width);
            _mainModel.ScreenHeight = (int) (Screen.PrimaryScreen.Bounds.Height);
            if (LittleTips.Properties.Settings.Default.TipsWindowWidth <= 0)
            {
                LittleTips.Properties.Settings.Default.TipsWindowWidth = (int) (Screen.PrimaryScreen.Bounds.Width * 0.4);
            }

            if (LittleTips.Properties.Settings.Default.TipsWindowHeight <= 0)
            {
                LittleTips.Properties.Settings.Default.TipsWindowHeight = (int) (Screen.PrimaryScreen.Bounds.Height * 0.86);
            }

            #endregion

            if (App.VersionNumber.Equals("1.0.1.0") && !LittleTips.Properties.Settings.Default.DataMigration)
            {
                LittleTips.Properties.Settings.Default.Favorites = null;
                LittleTips.Properties.Settings.Default.DataMigration = true;
                LittleTips.Properties.Settings.Default.Save();
            }

            string favorites = LittleTips.Properties.Settings.Default.Favorites;
            if (!string.IsNullOrEmpty(favorites))
            {
                FavoriteKeys = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(favorites, Common.JsonSerializerOptions);
            }

            if (FavoriteKeys == null)
            {
                FavoriteKeys = new Dictionary<string, List<string>>();
            }

            DirectoryInfo root = new DirectoryInfo(AppShortcut);
            FileInfo[] files = root.GetFiles();
            foreach (FileInfo file in files)
            {
                string json = File.ReadAllText($"{AppShortcut}/{file.Name}");
                Dto.App app = JsonSerializer.Deserialize<Dto.App>(json, Common.JsonSerializerOptions);
                if (app != null)
                {
                    if (!app.AppIcon.Contains(":"))
                    {
                        app.AppIcon = $"pack://application:,,,/{AppIcon}/{app.AppIcon}";
                    }

                    _mainModel.SupportApps.Add(app);
                    string key = app.Id;
                    _dictionary[key] = app;
                    if (!FavoriteKeys.ContainsKey(key))
                    {
                        FavoriteKeys[key] = new List<string>();
                    }
                }
            }
        }

        private void SaveData()
        {
            LittleTips.Properties.Settings.Default.Favorites = JsonSerializer.Serialize(FavoriteKeys, Common.UglyJsonSerializerOptions);
            LittleTips.Properties.Settings.Default.Save();
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_notAdaptedWindow != null)
                _notAdaptedWindow.Close();
            if (_tipsWindow != null)
                _tipsWindow.Close();
            if (_donateWindow != null)
                _donateWindow.Close();
            base.OnClosed(e);
            LittleTips.Properties.Settings.Default.Save();
            SaveData();
        }


        private void RegisterHotKey()
        {
            // register the event that is fired after the key press.
            _hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(HookKeyPressed);
            Keys hotKey = Keys.Oemtilde;
            if (!string.IsNullOrEmpty(LittleTips.Properties.Settings.Default.HotKey))
            {
                Keys toWinformsKey = Common.StringToWinformsKey(LittleTips.Properties.Settings.Default.HotKey);
                if (toWinformsKey != Keys.None)
                {
                    hotKey = toWinformsKey;
                }
            }

            LittleTips.Properties.Settings.Default.HotKey = hotKey.ToString();
            HotKeyTextBox.Text = hotKey.ToString();

            switch (LittleTips.Properties.Settings.Default.ModifierKeyIndex)
            {
                case 0:
                    _hook.RegisterHotKey(ModifierKeys.Control, hotKey);
                    break;
                case 1:
                    _hook.RegisterHotKey(ModifierKeys.Alt, hotKey);
                    break;
                case 2:
                    _hook.RegisterHotKey(ModifierKeys.Shift, hotKey);
                    break;
                case 3:
                    _hook.RegisterHotKey(ModifierKeys.Win, hotKey);
                    break;
                default:
                    _hook.RegisterHotKey(ModifierKeys.Control, hotKey);
                    break;
            }

            // register the control + alt + F12 combination as hot key.
            // hook.RegisterHotKey(ModifierKeys.Control | ModifierKeys.Alt, Keys.F12);
        }

        private void HookKeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (!_isShowing)
            {
                string appPath = Utils.ActiveWindow.GetActiveProgramName().ToLower();
                _isShowing = true;
                if (string.IsNullOrEmpty(LittleTips.Properties.Settings.Default.DonateKey))
                {
                    bool donate = _random.Next(_probability) % (_probability - 1) == 0;
                    if (donate)
                    {
                        if (_donateWindow == null)
                        {
                            _donateWindow = new DonateWindow(this)
                            {
                                ShowInTaskbar = false, ShowActivated = true,
                                Width = LittleTips.Properties.Settings.Default.TipsWindowWidth,
                                Height = LittleTips.Properties.Settings.Default.TipsWindowHeight,
                            };
                        }

                        _donateWindow.Show();
                        ShowingAppKey = null;
                        return;
                    }
                }

                bool found = false;
                foreach (string key in _dictionary.Keys)
                {
                    if (appPath.Contains(key.ToLower()))
                    {
                        found = true;
                        _mainModel.Shortcuts.Clear();
                        _mainModel.Favorites.Clear();
                        ShowingAppKey = key;
                        Dto.App app = _dictionary[key];
                        if (app != null)
                        {
                            _mainModel.AppName = app.AppName;
                            _mainModel.AppIcon = app.AppIcon;
                            foreach (Category category in app.Categories)
                            {
                                _mainModel.Shortcuts.Add(new Shortcut()
                                {
                                    EnglishCategoryName = category.EnglishCategoryName,
                                    ChineseCategoryName = category.ChineseCategoryName
                                });
                                foreach (Key item in category.Keys)
                                {
                                    _mainModel.Shortcuts.Add(new Shortcut()
                                    {
                                        Id = item.Id,
                                        ShortcutKey = item.ShortcutKey,
                                        EnglishFunctionDescription = item.EnglishFunctionDescription,
                                        ChineseFunctionDescription = item.ChineseFunctionDescription
                                    });
                                }
                            }

                            if (FavoriteKeys.ContainsKey(key))
                            {
                                List<string> favoriteKeyIds = FavoriteKeys[key];
                                foreach (string id in favoriteKeyIds)
                                {
                                    foreach (Shortcut shortcut in _mainModel.Shortcuts)
                                    {
                                        if (id.Equals(shortcut.Id))
                                        {
                                            shortcut.IsFavorite = true;
                                            _mainModel.Favorites.Add(shortcut);
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (_tipsWindow == null)
                        {
                            _tipsWindow = new TipsWindow(_mainModel, this)
                            {
                                ShowInTaskbar = false, ShowActivated = true,
                                Width = LittleTips.Properties.Settings.Default.TipsWindowWidth,
                                Height = LittleTips.Properties.Settings.Default.TipsWindowHeight,
                                WindowStartupLocation = WindowStartupLocation.CenterScreen
                            };
                        }

                        _tipsWindow.Show();
                        break;
                    }
                }

                if (!found)
                {
                    if (_notAdaptedWindow == null)
                    {
                        _notAdaptedWindow = new NotAdaptedWindow(this)
                        {
                            ShowInTaskbar = false, ShowActivated = true,
                            Width = LittleTips.Properties.Settings.Default.TipsWindowWidth,
                            Height = LittleTips.Properties.Settings.Default.TipsWindowHeight,
                            WindowStartupLocation = WindowStartupLocation.CenterScreen
                        };
                    }

                    _notAdaptedWindow.NotAdaptedModel.AppPath = appPath;
                    _notAdaptedWindow.Show();
                    ShowingAppKey = null;
                }
            }
            else
            {
                HiddenWidow();
            }
        }

        public void HiddenWidow()
        {
            _isShowing = false;
            if (_tipsWindow != null)
                _tipsWindow.Hide();
            if (_notAdaptedWindow != null)
                _notAdaptedWindow.Hide();
            if (_donateWindow != null)
                _donateWindow.Hide();
        }

        private void Donate_OnClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start($"{App.Host}/donate");
            e.Handled = true;
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            _mainModel.KeyGridVisibility = Visibility.Collapsed;
            _mainModel.SettingGridVisibility = Visibility.Collapsed;
            _mainModel.SupportAppsGridVisibility = Visibility.Collapsed;
            _mainModel.DebugGridVisibility = Visibility.Collapsed;
            _mainModel.MainGridVisibility = Visibility.Visible;
        }

        private void DonateButton_OnClick(object sender, RoutedEventArgs e)
        {
            _mainModel.MainGridVisibility = Visibility.Collapsed;
            _mainModel.SettingGridVisibility = Visibility.Collapsed;
            _mainModel.SupportAppsGridVisibility = Visibility.Collapsed;
            _mainModel.DebugGridVisibility = Visibility.Collapsed;
            _mainModel.KeyGridVisibility = Visibility.Visible;
        }

        private void SettingButton_OnClick(object sender, RoutedEventArgs e)
        {
            _mainModel.MainGridVisibility = Visibility.Collapsed;
            _mainModel.KeyGridVisibility = Visibility.Collapsed;
            _mainModel.SupportAppsGridVisibility = Visibility.Collapsed;
            _mainModel.DebugGridVisibility = Visibility.Collapsed;
            _mainModel.SettingGridVisibility = Visibility.Visible;
        }

        private void SupportButton_OnClick(object sender, RoutedEventArgs e)
        {
            _mainModel.MainGridVisibility = Visibility.Collapsed;
            _mainModel.KeyGridVisibility = Visibility.Collapsed;
            _mainModel.SettingGridVisibility = Visibility.Collapsed;
            _mainModel.DebugGridVisibility = Visibility.Collapsed;
            _mainModel.SupportAppsGridVisibility = Visibility.Visible;
        }

        private void DebugButton_OnClick(object sender, RoutedEventArgs e)
        {
            _mainModel.MainGridVisibility = Visibility.Collapsed;
            _mainModel.KeyGridVisibility = Visibility.Collapsed;
            _mainModel.SettingGridVisibility = Visibility.Collapsed;
            _mainModel.SupportAppsGridVisibility = Visibility.Collapsed;
            _mainModel.DebugGridVisibility = Visibility.Visible;
        }

        #region 任务栏

        private WinForms.NotifyIcon _notifyIcon;
        private WinForms.ContextMenu _contextMenu;
        private WinForms.MenuItem _closeApp;
        private WinForms.MenuItem _openWindow;
        private System.ComponentModel.IContainer _iContainer;

        private void InitWinForm()
        {
            _contextMenu = new WinForms.ContextMenu();
            _openWindow = new WinForms.MenuItem() {Text = "Open"};
            _closeApp = new WinForms.MenuItem() {Text = "Exit"};
            _iContainer = new System.ComponentModel.Container();
            WinForms.MenuItem[] menuItems = new WinForms.MenuItem[] {_openWindow, _closeApp};
            _contextMenu.MenuItems.AddRange(menuItems);
            _openWindow.Click += OpenApp_Click;
            _closeApp.Click += CloseApp_Click;

            _notifyIcon = new WinForms.NotifyIcon(_iContainer);
            _notifyIcon.Icon = new System.Drawing.Icon("Images/icons8-memory-100.ico");
            _notifyIcon.Text = "LittleTips";
            _notifyIcon.Visible = true;
            _notifyIcon.MouseDoubleClick += Icon_OnClick;
            _notifyIcon.ContextMenu = _contextMenu;
        }

        private void Icon_OnClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }

        private void OpenApp_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void CloseApp_Click(object sender, EventArgs e)
        {
            _notifyIcon.Dispose();
            Application.Current.Shutdown();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        #endregion

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Hide();
            e.Handled = true;
        }


        #region 激活与检查更新

        private BackgroundWorker _bgActivateBackgroundWorker;
        private BackgroundWorker _bgCheckUpdateBackgroundWorker;

        private void InitBackground()
        {
            _bgActivateBackgroundWorker = new BackgroundWorker()
            {
                WorkerSupportsCancellation = true
            };
            _bgActivateBackgroundWorker.DoWork += bgActivateBackgroundWorker_DoWork;
            _bgActivateBackgroundWorker.RunWorkerCompleted += bgActivateBackgroundWorker_RunWorkCompleted;

            _bgCheckUpdateBackgroundWorker = new BackgroundWorker()
            {
                WorkerSupportsCancellation = true
            };
            _bgCheckUpdateBackgroundWorker.DoWork += bgCheckUpdateBackgroundWorker_DoWork;
            _bgCheckUpdateBackgroundWorker.RunWorkerCompleted += bgCheckUpdateBackgroundWorker_RunWorkCompleted;
        }

        #region 激活

        private void DonateSaveButtonClick(object sender, RoutedEventArgs e)
        {
            string donate = TokenTextBox.Text;
            if (!string.IsNullOrEmpty(donate))
            {
                //验证Key
                if (_bgActivateBackgroundWorker.IsBusy)
                {
                    _bgActivateBackgroundWorker.CancelAsync();
                }

                if (!_bgActivateBackgroundWorker.IsBusy)
                {
                    ActivateProgressBar.Visibility = Visibility.Visible;
                    _bgActivateBackgroundWorker.RunWorkerAsync(new object[] {true, donate});
                }
            }

            e.Handled = true;
        }

        private void bgActivateBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            object[] args = (object[]) e.Argument;
            bool toActivate = (bool) args[0];
            string token = (string) args[1];
            Thread.Sleep(2 * 1000);
            //解析TOKEN并设置激活信息
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    string bios = Id.GetIdentifier();
                    if (!string.IsNullOrEmpty(bios))
                    {
                        string result;
                        if (toActivate) //激活软件
                        {
                            result = Common.Get($"{App.Host}/activate/activateByUUID?uuid={token}&id={bios}");
                        }
                        else
                        {
                            result = Common.Get($"{App.Host}/activate/unActivateByUUID?uuid={token}&id={bios}");
                        }

                        ActivateResult activate = JsonSerializer.Deserialize<ActivateResult>(result);
                        e.Result = activate;
                    }
                }
            }
            catch
            {
                //
            }
        }

        private void bgActivateBackgroundWorker_RunWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is ActivateResult activateResult)
            {
                int code = activateResult.code;
                switch (code)
                {
                    case 0:
                        if (activateResult.activate)
                        {
                            string token = activateResult.token;
                            LittleTips.Properties.Settings.Default.DonateKey = token;
                        }
                        else
                        {
                            LittleTips.Properties.Settings.Default.DonateKey = null;
                        }

                        LittleTips.Properties.Settings.Default.Save();
                        XMessageBox.Show("操作成功", this);
                        break;
                    case 1:
                        XMessageBox.Show("操作失败，该捐赠码已在另两台设备上使用过", this, type: "warning");
                        break;
                    default:
                        XMessageBox.Show("操作失败，未知的捐赠码", this, type: "warning");
                        break;
                }
            }
            else
            {
                XMessageBox.Show("操作失败", this, type: "warning");
            }

            ActivateProgressBar.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region 检查更新

        private void UpdateButton_OnClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (_bgCheckUpdateBackgroundWorker.IsBusy)
            {
                _bgCheckUpdateBackgroundWorker.CancelAsync();
            }

            if (!_bgCheckUpdateBackgroundWorker.IsBusy)
            {
                _bgCheckUpdateBackgroundWorker.RunWorkerAsync();
            }

            UpdateProgressBar.Visibility = Visibility.Visible;
        }

        private void OpenButton_OnClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start($"{App.Host}");
            e.Handled = true;
        }

        private void bgCheckUpdateBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(2 * 1000);
            CheckUpdateDto newVersion = new CheckUpdateDto(App.VersionNumber);
            try
            {
                string res = Common.Get($"{App.Host}/activate/checkUpdate?version={App.VersionNumber}");
                newVersion = JsonSerializer.Deserialize<CheckUpdateDto>(res, Common.JsonSerializerOptions);
            }
            catch
            {
                // ignored
            }

            e.Result = newVersion;
        }

        private void bgCheckUpdateBackgroundWorker_RunWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CheckUpdateDto newVersion = (CheckUpdateDto) e.Result;
            CheckUpdateDto oldVersion = new CheckUpdateDto(App.VersionNumber);
            long newV = 0;
            long oldV = 0;
            bool needUpdate = false;
            if (newVersion != null)
            {
                newV = newVersion.MajorVersionNumber * 100000 + newVersion.MinorVersionNumber * 10000 +
                       newVersion.RevisionNumber * 100 + newVersion.BuildNumber;
                oldV = oldVersion.MajorVersionNumber * 100000 + oldVersion.MinorVersionNumber * 10000 +
                       oldVersion.RevisionNumber * 100 + oldVersion.BuildNumber;
                if (oldV < newV)
                {
                    needUpdate = true;
                }
            }

            UpdateProgressBar.Visibility = Visibility.Collapsed;
            UpdateTextBlock.Text = needUpdate
                ? $"It can be updated to {newVersion.Version}, please go to the official website to download"
                : "No update required";
        }

        #endregion

        #endregion

        private void HotKeyTextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                switch (ModifierKeyComboBox.SelectedIndex)
                {
                    case 0: //Ctrl
                        LittleTips.Properties.Settings.Default.ModifierKeyIndex = 0;
                        break;
                    case 1: //Alt
                        LittleTips.Properties.Settings.Default.ModifierKeyIndex = 1;
                        break;
                    case 2: //Shift
                        LittleTips.Properties.Settings.Default.ModifierKeyIndex = 2;
                        break;
                    case 3: //Win
                        LittleTips.Properties.Settings.Default.ModifierKeyIndex = 3;
                        break;
                    default:
                        LittleTips.Properties.Settings.Default.ModifierKeyIndex = 0;
                        break;
                }

                Keys winformsKey = Common.WPFToWinformsKey(e.Key);
                if (winformsKey == Keys.None)
                {
                    textBox.Text = "不支持此键，请更换";
                }
                else
                {
                    textBox.Text = winformsKey.ToString();
                    LittleTips.Properties.Settings.Default.HotKey = winformsKey.ToString();
                }

                e.Handled = true;
            }
        }
    }
}