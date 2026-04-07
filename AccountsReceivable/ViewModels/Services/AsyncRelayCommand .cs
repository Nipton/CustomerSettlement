using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AccountsReceivable.ViewModels.Services
{
    public class AsyncRelayCommand : ICommand
    {
        private readonly Func<Task>? executeNoParam;
        private readonly Func<object?, Task>? executeWithParam;
        private readonly Predicate<object?>? canExecute;
        private bool isExecuting;

        public AsyncRelayCommand(Func<object?, Task> execute, Predicate<object?>? canExecute = null)
        {
            this.executeWithParam = execute;
            this.canExecute = canExecute;
        }
        public AsyncRelayCommand(Func<Task> execute, Predicate<object?>? canExecute = null)
        {
            executeNoParam = execute;
            this.canExecute = canExecute;
        }
        public bool CanExecute(object? parameter)
        {
            return !isExecuting && (canExecute?.Invoke(parameter) ?? true);
        }

        public async void Execute(object? parameter)
        {
            isExecuting = true;
            RaiseCanExecuteChanged();

            try
            {
                if (executeWithParam != null)
                    await executeWithParam(parameter);
                else
                    await executeNoParam!();
            }
            finally
            {
                isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        public event EventHandler? CanExecuteChanged  
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested(); 
        }
    }
}
