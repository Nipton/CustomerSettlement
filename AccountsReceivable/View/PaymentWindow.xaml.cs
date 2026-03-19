using AccountsReceivable.Models;
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
using System.Windows.Shapes;

namespace AccountsReceivable.View
{
    public partial class PaymentWindow : Window
    {
        Payment payment;
        ApplicationContext db;
        int id;
        public PaymentWindow(Payment? editPayment,int index)
        {
            InitializeComponent();
            db = new ApplicationContext();
            payment = new Payment();
            id = index;
            if(editPayment != null )
            {
                payment = editPayment;
            }
            DataContext = payment;
        }      
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                payment.AccountID = id;
                if (payment.ID != 0)
                {
                    db.Payment.Update(payment);
                    db.SaveChanges();
                }
                else
                {
                    db.Payment.Add(payment);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }
            this.Close();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
    }
}
