using CustomerSettlement.Data.Interfaces;
using CustomerSettlement.Exceptions;
using CustomerSettlement.Interfaces;
using CustomerSettlement.Models;
using CustomerSettlement.Models.Enums;
using CustomerSettlement.ViewModels.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CustomerSettlement.ViewModels
{
    public class ContractViewModel : ViewModelBase, ILoadable
    {
        private readonly INomenclatureRepository nomenclatureRepository;
        private readonly ICompanyRepository companyRepository;
        private IContractRepository contractRepository;
        private readonly IDialogService dialogService;
        private bool isFirstLoad = true;
        private ContractSubject? selectedContractSubject;
        private Company? selectedCompany;
        public IList SelectedContracts { get; set; } = new List<Contract>();
        private DateTime date = DateTime.Today;
        private string number = string.Empty;
        public ContractSubject? SelectedContractSubject
        {
            get => selectedContractSubject;
            set => Set(ref selectedContractSubject, value);
        }
        public Company? SelectedCompany
        {
            get => selectedCompany;
            set => Set(ref selectedCompany, value);
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
        public List<ContractSubject> Subjects => Enum.GetValues(typeof(ContractSubject)).Cast<ContractSubject>().OrderByDescending(x => x).ToList();
        public ObservableCollection<Company> Companies { get; set; } = new();
        public ICommand AddContractCommand { get; }
        public ICommand RemoveContractCommand { get; }
        public ICommand RefreshDataCommand { get; }
        public ContractViewModel(IContractRepository contractRepository, INomenclatureRepository nomenclatureRepository, ICompanyRepository companyRepository, IDialogService dialogService) 
        {
            this.contractRepository = contractRepository;
            this.nomenclatureRepository = nomenclatureRepository;
            this.companyRepository = companyRepository;
            this.dialogService = dialogService;
            companyRepository.DataChanged += LoadCompaniesAsync;
            AddContractCommand = new AsyncRelayCommand(AddContractAsync, _ => IsInputDataValid());
            RemoveContractCommand = new AsyncRelayCommand(RemoveContractAsync, _ => SelectedContracts.Count != 0);
            RefreshDataCommand = new AsyncRelayCommand(async _ => { await LoadCompaniesAsync(); await LoadContractsAsync(); });
        }
        public async Task LoadAsync()
        {
            if (isFirstLoad)
            {
                await LoadCompaniesAsync();
                await LoadContractsAsync();
                isFirstLoad = false;
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
                Contract contract = new Contract() { Number = Number, CompanyId = SelectedCompany!.Id, ContractSubject = SelectedContractSubject ?? ContractSubject.Other, Date = Date };
                await contractRepository.AddAsync(contract);
                contract.Company = SelectedCompany;
                Contracts.Add(contract);
                SelectedContractSubject = null;
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
                var selectet = SelectedContracts.Cast<Contract>().ToList();
                await contractRepository.DeleteAsync(selectet);
                foreach (var contract in selectet)
                {
                    Contracts.Remove(contract);                   
                }
            }
            catch (DeleteRestrictedException ex)
            {
                dialogService.ShowError("Ошибка!", ex.Message);
            }
            catch
            {
                dialogService.ShowError("Ошибка!", "Не удалось удалить объекты.");
            }
        }
        private bool IsInputDataValid()
        {
            return !string.IsNullOrWhiteSpace(Number) && SelectedCompany != null && SelectedContractSubject != null;
        }
    }
}
