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
    public partial class ReportView : UserControl
    {
        private FileIOService fileIOService;
        private ApplicationContext db;
        private ObservableCollection<CompanyOld> companiesList;
        private List<string> nomenclatureList;
        private List<string> categoryList;
        private List<AccountPartOne> accountsPartOneList;

        public ReportView()
        {
            InitializeComponent();
            fileIOService = new FileIOService();
            db = new ApplicationContext();
            nomenclatureList = new List<string>();
            categoryList = new List<string>();
            accountsPartOneList = new List<AccountPartOne>();
        }

        private async void ReportView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {
                    companiesList = fileIOService.LoadData();
                    nomenclatureList = fileIOService.LoadFromTextFile(PathFile.nomenclature);
                    categoryList = fileIOService.LoadFromTextFile(PathFile.category);

                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Shutdown();
            }
            nomenclatureList.Add("Все");
            categoryList.Add("Все");
            categoryComboBox.ItemsSource = categoryList;
            nomenclatureComboBox.ItemsSource = nomenclatureList;
            categoryComboBox.SelectedIndex = categoryList.Count - 1;
            nomenclatureComboBox.SelectedIndex = nomenclatureList.Count - 1;
        }

        private void ClearFilter(object sender, RoutedEventArgs e)
        {
            fromDatePecker.SelectedDate = null;
            toDatePecker.SelectedDate = null;
        }

        private void СalculateButton_Click(object sender, RoutedEventArgs e)
        {
            ClearTextBlock();
            if (string.IsNullOrEmpty(toDatePecker.Text) || string.IsNullOrEmpty(fromDatePecker.Text))
            {
                MessageBox.Show("Необходимо указать времеменной промежуток", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            try
            {
                accountsPartOneList = db.AccountsPartOne.Where(x => x.ID != -1 && x.Date >= fromDatePecker.SelectedDate && x.Date <= toDatePecker.SelectedDate).Include(x => x.AccountsList).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Shutdown();
            }
            if(accountsPartOneList.Count == 0)
            {
                ClearTextBlock();
                MessageBox.Show("Подходящие записи не найдены", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            List<AccountPartOne> filtredAccOneList = new List<AccountPartOne>();
            string selectedCategory = categoryComboBox.Text;
            if (selectedCategory == "Все")
            {
                filtredAccOneList = accountsPartOneList;
            }
            else
            {
                foreach (var accOneItem in accountsPartOneList)
                {
                    var company = companiesList.FirstOrDefault(x => x.Name == accOneItem.Company);
                    if (company != null && company.Category == selectedCategory)
                    {
                        filtredAccOneList.Add(accOneItem);
                    }
                }
            }
            List<AccountPartTwo> finalAccounts = new();
            if (nomenclatureComboBox.Text == "Все")
            {
                foreach (var accOne in filtredAccOneList)
                {
                    finalAccounts.AddRange(accOne.AccountsList);
                }
            }
            else
            {
                foreach (var accOne in filtredAccOneList)
                {
                    finalAccounts.AddRange(accOne.AccountsList.Where(x => x.Nomenclature == nomenclatureComboBox.Text));
                }
            }
            if (finalAccounts.Count == 0)
            {
                ClearTextBlock();
                MessageBox.Show("Подходящие записи не найдены", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            nomenclaturaTextBlock.Text = nomenclatureComboBox.Text;
            if (nomenclatureComboBox.Text != "Все")
            {
                unitTextBlock.Text = finalAccounts[0].Unit;
                volumeTextBlock.Text = finalAccounts.Sum(x => x.Quantity).ToString();
            }
            sumTextBlock.Text = finalAccounts.Sum(x => x.Sum).ToString("N2");
        }
        private void ClearTextBlock()
        {
            sumTextBlock.Text = "";
            unitTextBlock.Text = "";
            volumeTextBlock.Text = "";
            nomenclaturaTextBlock.Text = "";
        }
    }
}
