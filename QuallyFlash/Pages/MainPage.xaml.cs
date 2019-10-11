using Ninject;
using System.Windows.Controls;

namespace QuallyFlash
{
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = IoC.Kernel.Get<MainViewModel>();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Properties.Settings.Default.Sort = (sender as ComboBox).SelectedValue.ToString();
                Properties.Settings.Default.Save();
            } catch { }
        }
    }
}
