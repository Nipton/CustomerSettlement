using AccountsReceivable.Interfaces;
using AccountsReceivable.Models.Enums;
using AccountsReceivable.View;
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
        public async Task<bool> ShowWindowAsync(DialogType dialogType, params object[] args) 
        {
            Window window = dialogType switch
            {
                DialogType.CompanyEditor => factory.CreateWindow<CompanyEditorView, CompanyEditorViewModel>(args),
                DialogType.AccountEditor => factory.CreateWindow<AccountEditorView, AccountEditorViewModel>(args),
                DialogType.PaymentEditor => factory.CreateWindow<PaymentView, PaymentViewModel>(args),
                _ => throw new NotSupportedException($"Диалог {dialogType} не поддерживается. ")
            };
            if (window.DataContext is ILoadable loadable)
            {
                await loadable.LoadAsync();
            }
            if (window.DataContext is IDisposable disposable)
            {
                EventHandler handler = null!;
                handler = (_, _) =>
                {
                    disposable.Dispose();
                    window.Closed -= handler;
                };
                window.Closed += handler;
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
