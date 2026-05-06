using CustomerSettlement.ViewModels;
using System.Windows;


namespace CustomerSettlement
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
