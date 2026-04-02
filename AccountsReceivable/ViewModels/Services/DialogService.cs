using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AccountsReceivable.ViewModels.Services
{
    public class DialogService : IDialogService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IViewModelFactory factory;

        public DialogService(IServiceProvider serviceProvider, IViewModelFactory factory)
        {
            this.serviceProvider = serviceProvider;
            this.factory = factory;
        }

        public bool ShowWindow<TView, TViewModel>(params object[] args)
            where TView : Window
            where TViewModel : ViewModelBase
        {
            var vm = factory.Create<TViewModel>(args);
            var window = serviceProvider.GetRequiredService<TView>();

            window.DataContext = vm;
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
