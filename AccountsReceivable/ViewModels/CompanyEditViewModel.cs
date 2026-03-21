using AccountsReceivable.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsReceivable.ViewModels
{
    public class CompanyEditViewModel : ViewModelBase
    {
        private Company company;
        public CompanyEditViewModel(Company company) 
        { 
            this.company = company;
        }
    }
}
