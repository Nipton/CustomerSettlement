using AccountsReceivable.Models;
using AccountsReceivable.Services;
using AccountsReceivable.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AccountsReceivable.View
{
    public partial class CounterpartiesView : UserControl
    { 
        public CounterpartiesView()
        {
            InitializeComponent();
        }
        private void Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is not CounterpartiesViewModel vm)
                return;
            if (sender is not DataGrid grid)
                return;
            vm.SelectedItems = grid.SelectedItems.Cast<Company>().ToList();
        }
        private void OnBackgroundClick(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton != MouseButton.Left) return;
            CompaniesGrid.UnselectAll();
            CompaniesGrid.Focus();
        }
    }
}
