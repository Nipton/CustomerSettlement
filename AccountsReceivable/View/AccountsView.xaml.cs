using AccountsReceivable.Data;
using AccountsReceivable.Models;
using AccountsReceivable.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
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

namespace AccountsReceivable.View
{
    public partial class AccountsView : UserControl
    {
        private ApplicationContext db;
        private FileIOService fileIOService;
        ObservableCollection<CompanyOld>? companiesList;
        AccountPartOne currentAccountOne;
        int option = 0;
        public AccountsView()
        {
            InitializeComponent();
            fileIOService = new FileIOService();
            db = new ApplicationContext();
            currentAccountOne = new AccountPartOne();
            DataContext = currentAccountOne;           
        }
        public AccountsView(AccountPartOne accOne, int option)
        {
            this.option= option;
            InitializeComponent();
            fileIOService = new FileIOService();
            db = new ApplicationContext();
            currentAccountOne = accOne;
            currentAccountOne.errorCollection = new Dictionary<string, string?>();
            DataContext = currentAccountOne;
            if(option== 2) 
            {
                topSP.Visibility = Visibility.Collapsed;
                firstRow.Height = new GridLength(0);
                //saveButton.Visibility = Visibility.Collapsed;
                saveButton.Content = "Закрыть";
                saveButton.Margin = new Thickness(0, 0, 10, 0);
            }
            if(option== 1)
            {
                grid.Margin = new Thickness(0, 0, 20, 0);
                saveButton.Margin= new Thickness(0, 0, 10, 20);
                firstSP.Margin = new Thickness(10);
                SecondSP.Margin = new Thickness(10);
                ThirdSP.Margin = new Thickness(10);
                firstSP.HorizontalAlignment = HorizontalAlignment.Right;
                SecondSP.HorizontalAlignment = HorizontalAlignment.Right;
                ThirdSP.HorizontalAlignment = HorizontalAlignment.Right;

                topSP.Orientation = Orientation.Vertical;
                topSP.HorizontalAlignment = HorizontalAlignment.Left;

                firstRow.Height = GridLength.Auto;
                accountsDataGrid.Visibility = Visibility.Collapsed;
                addButton.Visibility = Visibility.Collapsed;
                dockPanel.HorizontalAlignment= HorizontalAlignment.Right;
                dockPanel.Margin= new Thickness(0, 15, 0, 0);

            }
        }

        private async void AccountsView_Loaded(object sender, RoutedEventArgs e)
        {
            NameScope.SetNameScope(DataGridRowContextMenu, NameScope.GetNameScope(this));
            try
            {
                await Task.Run(() =>
                {
                    companiesList = fileIOService.LoadData();
                    db.Contracts.Load();     
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Shutdown();
            }
            companyComboBox.ItemsSource = companiesList;
            contractComboBox.ItemsSource = db.Contracts.Local.ToObservableCollection();
            ReloadDG();
            if (currentAccountOne.ID!= 0)
                contractComboBox.SelectedItem = db.Contracts.FirstOrDefault(x => x.ID == currentAccountOne.ContractID);
            
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddAccount addAccount;
            if (currentAccountOne.ID == 0)
            {
                addAccount = new AddAccount(null);
            }
            else
            {
                AccountPartTwo accTwo = new AccountPartTwo();
                accTwo.Number = currentAccountOne.ID;
                addAccount = new AddAccount(accTwo);
            }
            addAccount.ShowDialog();
            ReloadDG();
            
        }

        private void ReloadDG()
        {
            try
            {
                if (currentAccountOne.ID == 0)
                {
                    db.AccountsPartTwo.Where(x => x.Number == -1).Load();
                    accountsDataGrid.ItemsSource = db.AccountsPartTwo.Local.ToObservableCollection();
                }
                else
                {
                    db.AccountsPartTwo.Where(x => x.Number == currentAccountOne.ID).Load();
                    accountsDataGrid.ItemsSource = db.AccountsPartTwo.Local.ToObservableCollection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Shutdown();
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if(option == 2)
            {
                Window.GetWindow(this).Close();
                return;
            }
            try
            {
                if (currentAccountOne.ID == 0)
                {
                    var accBuffer = db.AccountsPartTwo.Where(x => x.Number == -1);                    
                    if (!accBuffer.Any())
                    {
                        MessageBox.Show("Необходимо добавить хотя бы один счёт");
                        return;
                    }
                    currentAccountOne.Sum = accBuffer.Sum(x => x.Sum);
                    db.AccountsPartOne.Add(currentAccountOne);
                    db.SaveChanges();
                    foreach (var item in accBuffer)
                    {
                        item.Number = currentAccountOne.ID;
                    }
                    db.SaveChanges();
                }
                else
                {
                    //var accBuffer = db.AccountsPartTwo.Where(x => x.Number == currentAccountOne.ID);
                    //if (!accBuffer.Any())
                    //{
                    //    MessageBox.Show("Необходимо добавить хотя бы один счёт");
                    //    return;
                    //}
                    //currentAccountOne.Sum = accBuffer.Sum(x => x.Sum);
                    db.AccountsPartOne.Update(currentAccountOne);
                    //var originalItem = db.AccountsPartOne.Find(currentAccountOne.ID);
                    //db.Entry(originalItem).CurrentValues.SetValues(currentAccountOne);
                    db.SaveChanges();
                    Window.GetWindow(this).Close();
                    return;
                }
            }
            catch (Exception es)
            {
                MessageBox.Show(es.Message);
                Application.Current.Shutdown();
            }
            currentAccountOne = new AccountPartOne();
            DataContext = currentAccountOne;
            db.AccountsPartTwo.Local.Clear();
        }
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (accountsDataGrid.SelectedItems.Count > 1)
            {
                MessageBox.Show("Для редактирования необходимо выделить только один элемент.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            AccountPartTwo accountPartTwo = (AccountPartTwo)accountsDataGrid.SelectedItem;
            AddAccount addAccount = new AddAccount(accountPartTwo);
            addAccount.Title = "Редактирование";
            addAccount.ShowDialog();
            accountsDataGrid.Items.Refresh();
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            List<AccountPartTwo> selectedList = accountsDataGrid.SelectedItems.OfType<AccountPartTwo>().ToList();
            if (MessageBox.Show($"Вы действительно хотите удалить выделенные элементы в количестве {selectedList.Count} шт.? Отменить действие будет невозможно.", "Внимание!",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    db.AccountsPartTwo.RemoveRange(selectedList);
                    db.SaveChanges();
                }
                catch (Exception es)
                {
                    MessageBox.Show(es.Message);
                    Application.Current.Shutdown();
                }
            }
        }
        private void DeleteAll_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Вы действительно хотите удалить все элементы? Отменить действие будет невозможно.", "Внимание!",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {

                try
                {
                    db.AccountsPartTwo.RemoveRange(db.AccountsPartTwo.Local);
                    db.SaveChanges();
                }
                catch (Exception es)
                {
                    MessageBox.Show(es.Message);
                    Application.Current.Shutdown();
                }
            }
        }
    }
}
