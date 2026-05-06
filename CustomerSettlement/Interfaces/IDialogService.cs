using CustomerSettlement.Models.Enums;
using CustomerSettlement.ViewModels;
using System.Threading.Tasks;

namespace CustomerSettlement.Interfaces
{
    public interface IDialogService
    {
        Task<bool> ShowWindowAsync(DialogType dialogType, params object[] args);
        void CloseWindow<TViewModel>(TViewModel viewModel, bool? dialogResult) where TViewModel : ViewModelBase;
        void ShowError(string title, string message);
        void ShowInfo(string title, string message);
        bool ShowConfirmation(string title, string message);
    }
}
