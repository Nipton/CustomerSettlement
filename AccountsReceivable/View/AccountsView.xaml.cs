using AccountsReceivable.Data;
using AccountsReceivable.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace AccountsReceivable.View
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

        private void PrintAccount(object sender, RoutedEventArgs e)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                if (AccHeadersDataGrid.SelectedItem is AccountPartOne selectedAccOne)
                {
                    PrintAccount printAccount = new PrintAccount(selectedAccOne);
                    printAccount.ShowDialog();
                }
            }
        }

        private void PrintAct(object sender, RoutedEventArgs e)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                if (AccHeadersDataGrid.SelectedItem is AccountPartOne selectedAccOne)
                {
                    PrintAct printAccount = new PrintAct(selectedAccOne);
                    printAccount.ShowDialog();
                    if(printAccount.PrintStatus)
                    {
                        selectedAccOne.ActStatus = true;
                        db.AccountsPartOne.Update(selectedAccOne);
                        db.SaveChanges();
                    }
                }
            }
        }
    }
}
