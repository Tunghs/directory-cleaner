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
        private OpenFolderDialog _openFolderDialog;
        private DirectoryCleaner _directoryCleaner;
        private double _interval = 0;
        #endregion

        #region UI Variable
        [ObservableProperty]
        private string _directoryPath;

        [ObservableProperty]
        private string _directorySizeStr;

        [ObservableProperty]
        private string _availableSizeStr;

        [ObservableProperty]
        private long _directorySize;

        [ObservableProperty]
        private long _unavailableSize;

        [ObservableProperty]
        private double _thresholdRatio;

        [ObservableProperty]
        private string _intervalMin;

        [ObservableProperty]
        private string _setIntervalMin;

        [ObservableProperty]
        private double _setThresholdRatio;

        [ObservableProperty]
        private bool _isEnabledStartButton = true;

        [ObservableProperty]
        private bool _isEnabledStopButton;
        #endregion

        public SettingViewModel()
        {
            _openFolderDialog = new OpenFolderDialog() { Title = "Select Directory" };
            _directoryCleaner = new DirectoryCleaner();
            _directoryCleaner.TaskCompleteEvent += OnTaskComplete;
        }

        #region Command
        [RelayCommand]
        public void OnButtonClick(string @param)
        {
            switch (@param)
            {
                case "UpdateThresholdRatio":
                    UpdateThresholdRatio();
                    break;
                case "OpenFileDialog":
                    OpenFileDialog();
                    break;
                case "Start":
                    StartCleaner();
                    break;                     
                case "Stop":
                    StopCleaner();
                    break;                
                case "UpdateDirectoryStatus":
                    UpdateDirectoryStatus();
                    break;                
                case "ApplyTimer":
                    ApplyTimer();
                    break;
                case "ResetTimer":
                    IntervalMin = "0";
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Private Methods
        private void InitDirectory()
        {
            _directoryCleaner.SetManagedDirectoryPath(DirectoryPath);
            UpdateDriveStatusUI();
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

        private void UpdateThresholdRatio()
        {
            double ratio = ThresholdRatio / 100;
            SetThresholdRatio = ThresholdRatio;
            _directoryCleaner.SetDesiredFreeRatio(ratio);

            Logger.Instance.Print(Logger.LogLevel.INFO, $"Threshold 값 변경. {SetThresholdRatio}%");
        }

        private void OpenFileDialog()
        {
            if (_openFolderDialog.ShowDialog() == true)
            {
                DirectoryPath = _openFolderDialog.FolderName;
                InitDirectory();
            }
        }

        private void UpdateDirectoryStatus()
        {
            if (string.IsNullOrEmpty(DirectoryPath))
            {
                MessageBox.Show("Please select a directory.", "Select a drectory");
                return;
            }

            _directoryCleaner.CheckDriveStatus();
            UpdateDriveStatusUI();
        }

        private void StartCleaner()
        {
            if (_interval > 0)
            {
                _directoryCleaner.Run(_interval);
                IsEnabledStartButton = false;
                IsEnabledStopButton = true;
            }
            else
            {
                _directoryCleaner.Run();
            }
        }

        private void StopCleaner()
        {
            var messageBoxResult = MessageBox.Show("Are you sure you want to stop the cleaner?", "Stop", MessageBoxButtons.YesNo);
            if (messageBoxResult == DialogResult.Yes)
            {
                _directoryCleaner.Stop();
                IsEnabledStartButton = true;
                IsEnabledStopButton = false;
            }
        }

        private void ApplyTimer()
        {
            IntervalMin = SetIntervalMin;
            _interval = int.Parse(SetIntervalMin) * 60000;
            Logger.Instance.Print(Logger.LogLevel.INFO, $"Interval 값 변경. {IntervalMin} Min");
        }

        private void OnTaskComplete()
        {
            _directoryCleaner.CheckDriveStatus();
            UpdateDriveStatusUI();
        }

        private void UpdateDriveStatusUI()
        {
            DirectorySize = _directoryCleaner.TotalSpace;
            UnavailableSize = _directoryCleaner.TotalSpace - _directoryCleaner.AvailableSpace;
            DirectorySizeStr = FormatBytes(_directoryCleaner.TotalSpace);
            AvailableSizeStr = FormatBytes(_directoryCleaner.AvailableSpace);

            Logger.Instance.Print(Logger.LogLevel.INFO, $"Drive 상태 확인. 현재 경로: \"{DirectoryPath}\" Total: {DirectorySizeStr}, Available: {AvailableSizeStr}");
        }
        #endregion
    }
}
