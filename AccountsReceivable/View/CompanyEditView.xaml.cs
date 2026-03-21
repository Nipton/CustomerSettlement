using AccountsReceivable.Models;
using AccountsReceivable.Services;
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
using static System.Net.Mime.MediaTypeNames;

namespace AccountsReceivable.View
{
    public partial class CompanyEditView : Window
    {
        private FileIOService fileIOService = new FileIOService();
        private Window _window;
        private ObservableCollection<CompanyOld> _companiesList;
        private CompanyOld currentCompany;
        private CompanyOld original;
        private ObservableCollection<string>? categoryList;
        public CompanyEditView(Window window, ObservableCollection<CompanyOld> companiesList, CompanyOld? selectedCompany)
        {
            InitializeComponent();
            original = new CompanyOld();
            _window = window;
            _companiesList= companiesList;
            if(selectedCompany != null)
            {
                original = selectedCompany;
                original.errorCollection = new Dictionary<string, string?>();
            }
            currentCompany = (CompanyOld)original.Clone();
            DataContext = currentCompany;
        }
        public CompanyEditView()
        {
            InitializeComponent();
        }
        public CompanyEditView(Window window, CompanyOld? organization) 
        {
            InitializeComponent();
            chapterTextBlock.Text = "Организация";
            original = new CompanyOld();
            _window = window;
            if(organization != null)
            {
                original = organization;
                original.errorCollection = new Dictionary<string, string?>();
            }
            currentCompany = (CompanyOld)original.Clone();
            DataContext = currentCompany;
        }

        private void SaveButton(object sender, RoutedEventArgs e)
        {
            if (currentCompany.ID == 0)
            {
                try
                {
                    fileIOService.SaveData(currentCompany);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    _window.Close();
                    return;
                }
                _companiesList.Add(currentCompany);
                MessageBox.Show("Контрагент добавлен.");
            }
            else if (currentCompany.ID == -1)
            {
                try
                {
                    fileIOService.SaveOrganization(currentCompany);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    _window.Close();
                    return;
                }
                MessageBox.Show("Данные обновлены.");
            }
            else
            {
                try
                {
                    fileIOService.EditData(currentCompany);
                    _companiesList[_companiesList.IndexOf(original)] = currentCompany;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    _window.Close();
                    return;
                }
                
                MessageBox.Show("Контрагент изменён.");
            }
            _window.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _window.Close();
        }

        // для категорий все последующие 
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {
                    categoryList = new ObservableCollection<string>(fileIOService.LoadFromTextFile(PathFile.category));
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                _window.Close();
            }
            category.ItemsSource = categoryList;
        }
        private void AddUnit_Click(object sender, RoutedEventArgs e)
        {
            var selectedText = category.Text;
            hiddenStackPanel.Visibility = Visibility.Collapsed;
            if (categoryList != null && !categoryList.Contains(selectedText) && !string.IsNullOrEmpty(selectedText))
            {
                try
                {
                    fileIOService.AddToTextFile(selectedText, PathFile.category);
                    categoryList.Add(selectedText);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    _window.Close();
                }
            }
        }

        private void DeleteUnit_Click(object sender, RoutedEventArgs e)
        {
            var selectedText = category.Text;
            hiddenStackPanel.Visibility = Visibility.Collapsed;
            if (categoryList != null && categoryList.Contains(selectedText) && !string.IsNullOrEmpty(selectedText))
            {
                categoryList.Remove(selectedText);
                try
                {
                    fileIOService.DeleteFromTextFile(categoryList.ToList<string>(), PathFile.category);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    _window.Close();
                }
            }
            category.Text = "";
        }

        private void OptionUnit_Click(object sender, RoutedEventArgs e)
        {
            hiddenStackPanel.Visibility = Visibility.Visible;
            hiddenStackPanel.Focusable = true;
            hiddenStackPanel.Focus();
        }

        private void hiddenStackPanel_LostFocus(object sender, RoutedEventArgs e)
        {
            if(!hiddenStackPanel.IsMouseOver)
                hiddenStackPanel.Visibility = Visibility.Collapsed;
        }

    }
}
