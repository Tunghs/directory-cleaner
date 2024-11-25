
namespace Cleaner.Util
{
    public class DirectoryCleaner
    {
        private double _desiredFreeRatio;
        private string _managedDirectoryPath;
        private SearchOption _searchOption = SearchOption.AllDirectories;
        private System.Timers.Timer _timer;

        public long TotalSpace { get; private set; }
        public long AvailableSpace { get; private set; }

        public delegate void TaskCompleteHandler();
        public TaskCompleteHandler TaskCompleteEvent;

        public void CheckDriveStatus()
        {
            DriveInfo drive = new DriveInfo(Path.GetPathRoot(_managedDirectoryPath));
            AvailableSpace = drive.AvailableFreeSpace;
            TotalSpace = drive.TotalSize;
        }

        public void SetManagedDirectoryPath(string directoryPath)
        {
            _managedDirectoryPath = directoryPath;
            CheckDriveStatus();
        }

        public void SetDesiredFreeRatio(double desiredFreeRatio)
        {
            _desiredFreeRatio = desiredFreeRatio;
        }

        public async void Run()
        {
            Logger.Instance.Print(Logger.LogLevel.INFO, $"작업 시작");
            await Task.Run(Cleanup);
        }

        public void Run(double time)
        {
            Logger.Instance.Print(Logger.LogLevel.INFO, $"타이머 작업 시작");
            _timer = new System.Timers.Timer(time);
            _timer.Elapsed += (sender, e) => Cleanup();
            _timer.Start();
        }

        public void Stop()
        {
            Logger.Instance.Print(Logger.LogLevel.INFO, $"타이머 작업 종료");
            _timer.Elapsed -= (sender, e) => Cleanup();
            _timer.Stop();
        }

        private void Cleanup()
        {
            try
            {
                long bytesToFree = ComputeRequiredFreeSpace();
                long freedBytes = 0;

                if (bytesToFree <= 0)
                {
                    return;
                }

                string[] files = Directory.GetFiles(_managedDirectoryPath, "*.*", _searchOption);
                var filesInfo = files.Select(file => new FileInfo(file))
                                     .OrderBy(f => f.LastWriteTime)
                                     .ToList();

                int deletedFileCount = 0;
                foreach (var fileInfo in filesInfo)
                {
                    freedBytes += fileInfo.Length;
                    fileInfo.Delete();
                    deletedFileCount++;
                    if (freedBytes >= bytesToFree)
                    {
                        break; // 필요한 용량 확보 시 삭제 중지
                    }
                }

                Logger.Instance.Print(Logger.LogLevel.INFO, $"작업 완료. {deletedFileCount} 개 파일 삭제");
                TaskCompleteEvent?.Invoke();
            }
            catch(Exception ex)
            {
                Logger.Instance.Print(Logger.LogLevel.ERROR, "An error occurred during cleanup: " + ex.Message);
            }
        }

        private long ComputeRequiredFreeSpace()
        {
            DriveInfo drive = new DriveInfo(Path.GetPathRoot(_managedDirectoryPath));
            return (long)((1 - _desiredFreeRatio) * TotalSpace) - drive.AvailableFreeSpace;
        }
    }
}
