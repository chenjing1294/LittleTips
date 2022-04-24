using System;
using System.Windows;

namespace LittleTips.Views
{
    public partial class XMessageBox : Window
    {
        private string _msg;
        private string _link;
        private string _msgType; //error、warning、notification

        public static bool Show(string msg, Window owner = null, bool inUiThread = false, string link = null,
            string type = "notification")
        {
            if (inUiThread)
            {
                XMessageBox box = new XMessageBox(msg, link, type)
                {
                    WindowStartupLocation =
                        owner != null ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen,
                    Owner = owner,
                    ShowActivated = true,
                    ShowInTaskbar = true,
                    Title = type
                };
                return (bool) box.ShowDialog();
            }
            else
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    XMessageBox box = new XMessageBox(msg, link, type)
                    {
                        WindowStartupLocation = owner != null
                            ? WindowStartupLocation.CenterOwner
                            : WindowStartupLocation.CenterScreen,
                        Owner = owner,
                        ShowActivated = true,
                        ShowInTaskbar = true,
                        Title = type
                    };
                    box.ShowDialog();
                }));
                return false;
            }
        }

        public XMessageBox(string msg, string link = null, string type = "notification")
        {
            InitializeComponent();
            OkButton.Click += OkButton_OnClick;
            CancelButton.Click += CancelButton_OnClick;
            _msg = msg;
            _link = link;
            _msgType = type;
            Msg.Text = _msg;
            if (_msgType.Equals("notification"))
            {
                NotificationIcon.Visibility = Visibility.Visible;
                WarningIcon.Visibility = Visibility.Collapsed;
                ErrorIcon.Visibility = Visibility.Collapsed;
            }
            else if (_msgType.Equals("warning"))
            {
                NotificationIcon.Visibility = Visibility.Collapsed;
                WarningIcon.Visibility = Visibility.Visible;
                ErrorIcon.Visibility = Visibility.Collapsed;
            }
            else if (_msgType.Equals("error"))
            {
                NotificationIcon.Visibility = Visibility.Collapsed;
                WarningIcon.Visibility = Visibility.Collapsed;
                ErrorIcon.Visibility = Visibility.Visible;
            }

            Link.Visibility = !string.IsNullOrEmpty(_link) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_link))
            {
                System.Diagnostics.Process.Start(_link);
            }
        }
    }
}