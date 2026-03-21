using AccountsReceivable.Models;
using AccountsReceivable.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
    public partial class ReconciliationReport : UserControl
    {
        private ApplicationContext db;
        private FileIOService fileIOService;
        private ObservableCollection<CompanyOld>? companiesList;
        private ObservableCollection<AccountPartOne>? accountsPartOneList;
        private List<Payment>? paymentsList;
        public ReconciliationReport()
        {
            InitializeComponent();
            db = new ApplicationContext();
            fileIOService = new FileIOService();
            paymentsList = new List<Payment>();
        }

        private async void reconciliationReportView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {
                    companiesList = fileIOService.LoadData();
                    db.Contracts.Load();
                    accountsPartOneList = new ObservableCollection<AccountPartOne>(db.AccountsPartOne.Where(x => x.ID != -1).Include(x => x.Contract).ToList());

                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Shutdown();
            }
            companyComboBox.ItemsSource = companiesList;
            contractComboBox.ItemsSource = db.Contracts.Local.ToObservableCollection();
        }

        private void Search(object sender, SelectionChangedEventArgs e)
        {
            paymentsList?.Clear();
            IEnumerable<AccountPartOne>? filtredList = null;
            if (toDatePecker.SelectedDate != null && fromDatePecker.SelectedDate != null)
            {
                filtredList = accountsPartOneList?.Where(x => x.Date >= fromDatePecker.SelectedDate && x.Date <= toDatePecker.SelectedDate);

            }
            else if (toDatePecker.SelectedDate == null && fromDatePecker.SelectedDate != null)
            {
                filtredList = accountsPartOneList?.Where(x => x.Date >= fromDatePecker.SelectedDate);
            }
            else if (toDatePecker.SelectedDate != null && fromDatePecker.SelectedDate == null)
            {
                filtredList = accountsPartOneList?.Where(x => x.Date <= toDatePecker.SelectedDate);
            }
            if(companyComboBox.SelectedItem is CompanyOld selectedCompany)
            {
                filtredList = filtredList?.Where(x => x.Company == selectedCompany.Name);
            }
            if(contractComboBox.SelectedItem is Contract selectedContract)
            {
                filtredList = filtredList?.Where(x => x.ContractID == selectedContract.ID);
            }
            debitDataGrid.ItemsSource = null;
            creditDataGrid.ItemsSource = null;
            debitDataGrid.ItemsSource = filtredList?.OrderBy(x => x.Date);
            if (filtredList != null)
            {
                foreach (var item in filtredList)
                {
                    paymentsList?.AddRange(db.Payment.Where(x => x.AccountID == item.ID));
                }
            }
            creditDataGrid.ItemsSource = paymentsList;
        }
        private void ClearFilter(object sender, RoutedEventArgs e)
        {
            companyComboBox.SelectedItem = null;
            contractComboBox.SelectedItem = null;
            fromDatePecker.SelectedDate = null;
            toDatePecker.SelectedDate = null;
            debitDataGrid.ItemsSource = null;
            creditDataGrid.ItemsSource = null;
        }

        private void Print(object sender, RoutedEventArgs e)
        {
            if (companyComboBox.SelectedItem == null || fromDatePecker.SelectedDate == null || toDatePecker.SelectedDate == null)
            {
                MessageBox.Show("Для печати необходимо выбрать компанию и указать даты.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            List<AccountPartOne> debitList = debitDataGrid.ItemsSource.OfType<AccountPartOne>().ToList();
            if (debitList.Count == 0)
            {
                MessageBox.Show("Нет данных для печати.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            PrintReconciliationReport printReconciliationReport = new PrintReconciliationReport(debitList, paymentsList, (DateTime)fromDatePecker.SelectedDate, (DateTime)toDatePecker.SelectedDate);
            printReconciliationReport.ShowDialog();
        }
    }
}
