using AccountsReceivable.Services;
using AccountsReceivable.View;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AccountsReceivable
{
    public partial class MainWindow : Window
    {
        //private ContentControl organization;
        //private ContentControl counterparty;
        //private ContentControl nomenclature;
        //private ContentControl contract;
        //private ContentControl accounts;
        //private ContentControl archiveAccount;
        //private ContentControl reconciliationReport;
        //private ContentControl report;

        public MainWindow()
        {
            InitializeComponent();
            using(ApplicationContext db = new ApplicationContext())
            {
                db.Database.EnsureCreated();
            }
        }

        //private void Button_Organization(object sender, RoutedEventArgs e)
        //{
        //    if (MainSpace.Child == null || MainSpace.Child != organization)
        //    {
        //        organization = new Organization();
        //        MainSpace.Child = organization;
        //    }
        //}

        //private void Button_References(object sender, RoutedEventArgs e)
        //{
        //    popup.StaysOpen = true;
        //    popup.IsOpen = !popup.IsOpen;
        //}
        //private void References_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    if(popup.IsOpen) 
        //    popup.StaysOpen= false;
        //}
        //private void References_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    popup.StaysOpen = true;
        //    popup.IsOpen = !popup.IsOpen;
        //}

        //private void Counterparty_Button(object sender, RoutedEventArgs e)
        //{
        //    popup.IsOpen = false;
        //    if (MainSpace.Child == null || MainSpace.Child != counterparty)
        //    {
        //        counterparty = new Counterparties(this);
        //        MainSpace.Child = counterparty;
        //    }
        //}

        //private void Nomenclature_Button(object sender, RoutedEventArgs e)
        //{
        //    popup.IsOpen = false;
        //    if (MainSpace.Child == null || MainSpace.Child != nomenclature)
        //    {
        //        nomenclature = new Nomenclature(this);
        //        MainSpace.Child = nomenclature;
        //    }
        //}

        //private void Contract_Button(object sender, RoutedEventArgs e)
        //{
        //    popup.IsOpen = false;
        //    if (MainSpace.Child == null || MainSpace.Child != contract)
        //    {
        //        contract = new ContractData();
        //        MainSpace.Child = contract;
        //    }
        //}

        //private void AccountsButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if(MainSpace.Child == null || MainSpace.Child != accounts)
        //    {
        //        accounts = new AccountsView();
        //        MainSpace.Child = accounts;
        //    }
        //}

        //private void ArchiveButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if(MainSpace.Child == null || MainSpace.Child != archiveAccount)
        //    {
        //        archiveAccount = new ArchiveAccountView();
        //        MainSpace.Child = archiveAccount;
        //    }
        //}

        //private void ReconciliationRreport_Click(object sender, RoutedEventArgs e)
        //{
        //    if (MainSpace.Child == null || MainSpace.Child != reconciliationReport)
        //    {
        //        reconciliationReport = new ReconciliationReport();
        //        MainSpace.Child = reconciliationReport;
        //    }
        //}

        //private void ReportButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (MainSpace.Child == null || MainSpace.Child != report)
        //    {
        //        report = new ReportView();
        //        MainSpace.Child = report;
        //    }
        //}
    }
}
