using AccountsReceivable.Services;
using System;
using System.Collections.Generic;
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
    public partial class NomenclatureView : UserControl
    {
        private FileIOService fileIOService;
        private Window _window;
        private List<string> nomenclatureList;
        public NomenclatureView(Window window)
        {
            InitializeComponent();
            _window = window;
            fileIOService= new FileIOService();
            nomenclatureList = new List<string>();
        }
        public NomenclatureView()
        {
            InitializeComponent();
            fileIOService = new FileIOService();
            nomenclatureList = new List<string>();
        }

        private void Add_Nomenclature(object sender, RoutedEventArgs e)
        {
            string newItem = nomenclatureTextBox.Text;
            if (newItem.Length == 0)
            {
                return;
            }
            try
            {
                fileIOService.AddToTextFile(newItem, PathFile.nomenclature);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                _window.Close();
            }
            nomenclatureList.Add(newItem);
            nomenclatureTextBox.Text = "";
            nomenclatureListBox.Items.Refresh();


        }

        private void Nomenclature_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                nomenclatureList = fileIOService.LoadFromTextFile(PathFile.nomenclature);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                _window.Close();
            }
            nomenclatureListBox.ItemsSource = nomenclatureList;
        }

        private void Delete_Button(object sender, RoutedEventArgs e)
        {
            if(nomenclatureListBox.SelectedItem == null)
            { 
                return; 
            }
            if (MessageBox.Show($"Вы действительно хотите удалить выделенный элемент? Отменить действие будет невозможно.", "Внимание!",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                nomenclatureList.RemoveAt(nomenclatureListBox.SelectedIndex);
                try
                {
                    fileIOService.DeleteFromTextFile(nomenclatureList, PathFile.nomenclature);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    _window.Close();
                }
            }
            nomenclatureListBox.Items.Refresh();
        }
    }
}
