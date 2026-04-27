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
        private void OnBackgroundClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;
            ContractDataGrid.UnselectAll();
            ContractDataGrid.Focus();
        }
    }
}
