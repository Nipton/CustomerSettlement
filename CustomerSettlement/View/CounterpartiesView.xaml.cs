using System.Windows.Controls;
using System.Windows.Input;
namespace CustomerSettlement.View
{
    public partial class CounterpartiesView : UserControl
    { 
        public CounterpartiesView()
        {
            InitializeComponent();
        }
        private void OnBackgroundClick(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton != MouseButton.Left) return;
            CompaniesGrid.UnselectAll();
            CompaniesGrid.Focus();
        }
    }
}
