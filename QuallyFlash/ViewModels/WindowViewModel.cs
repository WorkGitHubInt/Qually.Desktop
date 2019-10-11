using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace QuallyFlash
{
    public class WindowViewModel
    {
        private readonly Window window;

        #region Commands
        public ICommand MinimizeCommand { get; set; }
        public ICommand MaximizeCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand MenuCommand { get; set; }
        public ICommand SendMessageCommand { get; set; }
        public ICommand OpenGuideCommand { get; set; }
        #endregion

        public string Logo { get; set; } = "Qually Flash";

        public WindowViewModel(Window window)
        {
            this.window = window;
            MinimizeCommand = new RelayCommand(() => window.WindowState = WindowState.Minimized);
            MaximizeCommand = new RelayCommand(() => window.WindowState ^= WindowState.Maximized);
            CloseCommand = new RelayCommand(() => window.Close());
            MenuCommand = new RelayCommand(() => SystemCommands.ShowSystemMenu(window, GetMousePosition()));
            SendMessageCommand = new RelayCommand(() => ((MainWindow)Application.Current.MainWindow).ShowPage(new ReportPage()));
            OpenGuideCommand = new RelayCommand(() => Process.Start("https://botqually.ru/Programs/QFGuide"));
            var resizer = new WindowResizer(window);
            Logo = $"Qually Flash ({typeof(MainWindow).Assembly.GetName().Version.ToString().Substring(0, typeof(MainWindow).Assembly.GetName().Version.ToString().Length - 4)})";
        }

        private Point GetMousePosition()
        {
            var position = Mouse.GetPosition(window);
            return new Point(position.X + window.Left, position.Y + window.Top);
        }
    }
}
