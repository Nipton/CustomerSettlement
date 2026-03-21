using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AccountsReceivable.ViewModels.Factories
{
    public interface IViewModelFactory
    {
        T Create<T>() where T : ViewModelBase;
        T Create<T>(params object[] args) where T : ViewModelBase;
    }
}
