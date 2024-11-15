using Cleaner.Common.Bases;
using Cleaner.Util;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Win32;

namespace Cleaner.UI.Viewers
{
    public partial class SettingViewModel : ViewModelBase
    {
        #region Fields
        private OpenFolderDialog _openFolderDialog = new OpenFolderDialog() { Title = "Select Directory" };
        private DirectoryCleaner _directoryCleaner = new DirectoryCleaner();
        #endregion

        #region UI Variable
        [ObservableProperty]
        private string _directoryPath;

        [ObservableProperty]
        private string _directorySize;

        [ObservableProperty]
        private string _availableSize;
        #endregion

        #region Command
        [RelayCommand]
        public void OpenFileDialog()
        {
            if (_openFolderDialog.ShowDialog() == true)
            {
                DirectoryPath = _openFolderDialog.FolderName;
                InitDirectory();
            }
        }

        [RelayCommand]
        public void OpCleanerStart()
        {
            _directoryCleaner.Run();
        }
        #endregion

        #region Private Methods
        private void InitDirectory()
        {
            _directoryCleaner.SetManagedDirectoryPath(DirectoryPath);
            DirectorySize = FormatBytes(_directoryCleaner.TotalSpace);
            AvailableSize = FormatBytes(_directoryCleaner.AvailableSpace);
        }

        private string FormatBytes(long bytes)
        {
            const int scale = 1024;
            string[] orders = new string[] { "GB", "MB", "KB", "Bytes" };
            long max = (long)Math.Pow(scale, orders.Length - 1);

            foreach (string order in orders)
            {
                if (bytes > max)
                    return string.Format("{0:##.##} {1}", decimal.Divide(bytes, max), order);

                max /= scale;
            }
            return "0 Bytes";
        }
        #endregion
    }
}
