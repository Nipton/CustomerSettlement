using AccountsReceivable.Data;
using AccountsReceivable.Models;
using AccountsReceivable.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
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
using System.Windows.Shapes;

namespace AccountsReceivable.View
{
    public partial class AddAccount : Window
    {
        ApplicationContext db;
        FileIOService fileIOService;
        List<string>? nomenclatureList;
        ObservableCollection<string>? unitList;
        AccountPartTwo accTwo;
        public AddAccount(AccountPartTwo? selectedAccTwo)
        {
            InitializeComponent();
            accTwo = new AccountPartTwo();
            db = new ApplicationContext();
            fileIOService = new FileIOService();
            if (selectedAccTwo != null)
            {
                accTwo = selectedAccTwo;
            }
            DataContext = accTwo;
        }
        private async void AddAccount_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {
                    nomenclatureList = fileIOService.LoadFromTextFile(PathFile.nomenclature);
                    unitList = new ObservableCollection<string>(fileIOService.LoadFromTextFile(PathFile.unit));
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Shutdown();
            }
            nomenclatureComboBox.ItemsSource = nomenclatureList;
            unitComboBox.ItemsSource = unitList;
        }
        private void SelectAddress(object sender, RoutedEventArgs e) //фокус при клике
        {
            TextBox tb = (sender as TextBox);
            if (tb != null)
            {
                tb.SelectAll();
            }
        }
        private void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = (sender as TextBox);
            if (tb != null)
            {
                if (!tb.IsKeyboardFocusWithin)
                {
                    e.Handled = true;
                    tb.Focus();
                }
            }
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (accTwo.Number == 0)
                {
                    accTwo.Number = -1;
                    db.AccountsPartTwo.Add(accTwo);
                    db.SaveChanges();
                }
                else
                {
                    db.AccountsPartTwo.Update(accTwo);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Shutdown();
            }
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void DoubleTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textTB = (sender as TextBox)?.Text.Replace('.', ',');
            if (!double.TryParse(textTB, out _))
            {
                (sender as TextBox)!.Text = "0";
            }
            else
            {
                (sender as TextBox)!.Text = textTB;
            }
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            vatTextBox.Text = "0";
        }
        private void Сalculate()
        {
            double price = double.Parse(priceTextBox.Text.Replace('.', ','));
            double quantity = double.Parse(quantityTextBox.Text.Replace('.', ','));
            double vat = double.Parse(vatTextBox.Text.Replace('.', ','));
            double sumWithoutVAT = price * quantity;
            withoutVATTextBlock.Text = sumWithoutVAT.ToString();
            amountVATTextBlock.Text = (sumWithoutVAT * (vat / 100)).ToString();
            sumTextBlock.Text = (sumWithoutVAT * (vat / 100) + sumWithoutVAT).ToString();

        }
        private void СalculateTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (accTwo != null)
            {
                var textTB = (sender as TextBox)?.Text.Replace('.', ',');
                if (double.TryParse(textTB, out _))
                {
                    Сalculate();
                }
            }
        }
     
        private void AddUnit_Click(object sender, RoutedEventArgs e)
        {
            var selectedText = unitComboBox.Text;
            hiddenStackPanel.Visibility = Visibility.Collapsed;
            if (unitList != null && !unitList.Contains(selectedText) && !string.IsNullOrEmpty(selectedText))
            {
                try
                {
                    fileIOService.AddToTextFile(selectedText, PathFile.unit);
                    unitList.Add(selectedText);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Application.Current.Shutdown();
                }
            }
        }

        private void DeleteUnit_Click(object sender, RoutedEventArgs e)
        {
            var selectedText = unitComboBox.Text;
            hiddenStackPanel.Visibility = Visibility.Collapsed;
            if (unitList != null && unitList.Contains(selectedText) && !string.IsNullOrEmpty(selectedText))
            {
                unitList.Remove(selectedText);
                try
                {
                    fileIOService.DeleteFromTextFile(unitList.ToList<string>(), PathFile.unit);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Application.Current.Shutdown();
                }
            }
            unitComboBox.Text = "";
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
