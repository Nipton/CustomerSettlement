using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AccountsReceivable.ViewModels.Services
{
    public interface IDialogService
    {
        bool ShowWindow<TView, TViewModel>(params object[] args) where TView : Window where TViewModel : ViewModelBase;
        void CloseWindow<TViewModel>(TViewModel viewModel, bool? dialogResult) where TViewModel : ViewModelBase;
        void ShowError(string title, string message);
        void ShowInfo(string title, string message);
        bool ShowConfirmation(string title, string message);
    }
}
