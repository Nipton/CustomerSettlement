using AccountsReceivable.Interfaces;
using AccountsReceivable.Models.Enums;
using AccountsReceivable.ViewModels.Commands;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AccountsReceivable.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase? currentViewModel;
        private bool isDropdownOpen;
        private PageType currentPageType = PageType.None;
        public ViewModelBase? CurrentViewModel
        {
            get => currentViewModel;
            set => Set(ref currentViewModel, value);
        }
        public PageType CurrentPageType 
        {
            get => currentPageType;
            set => Set(ref currentPageType, value);
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
        public ICommand ShowReconciliationViewCommand { get; }
        public ICommand ToggleReferencesCommand { get; }
        public ICommand ShowCounterpartyViewCommand { get; }
        public ICommand ShowContractViewCommand { get; }
        public ICommand ShowNomenclatureViewCommand { get; }

        public MainWindowViewModel(IViewModelFactory factory)
        {
            this.factory = factory;

            ShowOrganizationViewCommand = new AsyncRelayCommand(_ => ShowPageAsync<OrganizationViewModel>(PageType.Organization));
            ShowReportViewCommand = new AsyncRelayCommand(_ => ShowPageAsync<ReportViewModel>(PageType.Report));
            ShowAccointsViewCommand = new AsyncRelayCommand(_ => ShowPageAsync<AccountsJournalViewModel>(PageType.Accounts));
            ShowReconciliationViewCommand = new AsyncRelayCommand(_ => ShowPageAsync<ReconciliationReportViewModel>(PageType.Reconciliation));
            ToggleReferencesCommand = new RelayCommand(_ => IsDropdownOpen = true, _ => !IsDropdownOpen);
            ShowCounterpartyViewCommand = new AsyncRelayCommand(async _ => { await ShowPageAsync<CounterpartiesViewModel>(PageType.Counterparties); IsDropdownOpen = false; });
            ShowContractViewCommand = new AsyncRelayCommand(async _ => { await ShowPageAsync<ContractViewModel>(PageType.Contract); IsDropdownOpen = false; });
            ShowNomenclatureViewCommand = new AsyncRelayCommand(async _ => { await ShowPageAsync<NomenclatureViewModel>(PageType.Nomenclature); IsDropdownOpen = false; });
        }
        private async Task ShowPageAsync<T>(PageType pageType) where T : ViewModelBase
        {
            var vm = factory.Create<T>();

            CurrentViewModel = vm;
            CurrentPageType = pageType;

            if (vm is ILoadable loadable)
            {
                await loadable.LoadAsync();
            }
        }
    }
}
