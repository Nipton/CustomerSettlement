using AccountsReceivable.Models;
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
    /// <summary>
    /// Логика взаимодействия для EditAccountWindow.xaml
    /// </summary>
    public partial class EditAccountWindow : Window
    {
        private ContentControl accountsView;
        public EditAccountWindow(AccountPartOne accOne, int option)
        {
            InitializeComponent();
            accountsView = new AccountsView(accOne, option);
            editAccount.Child= accountsView;
        }
    }
}
