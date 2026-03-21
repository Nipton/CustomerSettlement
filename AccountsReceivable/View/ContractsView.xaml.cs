using AccountsReceivable.Models;
using AccountsReceivable.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
    public partial class ContractData : UserControl
    {
        private ApplicationContext db;
        private FileIOService fileIOService;
        ObservableCollection<CompanyOld> companyList;
        List<string> nomenclatureList;
        Contract contract; 
        public ContractData()
        {
            InitializeComponent();
            fileIOService = new FileIOService();
            contract = new Contract();
            db = new ApplicationContext();
            DataContext = contract;
        }

        private async void Contract_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {
                    nomenclatureList = fileIOService.LoadFromTextFile(PathFile.nomenclature);
                    companyList = fileIOService.LoadData();
                    db.Contracts.Load();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Shutdown();
            }
            nomenclatureComboBox.ItemsSource = nomenclatureList;
            companyComboBox.ItemsSource = companyList;
            contractDataGrid.ItemsSource = db.Contracts.Local.ToObservableCollection();
        }

       
        
        private void Add_Button(object sender, RoutedEventArgs e)
        {
            try
            {
                contract.Number = numberTextBlock.Text;
                db.Contracts.Add(contract);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Shutdown();
            }
            contract = new Contract();
            DataContext = contract;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedContractsCount = contractDataGrid.SelectedItems.Count;
            
            List <Contract> selectedContracts = contractDataGrid.SelectedItems.OfType<Contract>().ToList();
            if (selectedContractsCount == 0)
            {
                MessageBox.Show("Для удаления необходимо выбрать элемент");
                return;
            }
            if (MessageBox.Show($"Вы действительно хотите удалить выделенные элементы в количестве {selectedContractsCount} шт.? Отменить действие будет невозможно.", "Внимание!",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    for(int i = 0; i < selectedContractsCount; i++)
                    {
                        if(selectedContracts[i] != null)
                        {
                            if(db.AccountsPartOne.Any(x => x.ContractID == selectedContracts[i].ID))
                            {
                                MessageBox.Show($"Невозможно удалить элемент.\nДоговор \"{selectedContracts[i].Number}\" используется в связанных таблицах. Сначала удалите все связанные данные.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);  
                            }
                            else
                            {
                                db.Contracts.Remove(selectedContracts[i]);
                                db.SaveChanges();
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Application.Current.Shutdown();
                }
            }           
        }
    }
}
