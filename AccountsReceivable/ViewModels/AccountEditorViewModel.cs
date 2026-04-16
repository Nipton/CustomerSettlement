using AccountsReceivable.Data.Interfaces;
using AccountsReceivable.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsReceivable.ViewModels
{
    public class AccountEditorViewModel : ViewModelBase, ILoadable
    {
        private readonly IDataHub dataHub;
        private readonly IDialogService dialogService;
        public AccountEditorViewModel(IDataHub dataHub, IDialogService dialogService)
        {
            this.dataHub = dataHub;
            this.dialogService = dialogService;
        }

        public async Task LoadAsync()
        {
            
        }
    }
}
