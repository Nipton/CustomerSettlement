using AccountsReceivable.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace AccountsReceivable.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private object? currentView;
        private bool isDropdownOpen;
        private readonly Dictionary<Type, object> pages = new();
        public object? CurrentView
        {
            get => currentView;
            set => Set(ref currentView, value);
        }
        public bool IsDropdownOpen
        {
            get => isDropdownOpen;
            set => Set(ref isDropdownOpen, value);
        }
        public ICommand ShowOrganizationCommand { get; }
        public ICommand ShowReportViewCommand { get; }
        public ICommand ShowAccointsViewCommand { get; }
        public ICommand ShowArchiveCommand { get; }
        public ICommand ShowReconciliationCommand { get; }

        public MainWindowViewModel()
        {
            ShowOrganizationCommand = new RelayCommand(_ => ShowPage<Organization>());
            ShowReportViewCommand = new RelayCommand(_ => ShowPage<ReportView>());
            ShowAccointsViewCommand = new RelayCommand(_ => ShowPage<AccountsView>());
            ShowArchiveCommand = new RelayCommand(_ => ShowPage<ArchiveAccountView>());
            ShowReconciliationCommand = new RelayCommand(_ => ShowPage<ReconciliationReport>());

        }
        private void ShowPage<T>(bool reset = false) where T : new()
        {
            var type = typeof(T);
            if (CurrentView != null && CurrentView.GetType() == type && !reset)
                return;
            if (reset || !pages.TryGetValue(type, out var page))
            {
                page = new T();
                pages[type] = page;
            }
            CurrentView = page;
        }
    }
}
