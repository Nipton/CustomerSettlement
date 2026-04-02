using AccountsReceivable.Data;
using AccountsReceivable.Models;
using AccountsReceivable.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Policy;
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
    public partial class ArchiveAccountView : UserControl
    {
        private ApplicationContext db;
        private FileIOService fileIOService;
        private ObservableCollection<CompanyOld>? companiesList;
        private ObservableCollection<AccountPartOne>? accountsPartOneList;
        private ObservableCollection<Payment>? paymentsList;
        private AccountPartOne? accOne;
        public ArchiveAccountView()
        {
            InitializeComponent();
            db = new ApplicationContext();
            fileIOService = new FileIOService();
            accOne= new AccountPartOne();
        }

        private async void ArchiveAccountView_Loaded(object sender, RoutedEventArgs e)
        {
            NameScope.SetNameScope(DataGridContextMenu, NameScope.GetNameScope(this));
            NameScope.SetNameScope(DataGridContextMenu2, NameScope.GetNameScope(this));
            NameScope.SetNameScope(DataGridContextMenu3, NameScope.GetNameScope(this));
            try
            {
                await Task.Run(() =>
                {
                    companiesList = fileIOService.LoadData();
                    accountsPartOneList = new ObservableCollection<AccountPartOne>(db.AccountsPartOne.Where(x => x.ID != -1).Include(x => x.Contract).ToList());

                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Shutdown();
            }
            accounts1DataGrid.ItemsSource = accountsPartOneList?.OrderByDescending(x => x.ID);
            companyComboBox.ItemsSource = companiesList;
        }

        private void accounts1DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            accOne = accounts1DataGrid.SelectedItem as AccountPartOne;
            if (accOne != null)
            {
                try
                {
                    accounts2DataGrid.ItemsSource = db.AccountsPartTwo.Where(x => x.Number == accOne.ID).ToList();
                    paymentDataGrid.ItemsSource = db.Payment.Where(x => x.AccountID == accOne.ID).ToList();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Application.Current.Shutdown();
                }              
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var index = accounts1DataGrid.SelectedIndex;
            using (ApplicationContext db = new ApplicationContext())
            {
                int option = 1;
                AccountPartOne? selectedAccOne = accounts1DataGrid.SelectedItem as AccountPartOne;
                if (selectedAccOne != null)
                {                  
                    var accOneT = db.AccountsPartOne.AsNoTracking().FirstOrDefault(x => x.ID == selectedAccOne.ID);
                    if ((sender as MenuItem)!.Header.ToString() == "Редактировать счета")
                    {
                        option = 2;
                    }
                    EditAccountWindow editAccount = new EditAccountWindow(accOneT!, option);
                    editAccount.Title = "Редактировать счёт";
                    editAccount.ShowDialog();                  
                    var accBuffer = db.AccountsPartTwo.Where(x => x.Number == accOneT!.ID);
                    if (accBuffer.Any())
                    {
                        accOneT!.Sum = accBuffer.Sum(x => x.Sum);
                    }
                    if(accOneT!.Payment != null)
                    {
                        if (accOneT.Payment >= accOneT.Sum)
                        {
                            accOneT.PaymentStatus = true;
                        }
                        else
                        {
                            accOneT.PaymentStatus = false;
                        }
                    }
                    db.AccountsPartOne.Update(accOneT!);
                    db.SaveChanges();                    
                }
            }
            if (accOne != null)
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    accountsPartOneList = new ObservableCollection<AccountPartOne>(db.AccountsPartOne.Where(x => x.ID != -1).Include(x => x.Contract).ToList());
                    accounts1DataGrid.ItemsSource = accountsPartOneList?.OrderByDescending(x => x.ID);
                    accounts1DataGrid.SelectedIndex = index;
                    accounts2DataGrid.ItemsSource = db.AccountsPartTwo.Where(x => x.Number == accOne.ID).ToList();
                }
            }
        }
        private void DeleteAccount(object sender, RoutedEventArgs e)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                if (accounts1DataGrid.SelectedItem is AccountPartOne selectedAccOne)
                {
                    if (MessageBox.Show($"Вы действительно хотите удалить выделенный элемент? Отменить действие будет невозможно.", "Внимание!",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        db.AccountsPartOne.Remove(selectedAccOne);
                        db.SaveChanges();

                    }
                }
            }
            try
            {
                accountsPartOneList = new ObservableCollection<AccountPartOne>(db.AccountsPartOne.Where(x => x.ID != -1).Include(x => x.Contract).ToList());
                accounts1DataGrid.ItemsSource = accountsPartOneList?.OrderByDescending(x => x.ID);
                accounts2DataGrid.ItemsSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Shutdown();
            }

        }

        private void SearchByCompanies(object sender, SelectionChangedEventArgs e)
        {
            if (companyComboBox.SelectedItem is CompanyOld selectedCompany)
            {
                var filtredList = accountsPartOneList?.Where(x => x.Company == selectedCompany.Name).OrderByDescending(x => x.ID);
                accounts1DataGrid.ItemsSource = null;
                accounts1DataGrid.ItemsSource = filtredList;
            }
        }
        private void ClearFilter(object sender, RoutedEventArgs e)
        {
            companyComboBox.SelectedItem = null;
            fromDatePecker.SelectedDate = null;
            toDatePecker.SelectedDate = null;
            accounts1DataGrid.ItemsSource = null;
            accounts1DataGrid.ItemsSource = accountsPartOneList?.OrderByDescending(x => x.ID);
        }

        private void SearchByDate(object sender, SelectionChangedEventArgs e)
        {
            IEnumerable<AccountPartOne>? filtredList = null;
            if (toDatePecker.SelectedDate != null && fromDatePecker.SelectedDate != null)
            {
                filtredList = accountsPartOneList?.Where(x => x.Date >= fromDatePecker.SelectedDate && x.Date <= toDatePecker.SelectedDate);
                
            }
            else if (toDatePecker.SelectedDate == null && fromDatePecker.SelectedDate != null)
            {
                filtredList = accountsPartOneList?.Where(x => x.Date >= fromDatePecker.SelectedDate);
            }
            else if(toDatePecker.SelectedDate != null && fromDatePecker.SelectedDate == null)
            {
                filtredList = accountsPartOneList?.Where(x => x.Date <= toDatePecker.SelectedDate);
            }
            accounts1DataGrid.ItemsSource = null;
            accounts1DataGrid.ItemsSource = filtredList?.OrderByDescending(x => x.ID);
        }

        private void AddPayment(object sender, RoutedEventArgs e)
        {
            if (accounts1DataGrid.SelectedItem is AccountPartOne selectedAccOne)
            {
                PaymentWindow paymentWindow = new PaymentWindow(null, selectedAccOne.ID);
                paymentWindow.ShowDialog();
                paymentDataGrid.ItemsSource = db.Payment.Where(x => x.AccountID == selectedAccOne.ID).ToList();
                RecalculationPayment(selectedAccOne);
            }         
        }
        private void EditPayment(object sender, RoutedEventArgs e)
        {
            if (accounts1DataGrid.SelectedItem is AccountPartOne selectedAccOne)
            {
                if (paymentDataGrid.SelectedItems.Count != 1)
                {
                    MessageBox.Show("Для редактирования нужно выбрать только один элемент");
                    return;
                }
                if (paymentDataGrid.SelectedItem is Payment selectedPayment)
                {
                    PaymentWindow paymentWindow = new PaymentWindow(selectedPayment, selectedAccOne.ID);
                    paymentWindow.ShowDialog();
                    RecalculationPayment(selectedAccOne);
                }
            }
        }
        private void DeletePayment(object sender, RoutedEventArgs e)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                List<Payment> selectedList = paymentDataGrid.SelectedItems.OfType<Payment>().ToList();
                if (MessageBox.Show($"Вы действительно хотите удалить выделенные элементы в количестве {selectedList.Count} шт.? Отменить действие будет невозможно.", "Внимание!",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        db.Payment.RemoveRange(selectedList);
                        db.SaveChanges();
                    }
                    catch (Exception es)
                    {
                        MessageBox.Show(es.Message);
                        Application.Current.Shutdown();
                    }
                }
                if (accounts1DataGrid.SelectedItem is AccountPartOne selectedAccOne)
                    RecalculationPayment(selectedAccOne);
            }
        }

        private void RecalculationPayment(AccountPartOne selectedAccOne)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var recalculatedPayment = db.Payment.Where(x => x.AccountID == selectedAccOne.ID);
                if (recalculatedPayment.Any())
                {
                    selectedAccOne.Payment = recalculatedPayment.Sum(x => x.Sum);
                }
                if (selectedAccOne.Payment >= selectedAccOne.Sum)
                {
                    selectedAccOne.PaymentStatus = true;
                }
                else
                {
                    selectedAccOne.PaymentStatus = false;
                }
                db.AccountsPartOne.Update(selectedAccOne);
                db.SaveChanges();
            }
            try
            {
                accountsPartOneList = new ObservableCollection<AccountPartOne>(db.AccountsPartOne.Where(x => x.ID != -1).Include(x => x.Contract).ToList());
                accounts1DataGrid.ItemsSource = accountsPartOneList?.OrderByDescending(x => x.ID);
                paymentDataGrid.ItemsSource = db.Payment.Where(x => x.AccountID == selectedAccOne.ID).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Shutdown();
            }
        }

        private void PrintAccount(object sender, RoutedEventArgs e)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                if (accounts1DataGrid.SelectedItem is AccountPartOne selectedAccOne)
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
                if (accounts1DataGrid.SelectedItem is AccountPartOne selectedAccOne)
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
