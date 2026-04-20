using AccountsReceivable.Data.Interfaces;
using AccountsReceivable.Interfaces;
using AccountsReceivable.Models;
using AccountsReceivable.Models.Enums;
using AccountsReceivable.ViewModels.Commands;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AccountsReceivable.ViewModels
{
    public class AccountsJournalViewModel : ViewModelBase
    {
        private readonly IAccountRepository accountRepository;
        private readonly IDialogService dialogService;
        public ICommand OpenAccountEditorCommand { get; }
        public AccountsJournalViewModel(IDialogService dialogService, IAccountRepository accountRepository)
        {
            this.dialogService = dialogService;
            this.accountRepository = accountRepository;
            OpenAccountEditorCommand = new AsyncRelayCommand(OpenAccountEditor);
        }
        private async Task OpenAccountEditor()
        {
            var reuslt = await dialogService.ShowWindowAsync(DialogType.AccountEditor, new AccountHeader() { Id = 5});
        }
    }
}
