using AccountsReceivable.Models;
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
    /// <summary>
    /// Логика взаимодействия для AddCounterparty.xaml
    /// </summary>
    public partial class AddCounterparty : Window
    {
        private ContentControl creatingCounterparty;
        public AddCounterparty(ObservableCollection<Company> companiesList, Company? selectedCompany)
        {
            InitializeComponent();
            creatingCounterparty = new CreatingСounterparty(this, companiesList, selectedCompany);
            addCParty.Child = creatingCounterparty;
        }
        public AddCounterparty(Company? company)
        {
            InitializeComponent();
            creatingCounterparty = new CreatingСounterparty(this, company);
            addCParty.Child = creatingCounterparty;
        }
    }
}
