using System.Diagnostics;
using System.Globalization;
using System.Windows;

namespace QuallyFlash
{
    //public static class ProcessExtensions
    //{
    //    private static string FindIndexedProcessName(int pid)
    //    {
    //        var processName = Process.GetProcessById(pid).ProcessName;
    //        var processesByName = Process.GetProcessesByName(processName);
    //        string processIndexdName = null;

    //        for (var index = 0; index < processesByName.Length; index++)
    //        {
    //            processIndexdName = index == 0 ? processName : processName + "#" + index;
    //            var processId = new PerformanceCounter("Process", "ID Process", processIndexdName);
    //            if ((int)processId.NextValue() == pid)
    //            {
    //                return processIndexdName;
    //            }
    //        }

    //        return processIndexdName;
    //    }

    //    private static Process FindPidFromIndexedProcessName(string indexedProcessName)
    //    {
    //        var parentId = new PerformanceCounter("Process", "Creating Process ID", indexedProcessName);
    //        return Process.GetProcessById((int)parentId.NextValue());
    //    }

    //    public static Process Parent(this Process process)
    //    {
    //        return FindPidFromIndexedProcessName(FindIndexedProcessName(process.Id));
    //    }
    //}

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //if (Process.GetCurrentProcess().Parent().ProcessName == "cmd")
            //{
            //    Process.GetCurrentProcess().Kill();
            //}
            base.OnStartup(e);
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
            IoC.Setup();
            Current.MainWindow = new MainWindow();
            if (e.Args.Length > 0 && e.Args[0] == "-usb")
            {
                (Current.MainWindow as MainWindow).Usb = e.Args[1];
            }
            Current.MainWindow.Show();
        }
    }
}
