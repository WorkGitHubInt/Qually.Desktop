using System.Windows.Controls;

namespace QuallyFlash
{
    public partial class ReportPage : Page
    {
        public ReportPage()
        {
            InitializeComponent();
            DataContext = new ReportViewModel();
        }
    }
}
