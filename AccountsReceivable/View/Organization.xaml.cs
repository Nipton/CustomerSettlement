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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AccountsReceivable.View
{
    public partial class Organization : UserControl
    {
        private Company ourOrganization;
        private FileIOService fileIOService;
        public Organization()
        {
            InitializeComponent();
            ourOrganization = new Company();
            fileIOService= new FileIOService();
            ourOrganization.ID = -1;
        }

        private void EditButton(object sender, RoutedEventArgs e)
        {
            AddCounterparty addCounterparty = new AddCounterparty(ourOrganization);
            addCounterparty.Title = "Организация";
            addCounterparty.ShowDialog();
            Load();
        }

        private void Organization_Loaded(object sender, RoutedEventArgs e)
        {
            Load();
        }
        private void Load()
        {
            try
            {
                var tempOrg = fileIOService.LoadOrganization();
                if (tempOrg.ID != 0)
                {
                    ourOrganization = tempOrg;
                }
                DataContext = ourOrganization;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Shutdown();
            }
        }
    }
}
