using AccountsReceivable.Data;
using AccountsReceivable.Models;
using AccountsReceivable.ViewModels;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace AccountsReceivable.View
{
    public partial class ContractView : UserControl
    {
        public ContractView()
        {
            InitializeComponent();
        }     
        private void ContractDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not DataGrid grid)
                return;
            if (DataContext is not ContractViewModel vm)
                return;
            vm.SelectedContracts = grid.SelectedItems.Cast<Contract>().ToList();
        }

        private void OnBackgroundClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;
            ContractDataGrid.UnselectAll();
            ContractDataGrid.Focus();
        }
    }
}
