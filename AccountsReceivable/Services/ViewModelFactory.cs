using AccountsReceivable.Interfaces;
using AccountsReceivable.Models;
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
    public class ViewModelFactory : IViewModelFactory
    {
        private readonly IServiceProvider serviceProvider;
        public ViewModelFactory(IServiceProvider serviceProvider)
        { 
            this.serviceProvider = serviceProvider;
        }

        public T Create<T>() where T : ViewModelBase
        {
            return serviceProvider.GetRequiredService<T>();
        }
        public T Create<T>(params object[] args) where T : ViewModelBase
        {
            return ActivatorUtilities.CreateInstance<T>(serviceProvider, args);
        }
        public TView CreateWindow<TView, TViewModel>(params object[] args) where TView : Window where TViewModel : ViewModelBase
        {
            var vm = Create<TViewModel>(args);
            var window = serviceProvider.GetRequiredService<TView>();
            window.DataContext = vm;
            return window;
        }
    }
}
