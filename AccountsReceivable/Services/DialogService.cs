using AccountsReceivable.Interfaces;
using AccountsReceivable.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AccountsReceivable.Services
{
    public class DialogService : IDialogService
    {
        private readonly IViewModelFactory factory;

        public DialogService(IViewModelFactory factory)
        {
            this.factory = factory;
        }

        public async Task<bool> ShowWindowAsync<TView, TViewModel>(params object[] args) where TView : Window where TViewModel : ViewModelBase
        {
            var window = factory.CreateWindow<TView, TViewModel>(args);
            if (window.DataContext is ILoadable loadable)
            {
                await loadable.LoadAsync();
            }
            return window.ShowDialog() == true;
        }
        public void CloseWindow<TViewModel>(TViewModel viewModel, bool? dialogResult = null) where TViewModel : ViewModelBase
        {
            var window = Application.Current.Windows.Cast<Window>().FirstOrDefault(w => w.DataContext == viewModel);
            if (window != null)
            {
                if(dialogResult.HasValue)
                    window.DialogResult = dialogResult.Value;
                window.Close();
            }
        }
        public void ShowError(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowInfo(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public bool ShowConfirmation(string title, string message)
        {
            return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }
    }
}
