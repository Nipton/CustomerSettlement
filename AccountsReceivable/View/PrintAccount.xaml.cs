using AccountsReceivable.Models;
using AccountsReceivable.Services;
using RSDN;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace AccountsReceivable.View
{
    public partial class PrintAccount : Window
    {
        private AccountPartOne accOne;
        private List<AccountPartTwo> accTwo;
        private FileIOService fileIOService;
        private ObservableCollection<CompanyOld>? companiesList;
        private CompanyOld organization;
        public PrintAccount(AccountPartOne selectedAccOne)
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
            }
            this.Close();
        }

        private void PrintAccount_Loaded(object sender, RoutedEventArgs e)
        {
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

            innTextBlock.Text = $"ИНН {organization.Inn}";
            kppTextBlock.Text = $"КПП {organization.Kpp}";
            rsTextBlock.Text = organization.Rs;
            ksTextBlock.Text = organization.Ks;
            recipientTextBlock.Text = $"Получатель: {organization.ShortName}";
            bankTextBlock.Text = $"Банк получателя: {organization.Bank}";
            bikTextBlock.Text = organization.Bik;
            recipient2TextBlock.Text = FormationStrings.GenerateCompanyData(organization);
            foreach (var item in accTwo)
            {
                if (item.Period != null)
                {
                    item.Nomenclature = $"{item.Nomenclature} за {item.Period:Y}";
                }
            }
            dataGrid.ItemsSource = accTwo;
            numperTextBlock.Text = accOne.ID.ToString();
            dateTextBlock.Text = accOne.Date.ToString("d");
            totalTextBlock.Text = accOne.Sum.ToString("N2");
            HeadCompanyTextBlock.Text = $"Руководитель предприятия: ____________ {FormationStrings.GenerateFIO(organization)}";
            ChiefAccountantTextBlock.Text = $"Главный бухгалтер: ____________ {FormationStrings.GenerateFIO(organization)}";
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
            quantityTextBlock.Text = $"Всего наименований {accTwo.Count}, на сумму {accOne.Sum.ToString("N2")} руб.";
            var company = companiesList?.FirstOrDefault(x=> x.Name == accOne.Company);
            if (company != null) 
            {

                customerTextBlock.Text = FormationStrings.GenerateCompanyData(company);
                sumStrTextBlock.Text = RusCurrency.Str(accOne.Sum);
                if(sumVat == 0)
                {
                    vatTB.Text = "НДС не облагается";
                }
                else
                {
                    vatTB.Text = $"В том числе НДС - {RusCurrency.Str(sumVat)}";
                }
            }
        }
    }

    public static class FormationStrings
    {
        public static string GenerateCompanyData(CompanyOld company)
        {
            string counterparty = "";
            if (company != null)
            {
                counterparty = $"{company.ShortName}";
                if (!string.IsNullOrEmpty(company.Inn))
                {
                    counterparty += $", ИНН {company.Inn}";
                }
                if (!string.IsNullOrEmpty(company.Kpp))
                {
                    counterparty += $", КПП {company.Kpp}";
                }
                if (!string.IsNullOrEmpty(company.LegalAddress))
                {
                    counterparty += $", {company.LegalAddress}";
                }
                if (!string.IsNullOrEmpty(company.Phone))
                {
                    counterparty += $", тел: {company.Phone}";
                }
            }
            return counterparty;
        }
        public static string GenerateFIO(CompanyOld company)
        {
            string fio = "";
            if (company != null)
            {
                string[] fioArr = company.DirectorFullName.Split(' ');
                if (fioArr.Length == 3)
                {
                    fio = fioArr[1][0] + "." + fioArr[2][0] + ". " + fioArr[0];
                }
                if (fioArr.Length == 2)
                {
                    fio = fioArr[1][0] + ". " + fioArr[0];
                }
                if (fioArr.Length == 1)
                {
                    fio = fioArr[0];
                }
            }
            return fio;
        }
    }
    public class IndexIncrement : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
