using System.Collections.Concurrent;
using System.Text;

namespace Cleaner.Util
{
    public class Logger
    {
        #region Data

        public enum LogLevel
        {
            TRACE,  // 추적 레벨은 Debug보다 좀 더 상세한 정보를 나타냄
            DEBUG,  // 프로그램을 디버깅하기 위한 정보 지정
            INFO,   // 상태변경과 같은 정보성 메시지를 나타냄
            WARN,   // 처리 가능한 문제, 향후 시스템 에러의 원인이 될 수 있는 경고성 메시지를 나타냄
            ERROR,  // 요청을 처리하는 중 문제가 발생한 경우
            FATAL   // 아주 심각한 에러가 발생한 상태, 시스템적으로 심각한 문제가 발생해서 어플리케이션 작동이 불가능할 경우
        }

        private struct LogData
        {
            public LogLevel Level;
            public DateTime DateTime;
            public string Message;

            public LogData(LogLevel logLevel, string msg)
            {
                Level = logLevel;
                DateTime = DateTime.Now;
                Message = msg;
            }
        }

        #endregion Data

        #region Field

        private object _LockObject = new object();
        private string _SaveDirectoryPath = @".\Log";
        private ConcurrentQueue<LogData> _LogProcessQueue = new ConcurrentQueue<LogData>();
        private string _FilePath = string.Empty;

        #endregion Field

        private static readonly Lazy<Logger> _Instance =
            new Lazy<Logger>(() => new Logger());

        public static Logger Instance
        {
            get { return _Instance.Value; }
        }

        private Logger()
        {
            CheckSaveDirectory();
            Task.Run(() => { WriteLogWithLockThread(); });
        }

        public void Print(LogLevel level = LogLevel.INFO, string message = null)
        {
            _LogProcessQueue.Enqueue(new LogData(level, message));
            // Console.WriteLine($"{level}, {message}");
        }

        private void WriteLogWithLockThread()
        {
            while (true)
            {
                if (_LogProcessQueue.IsEmpty)
                    continue;

                LogData logData;
                _LogProcessQueue.TryDequeue(out logData);

                lock (_LockObject)
                {
                    WriteLog(logData.Level, logData.DateTime, logData.Message);
                }
            }
        }

        public void CheckSaveDirectory()
        {
            if (!Directory.Exists(_SaveDirectoryPath))
                Directory.CreateDirectory(_SaveDirectoryPath);
        }

        public string GetFilePath()
        {
            return _FilePath;
        }

        public void SetSaveLogPath(string path)
        {
            _SaveDirectoryPath = path;
            CheckSaveDirectory();
        }

        private void WriteLog(LogLevel level, DateTime dt, string msg)
        {
            try
            {
                _FilePath = $@"{_SaveDirectoryPath}\{dt.ToString("yyyyMMdd")}.log";
                var sb = GetLogData(level, dt.ToString("yyyy-MM-dd HH:mm:ss"), msg);

                using (FileStream fs = new FileStream(_FilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(sb.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private StringBuilder GetLogData(LogLevel level, string time, string message)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(time);
            sb.Append(" | ");
            sb.Append(level.ToString());
            sb.Append(" | ");
            sb.Append(message);
            return sb;
        }
    }
}