using Ninject;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace BotQually
{
    public class WindowViewModel : BaseViewModel
    {
        private readonly Window window;

        #region Commands
        public ICommand MinimizeCommand { get; set; }
        public ICommand MaximizeCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand MenuCommand { get; set; }
        public ICommand SettingsCommand { get; set; }
        public ICommand SendMessageCommand { get; set; }
        public ICommand OpenGuideCommand { get; set; }
        #endregion

        public WindowViewModel(Window window)
        {
            this.window = window;
            MinimizeCommand = new RelayCommand(() => Minimize(window));
            MaximizeCommand = new RelayCommand(() => window.WindowState ^= WindowState.Maximized);
            CloseCommand = new RelayCommand(() => window.Close());
            MenuCommand = new RelayCommand(() => SystemCommands.ShowSystemMenu(window, GetMousePosition()));
            SendMessageCommand = new RelayCommand(() => ((MainWindow)Application.Current.MainWindow).ShowPage(new ReportPage()));
            SettingsCommand = new RelayCommand(() =>
            {
                IoC.Kernel.Get<MainViewModel>().GlobalSettingsOpen = !IoC.Kernel.Get<MainViewModel>().GlobalSettingsOpen;
            });
            OpenGuideCommand = new RelayCommand(() => Process.Start("https://botqually.ru/Programs/BQGuide"));
            var resizer = new WindowResizer(window);
        }

        private void Minimize(Window window)
        {
            bool tray = IoC.Kernel.Get<MainViewModel>().GlobalSettings.Tray;
            if (tray)
            {
                window.Hide();
            }
            window.WindowState = WindowState.Minimized;
        }

        private Point GetMousePosition()
        {
            var position = Mouse.GetPosition(window);
            return new Point(position.X + window.Left, position.Y + window.Top);
        }
    }
}
