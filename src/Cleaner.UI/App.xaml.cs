using Cleaner.Util;

using System.IO;
using System.Windows;

using Wpf.Ui.Controls;

using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace Cleaner.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Fields
        private const string _APPLICATION_NAME_ = "Directory Cleaner";
        private NotifyIcon _notify;
        private ShellWindow _shellWindow;
        private bool _isDuplicated;
        #endregion

        public App()
        {
            new Bootstrapper();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (ProcessChecker.IsOpen(_APPLICATION_NAME_))
            {
                MessageBox.Show("The program is already running.", "Warning");
                _isDuplicated = true;
                Current.Shutdown();
            }

            _shellWindow = new ShellWindow();
            _shellWindow.Closing += ShellWindow_Closing;
            InitNotifyIcon();

            _shellWindow.ShowDialog();
            Logger.Instance.Print(Logger.LogLevel.INFO, $"프로그램 시작.");
        }

        private void ShellWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_isDuplicated)
            {
                return;
            }

            _notify.Visible = true;
            _notify.ShowBalloonTip(200);
            _shellWindow.Hide();
            e.Cancel = true;
        }

        #region Methods
        private void InitNotifyIcon()
        {
            try
            {
                var sri = System.Windows.Application.GetResourceStream(new Uri(@"directory-cleaner.ico", UriKind.Relative));
                var bitmap = new Bitmap(sri.Stream);
                var handle = bitmap.GetHicon();

                ContextMenuStrip menu = new ContextMenuStrip();
                menu.Items.Add(new ToolStripMenuItem("Open Directory Cleaner", null, new EventHandler(Open)));
                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add(new ToolStripMenuItem("Exit", null, new EventHandler(Exit)));

                _notify = new NotifyIcon();
                _notify.ContextMenuStrip = menu;
                _notify.Icon = Icon.FromHandle(handle);
                _notify.BalloonTipTitle = _APPLICATION_NAME_;
                _notify.BalloonTipText = "Program is running in behind";
                _notify.DoubleClick += Open;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Notify Icon Error," + ex.Message);
                Logger.Instance.Print(Logger.LogLevel.ERROR, "Notify Icon Error. " + ex.Message);
            }
        }

        private void Open(object sender, System.EventArgs e)
        {
            _notify.Visible = false;
            _shellWindow.ShowDialog();
        }

        private void Exit(object sender, System.EventArgs e)
        {
            if (MessageBox.Show($"Are you sure you want to exit {_APPLICATION_NAME_}?", "Exit", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
            {
                return;
            }
            _notify.Dispose();
            Current.Shutdown();

            Logger.Instance.Print(Logger.LogLevel.INFO, $"프로그램 종료.");
        }
        #endregion
    }

}
