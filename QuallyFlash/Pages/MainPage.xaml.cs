using System.Windows.Controls;

namespace QuallyFlash
{
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = MainViewModel.GetInstance();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                GlobalSettings.GetInstance().Sort = (sender as ComboBox).SelectedValue?.ToString();
            } catch { }
        }
    }
}
