using AccountsReceivable.Data.Interfaces;
using AccountsReceivable.Interfaces;
using AccountsReceivable.Models;
using AccountsReceivable.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AccountsReceivable.ViewModels
{
    public class ContractViewModel : ViewModelBase, ILoadable
    {
        private readonly INomenclatureRepository nomenclatureRepository;
        private readonly ICompanyRepository companyRepository;
        private IRepository<Contract> contractRepository;
        private readonly IDialogService dialogService;
        private bool isFirstLoad = true;
        private Nomenclature? selectedNomenclature;
        private Company? selectedCompany;
        private List<Contract> selectedContracts = new();
        private DateTime date = DateTime.Today;
        private string number = string.Empty;
        public Nomenclature? SelectedNomenclature
        {
            get => selectedNomenclature;
            set => Set(ref selectedNomenclature, value);
        }
        public Company? SelectedCompany
        {
            get => selectedCompany;
            set => Set(ref selectedCompany, value);
        }
        public List<Contract> SelectedContracts
        {
            get => selectedContracts;
            set => Set(ref selectedContracts, value);
        }
        public DateTime Date 
        {
            get => date;
            set => Set(ref date, value);
        }
        public string Number
        {
            get => number;
            set => Set(ref number, value);
        }
        public ObservableCollection<Contract> Contracts { get; set; } = new();
        public ObservableCollection<Nomenclature> Nomenclatures { get; set; } = new();
        public ObservableCollection<Company> Companies { get; set; } = new();
        public ICommand AddContractCommand { get; }
        public ICommand RemoveContractCommand { get; }
        public ICommand RefreshDataCommand { get; }
        public ContractViewModel(IRepository<Contract> contractRepository, INomenclatureRepository nomenclatureRepository, ICompanyRepository companyRepository, IDialogService dialogService) 
        {
            this.contractRepository = contractRepository;
            this.nomenclatureRepository = nomenclatureRepository;
            this.companyRepository = companyRepository;
            this.dialogService = dialogService;
            nomenclatureRepository.DataChanged += LoadNomenclaturesAsync;
            companyRepository.DataChanged += LoadCompaniesAsync;
            AddContractCommand = new AsyncRelayCommand(AddContractAsync, _ => IsInputDataValid());
            RemoveContractCommand = new AsyncRelayCommand(RemoveContractAsync, _ => SelectedContracts.Count != 0);
            RefreshDataCommand = new AsyncRelayCommand(async _ => {await LoadNomenclaturesAsync(); await LoadCompaniesAsync(); await LoadContractsAsync(); });
        }
        public async Task LoadAsync()
        {
            if (isFirstLoad)
            {
                await LoadNomenclaturesAsync();
                await LoadCompaniesAsync();
                await LoadContractsAsync();
                isFirstLoad = false;
            }
        }
        private async Task LoadNomenclaturesAsync()
        {
            try
            {
                Nomenclatures.Clear();
                var list = await nomenclatureRepository.GetAllAsync();
                foreach ( var nomenclature in list )
                {
                    Nomenclatures.Add(nomenclature);
                }
            }
            catch (Exception)
            {
                dialogService.ShowInfo("Внимание!", "Не удалось загрузить список услуг.");
            }
        }
        private async Task LoadCompaniesAsync()
        {
            try
            {
                Companies.Clear();
                var list = await companyRepository.GetAllCompaniesAsync();
                foreach (var company in list)
                {
                    Companies.Add(company);
                }
            }
            catch (Exception)
            {
                dialogService.ShowInfo("Внимание!", "Не удалось загрузить список контрагентов.");
            }
        }
        private async Task LoadContractsAsync()
        {
            try
            {
                Contracts.Clear();
                var list = await contractRepository.GetAllAsync();
                foreach (var contract in list)
                {
                    Contracts.Add(contract);
                }
            }
            catch(Exception)
            {
                dialogService.ShowInfo("Внимание!", "Не удалось загрузить список контрактов.");
            }
        }
        private async Task AddContractAsync()
        {
            if (!IsInputDataValid())
            {
                dialogService.ShowInfo("", "Заполните поля для сохранения");
                return;
            }
            try
            {
                Contract contract = new Contract() { Number = Number, CompanyId = SelectedCompany!.Id, NomenclatureId = SelectedNomenclature!.Id, Date = DateOnly.FromDateTime(Date) };
                await contractRepository.AddAsync(contract);
                contract.Company = SelectedCompany;
                contract.Nomenclature = SelectedNomenclature;
                Contracts.Add(contract);
                SelectedNomenclature = null;
                SelectedCompany = null;
                Number = string.Empty;
            }
            catch
            {
                dialogService.ShowError("Ошибка!", "Не удалось сохранить контракт.");
            }
        }
        private async Task RemoveContractAsync()
        {
            if (SelectedContracts.Count == 0)
            {
                dialogService.ShowInfo("Внимание!", "Для удаления необходимо выделить хотя бы один элемент.");
                return;
            }
            if (!dialogService.ShowConfirmation("Внимание!", $"Вы действительно хотите удалить выделенные элементы в количестве {SelectedContracts.Count} шт.? Отменить действие будет невозможно."))
                return;
            try
            {
                await contractRepository.DeleteAsync(SelectedContracts);
                foreach (var contract in SelectedContracts)
                {
                    Contracts.Remove(contract);                   
                }
                SelectedContracts.Clear();
            }
            catch
            {
                dialogService.ShowError("Ошибка!", "Не удалось удалить объекты.");
            }
        }
        private bool IsInputDataValid()
        {
            return !string.IsNullOrWhiteSpace(Number) && SelectedCompany != null && SelectedNomenclature != null;
        }
    }
}
