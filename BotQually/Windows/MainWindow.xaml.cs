using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace BotQually
{
    public partial class MainWindow : Window
    {
        public static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        #pragma warning disable IDE0044 // Добавить модификатор только для чтения
        private Stack<Page> pages = new Stack<Page>();
        public NotifyIcon ni = new NotifyIcon();
        public Page CurrentPage { get; set; }

        public MainWindow()
        {
            ni.Text = "Bot Qually";
            ni = new NotifyIcon
            {
                Icon = Properties.Resources.logo2,
                Visible = true
            };
            ni.MouseDown += new MouseEventHandler(NIClick);
            InitializeComponent();
            DataContext = new WindowViewModel(this);
        }

        public void NIClick(object sender, MouseEventArgs args)
        {
            if (args.Button == MouseButtons.Left)
            {
                Show();
                WindowState = WindowState.Normal;
            }
        }

        public void ShowPage(Page newPage)
        {
            pages.Push(newPage);
            Task.Factory.StartNew(() => ShowNewPage());
        }

        private void ShowNewPage()
        {
            Dispatcher.Invoke(delegate
            {
                if (MainFrame.Content != null)
                {
                    if (MainFrame.Content is Page oldPage)
                    {
                        oldPage.Loaded -= NewPage_Loaded;
                        UnloadPage(oldPage);
                    }
                }
                else
                {
                    ShowNextPage();
                }
            });
        }

        private void ShowNextPage()
        {
            Page newPage = pages.Pop();
            newPage.Loaded += NewPage_Loaded;
            MainFrame.Content = newPage;
        }

        private void ShowNextPageAfter()
        {
            Page newPage = pages.Pop();
            newPage.Loaded += NewPageAfter_Loaded;
            MainFrame.Content = newPage;
        }

        private async void UnloadPage(Page page)
        {
            Storyboard hidePage = (Resources["GrowAndFadeOut"] as Storyboard).Clone();
            hidePage.Begin(MainFrame);
            await Task.Delay(100);
            ShowNextPageAfter();
        }

        private void NewPage_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard showNewPage = Resources["GrowAndFadeIn"] as Storyboard;
            showNewPage.Begin(MainFrame);
            CurrentPage = sender as Page;
        }

        private void NewPageAfter_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard showNewPage = Resources["GrowAndFadeInAfter"] as Storyboard;
            showNewPage.Begin(MainFrame);
            CurrentPage = sender as Page;
        }

        private void HidePage_Completed(object sender, EventArgs e)
        {
            MainFrame.Content = null;
            ShowNextPageAfter();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ni.Dispose();
            Process.GetCurrentProcess().Kill();
        }
    }
}
