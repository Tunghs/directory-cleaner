using Cleaner.Util;

using System.Windows;

namespace Cleaner.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Fields
        private const string _APPLICATION_NAME_ = "Directory Cleaner";
        #endregion

        public App()
        {
            new Bootstrapper();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (ProcessChecker.IsOpen(_APPLICATION_NAME_))
            {
                MessageBox.Show("해당 프로그램이 이미 실행중 입니다.", "Warning");
            }
        }

        #region Methods

        #endregion
    }

}
