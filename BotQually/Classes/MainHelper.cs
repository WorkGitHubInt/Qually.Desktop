using System.Windows;
using System.Windows.Controls;

namespace BotQually
{
    public static class MainHelper
    {
        public static string ProgramId = "1";

        public static void ShowPage(Page page)
        {
            ((MainWindow)Application.Current.MainWindow).ShowPage(page);
        }

        public static string GetProgramVersion()
        {
            return typeof(MainWindow).Assembly.GetName().Version.ToString();
        }
    }
}
