using AccountsReceivable.Helpers;
using AccountsReceivable.Models;
using AccountsReceivable.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AccountsReceivable.View
{
    public partial class PrintAct : Window
    {
        private AccountPartOne accOne;
        private List<AccountPartTwo> accTwo;
        private FileIOService fileIOService;
        private ObservableCollection<CompanyOld>? companiesList;
        private CompanyOld organization;
        private bool printStatus = false;
        public bool PrintStatus
        {
            get { return printStatus; }
            set { printStatus = value; }
        }
        public PrintAct(AccountPartOne selectedAccOne)
        {
            InitializeComponent();
            fileIOService = new FileIOService();
            organization = new CompanyOld();
            accOne = selectedAccOne;
            accTwo = accOne.AccountsList;
            DataContext = accOne;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            //const double pixelsInCentimeter = 96 / 2.54;
            if (printDialog.ShowDialog() == true)
            {
                Size pageSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);
                grid.Measure(pageSize);
                grid.Arrange(new Rect(5, 5, pageSize.Width, pageSize.Height));
                //grid2.Height = 29.7 * pixelsInCentimeter;
                printDialog.PrintVisual(grid, "Печать");
                PrintStatus= true;
            }
            this.Close();

        }

        private void PrintAct_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                companiesList = fileIOService.LoadData();
                organization = fileIOService.LoadOrganization();   
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }
            var company = companiesList?.FirstOrDefault(x => x.Name == accOne.Company);
            recipient2TextBlock.Text = FormationStrings.GenerateCompanyData(organization);
            foreach (var item in accTwo)
            {
                if(item.Period != null)
                {
                    item.Nomenclature = $"{item.Nomenclature} за {item.Period:Y}";
                }
            }
            dataGrid.ItemsSource = accTwo;
            numperTextBlock.Text = accOne.ID.ToString();
            dateTextBlock.Text = accOne.Date.ToString("D");
            totalTextBlock.Text = accOne.Sum.ToString("N2");
            customerTextBlock2.Text = $"Представитель Заказчика: ____________ {FormationStrings.GenerateFIO(company)}";
            executorTextBlock.Text = $"Представитель Исполнителя: ____________ {FormationStrings.GenerateFIO(organization)}";
            double sumVat = 0;
            foreach (var acc in accTwo)
            {
                if(acc.VAT != 0)
                {
                    sumVat += acc.Sum -  acc.Price * acc.Quantity;
                }
            }
            if(sumVat == 0)
            {
                vatStrTextBlock.Text = "Без налога НДС";
            }
            else
            {
                vatStrTextBlock.Text = "В том числе НДС:";
                sumVatTextBlock.Text = sumVat.ToString("N2");
            }
            quantityTextBlock.Text = $"Всего оказано услуг {accTwo.Count}, на сумму {accOne.Sum.ToString("N2")} руб.";
            
            if (company != null) 
            {

                customerTextBlock.Text = FormationStrings.GenerateCompanyData(company);
                sumStrTextBlock.Text = RusCurrency.Str((decimal)accOne.Sum);
                if(sumVat == 0)
                {
                    vatTB.Text = "НДС не облагается";
                }
                else
                {
                    vatTB.Text = $"В том числе НДС - {RusCurrency.Str((decimal)sumVat)}";
                }
            }
        }
    }
}
