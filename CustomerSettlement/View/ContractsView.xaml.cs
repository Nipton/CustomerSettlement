using System.Windows.Controls;
using System.Windows.Input;

namespace CustomerSettlement.View
{
    public partial class ContractView : UserControl
    {
        public ContractView()
        {
            InitializeComponent();
        }     
        private void OnBackgroundClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;
            ContractDataGrid.UnselectAll();
            ContractDataGrid.Focus();
        }
    }
}
