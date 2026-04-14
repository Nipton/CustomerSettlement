using AccountsReceivable.ViewModels;
using System.Windows;


namespace AccountsReceivable
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;;
        }
    }
}
