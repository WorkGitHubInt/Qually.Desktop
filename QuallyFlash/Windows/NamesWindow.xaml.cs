using System.Windows;

namespace QuallyFlash
{
    public partial class NamesWindow : Window
    {
        public NamesWindow()
        {
            InitializeComponent();
            DataContext = new NamesViewModel(this);
        }
    }
}
