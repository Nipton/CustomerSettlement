using AccountsReceivable.Data.Interfaces;
using AccountsReceivable.Helpers;
using AccountsReceivable.Interfaces;
using AccountsReceivable.Models;
using AccountsReceivable.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AccountsReceivable.ViewModels
{
    public class AccountEditorViewModel : ViewModelBase, ILoadable, IDataErrorInfo, IDisposable
    {
        #region Поля и свойства
        private readonly ICompanyRepository companyRepository;
        private readonly IContractRepository contractRepoitory;
        private readonly IDialogService dialogService;
        private readonly IAccountEditingService editingService;
        private AccountHeader workingAccHeader = new();
        private AccountHeader incomingAccHeader;
        private DateTime dateAccount = DateTime.Today;
        private DateTime period = DateTime.Today;
        private Company? selectedCompany;
        private Contract? selectedContract;
        private Nomenclature? selectedNomenclature;
        private AccountLine? selectedAccountLine;
        private decimal? quantity = 1;
        private decimal? price = 0;
        private decimal? vatRate = 0;  
        private decimal totalAccountSum;
        private decimal totalVatSum;
        private bool isEditPanelVisible;
        private bool isEditing;
        private bool isUpdating;
        private string headerAccountsLine = "Позиции счёта";
        public DateTime DateAccount { get => dateAccount; set => Set(ref dateAccount, value); }
        private List<Company> allCompanies = new();
        private List<Contract> allContracts = new();
        public List<Nomenclature> Nomenclatures { get; set; } = new();
        public ObservableCollection<Company> Companies { get; set; } = new();
        public ObservableCollection<Contract> Contracts { get; set; } = new();      
        public ObservableCollection<AccountLine> AccountLines { get; set; } = new();
        public Company? SelectedCompany { get => selectedCompany; set { if (Set(ref selectedCompany, value)) SetContractsAfterSelectCompany(); } }
        
        public Contract? SelectedContract { get => selectedContract; set { if (Set(ref selectedContract, value)) SetCompaniesAfterSelectContract();  } }
        public Nomenclature? SelectedNomenclature { get => selectedNomenclature; set => Set(ref selectedNomenclature, value); }
        public AccountLine? SelectedAccountLine { get => selectedAccountLine; set => Set(ref selectedAccountLine, value); }
        public string WindowTitle { get; set; } = string.Empty;
        public string HeaderAccountsLine { get => headerAccountsLine; set => Set(ref headerAccountsLine, value); }
        public decimal? Quantity
        {
            get => quantity; set
            {
                if (Set(ref quantity, value))
                { OnPropertyChanged(nameof(AmountWithVat)); OnPropertyChanged(nameof(VatSum)); OnPropertyChanged(nameof(AmountWithoutVat)); }
            }
        }
        public decimal? Price
        {
            get => price; set
            {
                if (Set(ref price, value))
                { OnPropertyChanged(nameof(AmountWithVat)); OnPropertyChanged(nameof(VatSum)); OnPropertyChanged(nameof(AmountWithoutVat)); }
            }
        }
        public decimal? VatRate
        {
            get => vatRate; set
            {
                if (Set(ref vatRate, value))
                { OnPropertyChanged(nameof(AmountWithVat)); OnPropertyChanged(nameof(VatSum)); }
            }
        }
        public decimal AmountWithVat => DecimalHelper.SafeAdd(AmountWithoutVat, VatSum);
        public decimal AmountWithoutVat => DecimalHelper.SafeMultiply(Quantity, Price);
        public decimal VatSum => DecimalHelper.SafeMultiply(AmountWithoutVat, (VatRate / 100));
        public DateTime Period { get => period; set => Set(ref period, value); }
        public decimal TotalAccountSum { get => totalAccountSum; set => Set(ref totalAccountSum, value); }
        public decimal TotalVatSum { get => totalVatSum; set => Set(ref totalVatSum, value); }
        public bool IsEditPanelVisible { get => isEditPanelVisible; set => Set(ref isEditPanelVisible, value); }
        #endregion
        public ICommand SaveNewLineCommand { get; }
        public ICommand AddNewLineCommand { get; }
        public ICommand FinishEditLineCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand EditAccountLineCommand { get; }
        public ICommand RemoveAccountLineCommand { get; }
        public ICommand SaveAllCommand { get; }
        public ICommand ClearSelectionCommand { get; }
        
        public AccountEditorViewModel(AccountHeader accountHeader, ICompanyRepository companyRepository, IContractRepository contractRepoitory, IAccountEditingService editingService, IDialogService dialogService)
        {
            this.companyRepository = companyRepository;
            this.contractRepoitory = contractRepoitory;
            this.dialogService = dialogService;
            this.editingService = editingService;
            incomingAccHeader = accountHeader;
            
            SaveNewLineCommand = new RelayCommand(_ => SaveAccountLine(), _ =>  IsValid && SelectedNomenclature != null);
            CancelCommand = new RelayCommand(_ => CancelWindow());
            AddNewLineCommand = new RelayCommand(_ => AddNewAccountLine());
            FinishEditLineCommand = new RelayCommand(_ => FinishEditLine());
            EditAccountLineCommand = new RelayCommand(_ => EditAccountLine(), _ => SelectedAccountLine != null);
            RemoveAccountLineCommand = new RelayCommand(_ => RemoveAccountLine(), _ => SelectedAccountLine != null);
            SaveAllCommand = new AsyncRelayCommand(SaveAccountHeaderAsync, _ => !IsEditPanelVisible && SelectedContract != null && SelectedCompany != null);
            ClearSelectionCommand = new RelayCommand(_ => ClearHeaderSelection());
        }
        public async Task LoadAsync()
        {
            try
            {
                allCompanies = (await companyRepository.GetAllCompaniesAsync()).ToList();
                allContracts = (await contractRepoitory.GetAllAsync()).ToList();
                foreach (var company in allCompanies)
                    Companies.Add(company);
                foreach (var contract in allContracts)
                    Contracts.Add(contract);
                Nomenclatures = (await editingService.GetNomenclaturesAsync()).ToList();
                await InitializeAsync();
            }
            catch (Exception)
            {
                dialogService.ShowError("Ошибка!", "Не удалось загрузить данные.");
            }
        }
        private async Task InitializeAsync()
        {
            if (incomingAccHeader.Id == 0)
            {
                WindowTitle = "Создание счёта";
                return;
            }
            WindowTitle = "Редактирование счёта";
            var loadedAccHeader = await editingService.GetAccountAsync(incomingAccHeader.Id);
            if (loadedAccHeader == null)
            {
                dialogService.ShowError("Ошибка!", "Не удалось загрузить данные счёта.");
                return;
            }
            else
                workingAccHeader = loadedAccHeader;
            SelectedCompany = allCompanies.FirstOrDefault(x => x.Id == workingAccHeader.CompanyId);
            SelectedContract = allContracts.FirstOrDefault(x => x.Id == workingAccHeader.ContractId);
            DateAccount = workingAccHeader.Date;
            foreach (var line in workingAccHeader.AccountsList)
                AccountLines.Add(line);
            CalculateTotalSum();
        }
        private void SetCompaniesAfterSelectContract()
        {
            if (isUpdating) return;
            isUpdating = true;
            try
            {
                var filtred = allCompanies.Where(x => SelectedContract?.CompanyId == x.Id).ToList();
                Company? oldSelectedCompany = SelectedCompany?.Id == SelectedContract?.CompanyId ? SelectedCompany : null;
                Companies.Clear();
                foreach (var company in filtred)
                    Companies.Add(company);
                SelectedCompany = oldSelectedCompany;
            }
            finally { isUpdating = false; }
        }
        private void SetContractsAfterSelectCompany()
        {
            if (isUpdating) return;
            isUpdating = true;
            try
            {
                var filtred = allContracts.Where(x => SelectedCompany?.Id == x.CompanyId).ToList();
                Contract? oldSelectedContract = SelectedCompany?.Id == SelectedContract?.CompanyId ? SelectedContract : null;
                Contracts.Clear();
                foreach (var contract in filtred)
                    Contracts.Add(contract);
                SelectedContract = oldSelectedContract;
            }
            finally { isUpdating = false; }
        }
        private void ClearHeaderSelection()
        {
            if (isUpdating) return;
            isUpdating = true;
            try
            {
                SelectedCompany = null;
                SelectedContract = null;
                Companies.Clear();
                Contracts.Clear();
                foreach (var company in allCompanies)
                    Companies.Add(company);
                foreach (var contract in allContracts)
                    Contracts.Add(contract);
                DateAccount = DateTime.Today;
            }
            finally { isUpdating = false; }
        }
        public async Task SaveAccountHeaderAsync()
        {
            if (SelectedContract == null || SelectedCompany == null)
            {
                dialogService.ShowInfo("", "Для сохранения необходимо выбрать компанию и контракт.");
                return;
            }
            workingAccHeader.AccountsList = AccountLines;
            workingAccHeader.CompanyId = SelectedCompany.Id;
            workingAccHeader.ContractId = SelectedContract.Id;
            workingAccHeader.Date = DateAccount;
            workingAccHeader.Sum = TotalAccountSum;
            if (incomingAccHeader.Id != 0)
               workingAccHeader.PaymentStatus = workingAccHeader.Sum > 0 && workingAccHeader.Sum <= workingAccHeader.PaymentSum;
            
            await editingService.SaveAccountsAsync(workingAccHeader);

            incomingAccHeader.Id = workingAccHeader.Id;
            incomingAccHeader.Date = workingAccHeader.Date;
            incomingAccHeader.Company = SelectedCompany;
            incomingAccHeader.Contract = SelectedContract;
            incomingAccHeader.Sum = workingAccHeader.Sum;
            incomingAccHeader.CompanyId = SelectedCompany.Id;
            incomingAccHeader.ContractId = SelectedContract.Id;
            incomingAccHeader.AccountsList = workingAccHeader.AccountsList;
            incomingAccHeader.PaymentStatus = workingAccHeader.PaymentStatus;

            dialogService.CloseWindow(this, true);
        }       
        private void AddNewAccountLine()
        {
            IsEditPanelVisible = true;
            isEditing = false;
            HeaderAccountsLine = "Добавление позиции счёта";
        }
        private void EditAccountLine()
        {
            if (SelectedAccountLine == null)
            {
                dialogService.ShowInfo("Внимание!", "Для редактирования необходимо выделить один элемент.");
                return;
            }
            isEditing = true;
            IsEditPanelVisible = true;
            HeaderAccountsLine = "Редактирование позиции счёта";
            SelectedNomenclature = SelectedAccountLine.Nomenclature;
            Quantity = SelectedAccountLine.Quantity;
            Price = SelectedAccountLine.Price;
            VatRate = SelectedAccountLine.VatRate;
            Period = SelectedAccountLine.Period;
        }        
        private void SaveAccountLine()
        {
            if (SelectedNomenclature == null || !IsValid)
            {
                dialogService.ShowInfo("", "Для добавления заполните все необходимые поля.");
                return;
            }
            AccountLine line;
            if (isEditing && SelectedAccountLine != null)
            {
                line = SelectedAccountLine;
                MapLineFromForm(line);
                var index = AccountLines.IndexOf(SelectedAccountLine);
                AccountLines.RemoveAt(index);
                AccountLines.Insert(index, line);
            }
            else
            {
                line = new AccountLine();
                MapLineFromForm(line);
                AccountLines.Add(line);
            }                    
            FinishEditLine();
        }
        private void RemoveAccountLine()
        {
            if (SelectedAccountLine == null)
            {
                dialogService.ShowInfo("Внимание!", "Для редактирования необходимо выделить один элемент.");
                return;
            }
            AccountLines.Remove(SelectedAccountLine);
            CalculateTotalSum();
        }
        private void MapLineFromForm(AccountLine line)
        {
            line.NomenclatureId = SelectedNomenclature!.Id;
            line.Nomenclature = SelectedNomenclature;
            line.Period = Period;
            line.AmountWithVat = AmountWithVat;
            if (!Quantity.HasValue || !Price.HasValue || !VatRate.HasValue)
                throw new ArgumentException("Ошибка значений");
            line.Quantity = Quantity.Value;
            line.Price = Price.Value;
            line.VatRate = VatRate.Value;
        }
        private void FinishEditLine()
        {
            IsEditPanelVisible = false;
            isEditing = false;
            HeaderAccountsLine = "Позиции счёта";
            SelectedAccountLine = null;
            CalculateTotalSum();
            ClearAccountLineForm();
        }
        private void CalculateTotalSum()
        {
            try
            {
                TotalAccountSum = AccountLines.Sum(x => x.AmountWithVat);
                TotalVatSum = AccountLines.Sum(x => x.Quantity * x.Price * x.VatRate / 100);
            }
            catch (OverflowException)
            {
                dialogService.ShowError("Внимание!", "Критическая ошибка. Введенные значения слишком большие.");
                TotalAccountSum = 0;
                TotalVatSum = 0;
            }
        }        
        private void ClearAccountLineForm()
        {
            SelectedNomenclature = null;
            Period = DateTime.Today;
            Price = 0;
            VatRate = 0;
            Quantity = 1;
        }
        private void CancelWindow()
        {
            if(dialogService.ShowConfirmation("Подтверждние", "Вы уверены, что хотите отменить все изменения?"))
                dialogService.CloseWindow(this, false);
        }
        public void Dispose()
        {
            editingService.Dispose();
        }
        #region Валидация
        public string Error => null!;
        private Dictionary<string, string?> errorCollection = new ();
        public bool IsValid => errorCollection.Values.All(x => x == null);
        #nullable disable
        public string this[string columnName]
        {
            get
            {
                string error = columnName switch
                {
                    nameof(Quantity) => ValidateDecimal(Quantity, columnName),
                    nameof(Price) => ValidateDecimal(Price, columnName),
                    nameof(VatRate) => ValidateDecimal(VatRate, columnName),
                    _ => null
                };                
                errorCollection[columnName] = error;
                OnPropertyChanged(nameof(IsValid));
                return error;
            }
        }
        #nullable restore
        public string? ValidateDecimal(decimal? value, string fieldName)
        {
            if (value != null)
            {
                if (value < 0)
                    return "Значение не может быть отрицательным";
                if (fieldName == nameof(Quantity) && Price > 0 && value > decimal.MaxValue / Price)
                    return "Слишком большое значение";
                if (fieldName == nameof(Price) && Quantity > 0 && value > decimal.MaxValue / Quantity)
                    return "Слишком большое значение";
                if(fieldName == nameof(VatRate) && value> 120)
                    return "Ставка НДС не может превышать 120%";
                return null;
            }
            return "Необходимо ввести число";
        }
        #endregion
    }
}
