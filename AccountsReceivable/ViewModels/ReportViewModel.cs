using AccountsReceivable.Data.Interfaces;
using AccountsReceivable.Helpers;
using AccountsReceivable.Interfaces;
using AccountsReceivable.Models;
using AccountsReceivable.Models.Enums;
using AccountsReceivable.ViewModels.Commands;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AccountsReceivable.ViewModels
{
    public class ReportViewModel : ViewModelBase, ILoadable
    {
        private readonly INomenclatureRepository nomenclatureRepository;
        private readonly IAccountRepository accountRepository;
        private readonly IDialogService dialogService;
        private DateTime? fromDate;
        private DateTime? toDate;
        private string serviceName = string.Empty;
        private decimal? volume;
        private string unit = string.Empty;
        private decimal? totalSum;
        private string periodDisplay = string.Empty;
        private string categoryCompany = string.Empty;
        private bool isLoaded;
        private Nomenclature? selectedNomenclature;
        private SelectableCategory? selectedCategory;
        private ObservableCollection<Nomenclature> nomenclatures = new();
        private ObservableCollection<SelectableCategory> categories = new();
        public DateTime? FromDate { get => fromDate; set => Set(ref fromDate, value); }
        public DateTime? ToDate { get => toDate; set => Set(ref toDate, value); }
        public string ServiceName { get => serviceName; set => Set(ref serviceName, value); }
        public decimal? Volume { get => volume; set => Set(ref volume, value); }
        public string Unit { get => unit; set => Set(ref unit, value); }
        public string PeriodDisplay { get => periodDisplay; set => Set(ref periodDisplay, value); }
        public string CategoryCompany { get => categoryCompany; set => Set(ref categoryCompany, value); }
        public decimal? TotalSum { get => totalSum; set => Set(ref totalSum, value); }
        public ObservableCollection<Nomenclature> Nomenclatures { get => nomenclatures; set => Set(ref nomenclatures, value); }
        public ObservableCollection<SelectableCategory> Categories { get => categories; set => Set(ref categories, value); }
        public Nomenclature? SelectedNomenclature { get => selectedNomenclature; set => Set(ref selectedNomenclature, value); }
        public SelectableCategory? SelectedCategory { get => selectedCategory; set => Set(ref selectedCategory, value); }
        
        public ICommand ResetFormCommand {  get; }
        public ICommand CalculateCommand { get; }
        

        public ReportViewModel(IDialogService dialogService, INomenclatureRepository nomenclatureRepository, IAccountRepository accountRepository)
        {
            this.dialogService = dialogService;
            this.nomenclatureRepository = nomenclatureRepository;
            this.accountRepository = accountRepository;
            nomenclatureRepository.DataChanged += LoadNomenclaturesAsync;
            ResetFormCommand = new RelayCommand(_ => ClearForm());
            CalculateCommand = new AsyncRelayCommand(CalculateReport, _ => CanCalculate());
        }
        public async Task LoadAsync()
        {
            if (isLoaded) return;
            await LoadNomenclaturesAsync();
            LoadCategory();
            isLoaded = true;
        }
        private async Task LoadNomenclaturesAsync()
        {
            var nomenclatures = await nomenclatureRepository.GetAllAsync();
            Nomenclatures = new ObservableCollection<Nomenclature>(nomenclatures){ new Nomenclature { Name = "Все", Unit = "", Id = -1 } };
        }
        private void LoadCategory()
        {
            Categories.Clear();
            foreach (CompanyCategory category in Enum.GetValues(typeof(CompanyCategory)))
            {
                Categories.Add(new SelectableCategory { Id = (int)category, Name = category.GetDisplayName() });
            }
            Categories.Add(new SelectableCategory { Id = -1, Name = "Все" });
        }

        private async Task CalculateReport()
        {
            if (!CanCalculate())
            {
                dialogService.ShowInfo("Создать отчет", "Необходимо заполнить все поля.");
                return;
            }
            if (FromDate > ToDate)
            {
                dialogService.ShowInfo("Создать отчет", "Дата начала периода не может быть позже даты окончания.");
                return;
            }
            CompanyCategory? category = SelectedCategory!.Id == -1 ? null : (CompanyCategory)SelectedCategory!.Id;
            int? nomenclatureId = SelectedNomenclature!.Id == -1 ? null : SelectedNomenclature.Id;
            var reportData = await accountRepository.GetServiceStatisticsAsync(FromDate!.Value, ToDate!.Value, nomenclatureId, category);
            if (!reportData.HasData)
            {
                dialogService.ShowInfo("Создать отчет", "Подходящие записи не найдены.");
                return;
            }
            CategoryCompany = category == null ? "Все" : SelectedCategory.Name;
            ServiceName = nomenclatureId == null ? "Все" : SelectedNomenclature.Name;
            Unit = nomenclatureId == null ? "—" : SelectedNomenclature.Unit;
            PeriodDisplay = $"{FromDate:d} ─ {ToDate:d}";
            TotalSum = reportData.Sum;
            Volume = reportData.Volume;
        }
        private bool CanCalculate()
        {
            return SelectedNomenclature != null && SelectedCategory != null && ToDate != null && FromDate != null;
        }
        private void ClearForm()
        {
            FromDate = null;
            ToDate = null;
            SelectedCategory = null;
            SelectedNomenclature = null;
            ServiceName = string.Empty;
            PeriodDisplay = string.Empty;
            CategoryCompany = string.Empty;
            Unit = string.Empty;
            TotalSum = null;
            Volume = null;
        }
    }   
}
