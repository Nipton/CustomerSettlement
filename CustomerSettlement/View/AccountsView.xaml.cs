using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CustomerSettlement.View
{
    public partial class AccountsView : UserControl
    {
        public AccountsView()
        {
            InitializeComponent();
        }
        private void OnBackgroundClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;
            var current = e.OriginalSource as DependencyObject;
            while (current != null)
            {
                if (current is DataGrid)
                    return;
                current = VisualTreeHelper.GetParent(current);
            }
            AccHeadersDataGrid.UnselectAll();
            AccHeadersDataGrid.Focus();
        }
        private void DatePicker_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var picker = (DatePicker)sender;
                var binding = picker.GetBindingExpression(DatePicker.SelectedDateProperty);
                binding?.UpdateSource();
            }
        }
    }
}
