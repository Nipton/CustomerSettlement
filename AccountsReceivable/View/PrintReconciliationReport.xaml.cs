using AccountsReceivable.Data;
using AccountsReceivable.Models;
using AccountsReceivable.Services;
using RSDN;
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
using System.Windows.Shapes;

namespace AccountsReceivable.View
{
    public partial class PrintReconciliationReport : Window
    {
        private List<AccountPartOne> accOneList;
        private List<Payment>? paymentsList;
        private ObservableCollection<DataForTable> dataForTableList;
        private FileIOService fileIOService;
        private CompanyOld organization;
        private ObservableCollection<CompanyOld>? companiesList;
        private DateTime minDate;
        private DateTime maxDate;
        private double? finalDebitBalance;
        private double? finaleCreditBalance;

        public PrintReconciliationReport(List<AccountPartOne> selectedAccOne, List<Payment>? selectedPayments, DateTime dateTime, DateTime maxDate)
        {
            InitializeComponent();
            dataForTableList = new ObservableCollection<DataForTable>();
            fileIOService = new FileIOService();
            organization = new CompanyOld();
            DataContext = dataForTableList;
            minDate = dateTime;
            accOneList = selectedAccOne;
            paymentsList = selectedPayments;
            this.maxDate = maxDate;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataGrid();
            try
            {
                companiesList = fileIOService.LoadData();
                organization = fileIOService.LoadOrganization(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }
            CompanyOld? company = companiesList?.FirstOrDefault(x => x.ShortName == accOneList[0].Company);
            dateTextBlock.Text = $"взаимных расчётов по состоянию на {maxDate:d}";
            companiesTextBlock.Text = $"между {organization.ShortName} и {accOneList[0].Company} по договору № {accOneList[0].Contract?.Number} от {accOneList[0].Contract?.Date:d}";
            textTextBlock.Text = $"Мы, нижеподписавшиеся, {organization.Position} {organization.ShortName} {organization.DirectorFullName}, с одной стороны, и {company?.Position} {company?.ShortName} {company?.DirectorFullName}, с другой стороны, составили настоящий акт сверки в том, что состояние взаимных расчетов по данным учета следующее: ";
            if(finalDebitBalance > finaleCreditBalance && finalDebitBalance!=null)
            {
                dataCompany1.Text = $"По данным {organization.ShortName}\nна {maxDate:d} задолженность в пользу {organization.ShortName} составляет {finalDebitBalance} руб. ({RusCurrency.Str((double)finalDebitBalance)})";
            }
            else if(finalDebitBalance == finaleCreditBalance)
            {
                dataCompany1.Text = $"По данным {organization.ShortName}\nна {maxDate:d} задолженность отсутствует";
            }
            else if(finalDebitBalance < finaleCreditBalance && finaleCreditBalance != null) 
            {
                dataCompany1.Text = $"По данным {organization.ShortName}\nна {maxDate:d} задолженность в пользу {company?.ShortName} составляет {finaleCreditBalance} руб. ({RusCurrency.Str((double)finaleCreditBalance)})";
            }
            dataCompany2.Text = $"По данным {company?.ShortName}";
            dataCompany3.Text = $"По данным {organization.ShortName}";
            dataCompany4.Text = $"По данным {company?.ShortName}";
            HeadCompanyTextBlock.Text = $"Руководитель: ____________ {FormationStrings.GenerateFIO(organization)}";
            HeadOrgTextBlock.Text = $"Руководитель: ____________ {FormationStrings.GenerateFIO(company)}";

        }
        private void LoadDataGrid()
        {
            double balanceDebitBefore;
            double? balanceCreditBefore;
            double? balanceDebitAfter;
            double? balanceCreditAfter;
            using (ApplicationContext db = new ApplicationContext())
            {
                var accountsBefore = db.AccountsPartOne.Where(x => x.ID != -1 && x.Company == accOneList[0].Company && x.Date < minDate);
                balanceDebitBefore = accountsBefore.Sum(x => x.Sum);
                balanceCreditBefore = accountsBefore.Sum(x => x.Payment);
            }
            DataForTable dataForTable = new DataForTable() { NameDocument = $"Сальдо на {minDate:d}" };
            if (balanceDebitBefore >= balanceCreditBefore)
            {
                dataForTable.Debit = balanceDebitBefore - balanceCreditBefore;
                dataForTable.Credit = 0;
            }
            else
            {
                dataForTable.Credit = balanceCreditBefore - balanceDebitBefore;
                dataForTable.Debit = 0;
            }
            dataForTableList.Add(dataForTable);
            foreach (var accOne in accOneList)
            {
                dataForTable = new DataForTable()
                {
                    Date = accOne.Date,
                    NameDocument = $"Счёт №{accOne.ID} от {accOne.Date.ToString("d")}",
                    Debit = accOne.Sum
                };

                dataForTableList.Add(dataForTable);
            }
            if (paymentsList != null)
            {
                foreach (var payment in paymentsList)
                {
                    dataForTable = new DataForTable()
                    {
                        Date = payment.Date,
                        NameDocument = $"п/п № {payment.Number} от {payment.Date:d}",
                        Credit = (double)payment.Sum
                    };
                    dataForTableList.Add(dataForTable);
                }
            }
            dataForTable = new DataForTable() { NameDocument = "Обороты за период" };
            balanceDebitAfter = dataForTableList.Skip(1).Sum(x => x.Debit);
            dataForTable.Debit = balanceDebitAfter;
            balanceCreditAfter = dataForTableList.Skip(1).Sum(x => x.Credit);
            dataForTable.Credit = balanceCreditAfter;
            dataForTableList.Add(dataForTable);
            dataForTable = new DataForTable() { NameDocument = $"Сальдо на {maxDate:d}" };
            if (balanceDebitBefore + balanceDebitAfter >= balanceCreditBefore + balanceCreditAfter)
            {
                finalDebitBalance = balanceDebitBefore + balanceDebitAfter - (balanceCreditBefore + balanceCreditAfter);
                dataForTable.Debit = finalDebitBalance;
                finaleCreditBalance = 0;
                dataForTable.Credit = 0;
            }
            else
            {
                finaleCreditBalance = balanceCreditBefore + balanceCreditAfter - (balanceDebitBefore + balanceDebitAfter);
                dataForTable.Credit = finaleCreditBalance;
                finalDebitBalance = 0;
                dataForTable.Debit = 0;
            }
            dataForTableList.Add(dataForTable);
            actDataGrid.ItemsSource = dataForTableList;
            List<DataForTable> emptyList= new List<DataForTable>(dataForTableList.Count);
            for (int i = 0; i < dataForTableList.Count; i++)
            {
                emptyList.Add(new DataForTable());
            }
            emptyDataGrid.ItemsSource = emptyList;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                Size pageSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);
                grid.Measure(pageSize);
                grid.Arrange(new Rect(5, 5, pageSize.Width, pageSize.Height));
                printDialog.PrintVisual(grid, "Печать");
            }
            this.Close();

        }
    }
    
    public class DataForTable
    {
        public DateTime? Date { get; set; }
        public string? NameDocument { get; set; }
        public double? Debit { get; set; }
        public double? Credit { get; set; }
    }
}
