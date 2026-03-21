using AccountsReceivable.Interfaces;
using AccountsReceivable.View;
using AccountsReceivable.ViewModels.Factories;
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
        private ViewModelBase currentViewModel;
        private bool isDropdownOpen;       
        public ViewModelBase CurrentViewModel
        {
            get => currentViewModel;
            set => Set(ref currentViewModel, value);
        }
        public bool IsDropdownOpen
        {
            get => isDropdownOpen;
            set => Set(ref isDropdownOpen, value);
        }

        private readonly IViewModelFactory factory;
        public ICommand ShowOrganizationViewCommand { get; }
        public ICommand ShowReportViewCommand { get; }
        public ICommand ShowAccointsViewCommand { get; }
        public ICommand ShowArchiveViewCommand { get; }
        public ICommand ShowReconciliationViewCommand { get; }
        public ICommand ToggleReferencesCommand { get; }
        public ICommand ShowCounterpartyViewCommand { get; }
        public ICommand ShowContractViewCommand { get; }
        public ICommand ShowNomenclatureViewCommand { get; }

        public MainWindowViewModel(IViewModelFactory factory)
        {
            this.factory = factory;

            ShowOrganizationViewCommand = new RelayCommand(async _ => await ShowPageAsync<OrganizationViewModel>());
            ShowReportViewCommand = new RelayCommand(async _ => await ShowPageAsync<ReportViewModel>());
            //ShowAccointsViewCommand = new RelayCommand(_ => ShowPage<AccountsView>());
            //ShowArchiveViewCommand = new RelayCommand(_ => ShowPage<ArchiveAccountView>());
            //ShowReconciliationViewCommand = new RelayCommand(_ => ShowPage<ReconciliationReport>());
            //ToggleReferencesCommand = new RelayCommand(_ => IsDropdownOpen = true, _ => !IsDropdownOpen);
            //ShowCounterpartyViewCommand = new RelayCommand(_ => { ShowPage<Counterparties>(); IsDropdownOpen = false; });
            //ShowContractViewCommand = new RelayCommand(_ => { ShowPage<ContractData>(); IsDropdownOpen = false; });
            //ShowNomenclatureViewCommand = new RelayCommand(_ => { ShowPage<Nomenclature>(); IsDropdownOpen = false; });
        }
        private async Task ShowPageAsync<T>() where T : ViewModelBase
        {
            var vm = factory.Create<T>();

            CurrentViewModel = vm;

            if (vm is ILoadable loadable)
            {
                await loadable.LoadAsync();
            }
        }
    }
}
