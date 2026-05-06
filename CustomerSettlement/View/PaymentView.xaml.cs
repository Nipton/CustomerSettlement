using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;


namespace CustomerSettlement.View
{
    public partial class PaymentView : Window
    {
        public PaymentView() 
        {
            InitializeComponent();
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => textBox?.SelectAll()), DispatcherPriority.Input);
        }
    }
}
