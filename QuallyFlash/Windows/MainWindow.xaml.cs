using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace QuallyFlash
{
    public partial class MainWindow : Window
    {
        private Stack<Page> Pages = new Stack<Page>();
        public Page CurrentPage { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new WindowViewModel(this);
        }

        public void ShowPage(Page newPage)
        {
            Pages.Push(newPage);
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
                        UnloadPage();
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
            Page newPage = Pages.Pop();
            newPage.Loaded += NewPage_Loaded;
            MainFrame.Content = newPage;
        }

        private void ShowNextPageAfter()
        {
            Page newPage = Pages.Pop();
            newPage.Loaded += NewPageAfter_Loaded;
            MainFrame.Content = newPage;
        }

        private async void UnloadPage()
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
            Process.GetCurrentProcess().Kill();
        }
    }
}
