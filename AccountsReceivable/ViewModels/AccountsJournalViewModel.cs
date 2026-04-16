using AccountsReceivable.Interfaces;
using AccountsReceivable.View;
using AccountsReceivable.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AccountsReceivable.ViewModels
{
    public class AccountsJournalViewModel : ViewModelBase
    {
        private readonly IDialogService dialogService;
        public ICommand OpenAccountEditorCommand { get; }
        public AccountsJournalViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;
            OpenAccountEditorCommand = new AsyncRelayCommand(OpenAccountEditor);
        }
        private async Task OpenAccountEditor()
        {
            var reuslt = await dialogService.ShowWindowAsync<AccountEditorView, AccountEditorViewModel>();
        }
    }
}
