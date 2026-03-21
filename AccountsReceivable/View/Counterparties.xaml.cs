using AccountsReceivable.Models;
using AccountsReceivable.Services;
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
    public partial class Counterparties : UserControl
    { 
        private MainWindow _main;
        private ObservableCollection<CompanyOld> companiesList;
        private FileIOService fileIOService;
        public Counterparties(MainWindow main)
        {
            InitializeComponent();
            _main = main;
        }
        public Counterparties()
        {
            InitializeComponent();
        }
        private void AddButton(object sender, RoutedEventArgs e)
        {
            AddCounterparty addCounterparty = new AddCounterparty(companiesList, null); 
            addCounterparty.ShowDialog();
        }
        private void EditButton(object sender, RoutedEventArgs e)
        {
            if(conterpariesList.SelectedItems.Count > 1)
            {
                MessageBox.Show("Для редактирования необходимо выделить только один элемент.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            CompanyOld company = (CompanyOld)conterpariesList.SelectedItem;
            AddCounterparty addCounterparty = new AddCounterparty(companiesList, company);
            addCounterparty.Title = "Редактировать контрагента";
            addCounterparty.ShowDialog();
            conterpariesList.Items.Refresh();
        }
        private void RemoveButton(object sender, RoutedEventArgs e)
        {
            List<CompanyOld> selectedList = conterpariesList.SelectedItems.OfType<CompanyOld>().ToList();
            if (MessageBox.Show($"Вы действительно хотите удалить выделенные элементы в количестве {selectedList.Count} шт.? Отменить действие будет невозможно.", "Внимание!", 
                MessageBoxButton.YesNo, MessageBoxImage.Question ) == MessageBoxResult.Yes)
            {
                try
                {
                    fileIOService.RemoveData(companiesList, selectedList);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    _main.Close();
                }
            }
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            NameScope.SetNameScope(DataGridContextMenu, NameScope.GetNameScope(this));
            fileIOService = new FileIOService();
            try
            {
                companiesList = fileIOService.LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                _main.Close();
            }
            conterpariesList.ItemsSource = companiesList;

        }

        private void SearchTermTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if(textBox!.Text != "")
            {
                var filteredList = companiesList.Where(x => x.Name.ToLower().Contains(textBox.Text.ToLower()) || x.ID.ToString().Contains(textBox.Text));
                conterpariesList.ItemsSource = null;
                conterpariesList.ItemsSource = filteredList;
            }
            else
            {
                conterpariesList.ItemsSource = companiesList;
            }
        }
    }
}
