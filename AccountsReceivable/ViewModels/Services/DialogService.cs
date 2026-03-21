using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AccountsReceivable.ViewModels.Factories
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

        public void ShowWindow<TView, TViewModel>(params object[] args)
            where TView : Window
            where TViewModel : ViewModelBase
        {
            var vm = factory.Create<TViewModel>(args);
            var window = serviceProvider.GetRequiredService<TView>();

            window.DataContext = vm;
            window.ShowDialog();
        }
        public void ShowError(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OKCancel, MessageBoxImage.Error);
        }

        public void ShowInfo(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OKCancel, MessageBoxImage.Information);
        }
    }
}
