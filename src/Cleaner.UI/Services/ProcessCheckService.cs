using Cleaner.Util;

using System.Windows;

namespace Cleaner.UI.Services
{
    public interface IProcessCheckService
    {
        bool IsOpen(string processName);
    }

    public class ProcessCheckService : IProcessCheckService
    {
        public bool IsOpen(string processName)
        {
            if (ProcessChecker.Do(processName))
            {
                string? currntProcessName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                int currentProcess = System.Diagnostics.Process.GetCurrentProcess().Id;
                System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName(currntProcessName);

                foreach (System.Diagnostics.Process process in processes)
                {
                    if (currentProcess == process.Id)
                    {
                        continue;
                    }

                    // find MainWindow Title
                    IntPtr hwnd = ProcessChecker.FindWindow(null, processName);
                    if (hwnd.ToInt32() > 0)
                    {
                        //Activate it
                        ProcessChecker.SetForegroundWindow(hwnd);

                        WindowShowStyle command = ProcessChecker.IsIconicNative(hwnd) ? WindowShowStyle.Restore : WindowShowStyle.Show;
                        ProcessChecker.ShowWindow(hwnd, command);
                    }
                }

                Application.Current.Shutdown();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
