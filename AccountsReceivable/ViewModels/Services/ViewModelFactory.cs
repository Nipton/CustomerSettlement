using AccountsReceivable.Models;
using AccountsReceivable.View;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsReceivable.ViewModels.Factories
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
    }
}
