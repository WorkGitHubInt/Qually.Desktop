using System.Windows.Controls;

namespace BotQually
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
