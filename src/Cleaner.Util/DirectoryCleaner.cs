
namespace Cleaner.Util
{
    public class DirectoryCleaner
    {
        private double _desiredFreeRatio;
        private string _managedDirectoryPath;
        private SearchOption _searchOption = SearchOption.TopDirectoryOnly;
        private System.Timers.Timer _timer;

        public long TotalSpace { get; private set; }
        public long AvailableSpace { get; private set; }

        private void CheckDriveStatus(string path)
        {
            DriveInfo drive = new DriveInfo(path);
            AvailableSpace = drive.AvailableFreeSpace;
            TotalSpace = drive.TotalSize;
        }

        public void SetManagedDirectoryPath(string directoryPath)
        {
            _managedDirectoryPath = directoryPath;
            CheckDriveStatus(Path.GetPathRoot(directoryPath));
        }

        public void SetDesiredFreeRatio(double desiredFreeRatio)
        {
            _desiredFreeRatio = desiredFreeRatio;
        }

        public void Run()
        {
            Cleanup();
        }

        public void Run(double time)
        {
            _timer = new System.Timers.Timer(time);
            _timer.Elapsed += (sender, e) => Cleanup();
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Elapsed -= (sender, e) => Cleanup();
            _timer.Stop();
        }

        private void Cleanup()
        {
            try
            {
                string[] files = Directory.GetFiles(_managedDirectoryPath, "*.*", _searchOption);
                var filesInfo = files.Select(file => new FileInfo(file))
                                     .OrderBy(f => f.LastWriteTime)
                                     .ToList();

                long bytesToFree = ComputeRequiredFreeSpace();
                long freedBytes = 0;

                foreach (var fileInfo in filesInfo)
                {
                    freedBytes += fileInfo.Length;
                    Console.WriteLine($"Deleting: {fileInfo.FullName}, Size: {fileInfo.Length / 1024} KB");
                    fileInfo.Delete();

                    if (freedBytes >= bytesToFree)
                    {
                        break; // 필요한 용량 확보 시 삭제 중지
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("An error occurred during cleanup: " + ex.Message);
            }
        }

        private long ComputeRequiredFreeSpace()
        {
            DriveInfo drive = new DriveInfo("C");
            return (long)((1 - _desiredFreeRatio) * TotalSpace) - drive.AvailableFreeSpace;
        }
    }
}
