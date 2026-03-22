using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AccountsReceivable.ViewModels.Factories
{
    public interface IDialogService
    {
        void ShowWindow<TView, TViewModel>(params object[] args) where TView : Window where TViewModel : ViewModelBase;
        void CloseWindow<TViewModel>(TViewModel viewModel) where TViewModel : ViewModelBase;
        void ShowError(string title, string message);
        void ShowInfo(string title, string message);
    }
}
