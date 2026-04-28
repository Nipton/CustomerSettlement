using System.Windows.Controls;

namespace AccountsReceivable.View
{
    public partial class ReconciliationReportView : UserControl
    {
        public ReconciliationReportView()
        {
            InitializeComponent();
        }
        //private void Print(object sender, RoutedEventArgs e)
        //{
        //    if (CompanyComboBox.SelectedItem == null || fromDatePecker.SelectedDate == null || toDatePecker.SelectedDate == null)
        //    {
        //        MessageBox.Show("Для печати необходимо выбрать компанию и указать даты.", "", MessageBoxButton.OK, MessageBoxImage.Information);
        //        return;
        //    }
        //    List<AccountPartOne> debitList = debitDataGrid.ItemsSource.OfType<AccountPartOne>().ToList();
        //    if (debitList.Count == 0)
        //    {
        //        MessageBox.Show("Нет данных для печати.", "", MessageBoxButton.OK, MessageBoxImage.Information);
        //        return;
        //    }
        //    PrintReconciliationReport printReconciliationReport = new PrintReconciliationReport(debitList, paymentsList, (DateTime)fromDatePecker.SelectedDate, (DateTime)toDatePecker.SelectedDate);
        //    printReconciliationReport.ShowDialog();
        //}
    }
}
