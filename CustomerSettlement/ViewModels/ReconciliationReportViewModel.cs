using CustomerSettlement.Data.Interfaces;
using CustomerSettlement.Interfaces;
using CustomerSettlement.Models;
using CustomerSettlement.Models.Enums;
using CustomerSettlement.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CustomerSettlement.ViewModels
{
    public class ReconciliationReportViewModel : ViewModelBase, ILoadable
    {
        private readonly IDialogService dialogService;
        private readonly ICompanyRepository companyRepository;
        private readonly IContractRepository contractRepository;
        private readonly IAccountRepository accountRepository;
        private readonly IReconciliationReportBuilder reportBuilder;
        private readonly IDocumentServiceFactory documentService;
        private bool isLoaded;
        private ReconciliationReport? reportData;
        private decimal? openingBalance;
        private decimal? closingBalance;
        private decimal? chargedTotal;
        private decimal? paymentsTotal;
        private decimal? change;
        private string? period;
        private Company? selectedCompany;
        private Contract? selectedContract;
        private DateTime? fromDate;
        private DateTime? toDate;
        public DateTime? FromDate { get => fromDate; set => Set(ref fromDate, value); }
        public DateTime? ToDate { get => toDate; set => Set(ref toDate, value); }
        public ObservableCollection<Company> Companies { get; set; } = new();
        public ObservableCollection<Contract> Contracts { get; set; } = new();
        public ObservableCollection<AccountLine> AccountLines { get; set; } = new();
        public ObservableCollection<Payment> Payments { get; set; } = new();
        public Company? SelectedCompany
        {
            get => selectedCompany; 
            set
            {
                if (Set(ref selectedCompany, value))
                    _ = LoadContractAsync();
            }
        }
        public Contract? SelectedContract { get => selectedContract; set => Set(ref selectedContract, value); }
        public decimal? OpeningBalance { get => openingBalance; set => Set(ref openingBalance, value); }
        public decimal? ClosingBalance { get => closingBalance; set => Set(ref closingBalance, value); }
        public decimal? ChargedTotal { get => chargedTotal; set => Set(ref chargedTotal, value); }
        public decimal? PaymentsTotal { get => paymentsTotal; set => Set(ref paymentsTotal, value); }
        public decimal? Change { get => change; set => Set(ref change, value); }
        public string? Period { get => period; set => Set(ref period, value); }
        public ICommand GenerateReportCommand { get; }
        public ICommand PrintReportCommand {  get; }
        public ICommand ClearFormCommand { get; }
        public ReconciliationReportViewModel(IDialogService dialogService, ICompanyRepository companyRepository, IContractRepository contractRepository, IAccountRepository accountRepository, IReconciliationReportBuilder reportBuilder, IDocumentServiceFactory documentService)
        {
            this.accountRepository = accountRepository;
            this.dialogService = dialogService;
            this.companyRepository = companyRepository;
            this.contractRepository = contractRepository;
            this.reportBuilder = reportBuilder;
            this.documentService = documentService;
            companyRepository.DataChanged += LoadCompaniesAsync;
            GenerateReportCommand = new AsyncRelayCommand(GenerateReport, _ => CanGenerateReport());
            ClearFormCommand = new RelayCommand(_ => ClearAll());
            PrintReportCommand = new AsyncRelayCommand(PrintReconciliationReport);
        }

        public async Task LoadAsync()
        {
            if (isLoaded) return;
            await LoadCompaniesAsync();
            isLoaded = true;
        }
        private async Task LoadCompaniesAsync()
        {
            try
            {
                Companies.Clear();
                var companies = await companyRepository.GetAllCompaniesAsync();
                foreach (var company in companies)
                    Companies.Add(company);
            }
            catch (Exception)
            {
                dialogService.ShowError("Ошибка!", "Не удалось загрузить список контрагентов.");
            }
        }
        private async Task LoadContractAsync()
        {
            Contracts.Clear();
            if (SelectedCompany == null) return;
            try
            {
                var contracts = await contractRepository.GetContractsByCompanyIdAsync(SelectedCompany.Id);
                foreach (var contract in contracts)
                    Contracts.Add(contract);
            }
            catch (Exception)
            {
                dialogService.ShowError("Ошибка!", "Не удалось загрузить список договоров.");
            }
        }
        private bool CanGenerateReport()
        {
            return SelectedCompany != null && FromDate != null && ToDate != null;
        }
        private async Task GenerateReport()
        {
            if (!CanGenerateReport())
            {
                dialogService.ShowInfo("Формирование акта", "Необходимо выбрать компанию и указать даты.");
                return;
            }
            if (FromDate > ToDate)
            {
                dialogService.ShowInfo("Формирование акта", "Дата начала периода не может быть позже даты окончания.");
                return;
            }
            List<AccountHeader> accounts;
            var selectedCompany = SelectedCompany;
            var selectedContract = SelectedContract;
            try
            {
                accounts = (await accountRepository.GetAccountsByCompanyAsync(selectedCompany!.Id, selectedContract?.Id)).ToList();
            }
            catch (Exception)
            {
                dialogService.ShowError("Ошибка!", "Не удалось загрузить список счетов.");
                return;
            }
            ClearForm();
            ReconciliationReport reconciliationReport;
            try
            {
                reconciliationReport = reportBuilder.Build(accounts, selectedContract, FromDate!.Value, ToDate!.Value);
                foreach (var accountLine in reconciliationReport.AccountLines)
                    AccountLines.Add(accountLine);
                foreach (var payment in reconciliationReport.Payments)
                    Payments.Add(payment);
                OpeningBalance = reconciliationReport.OpeningBalance;
                ClosingBalance = reconciliationReport.ClosingBalance;
                Change = reconciliationReport.Change;
                ChargedTotal = reconciliationReport.ChargedTotal;
                PaymentsTotal = reconciliationReport.PaymentsTotal;
                Period = reconciliationReport.FormattedPeriod;
                reportData = reconciliationReport;
            }
            catch (Exception)
            {
                dialogService.ShowError("Ошибка!", "Не удалось выполнить расчёты.");
                return;
            }
        }
        private async Task PrintReconciliationReport()
        {
            if (reportData == null)
            {
                dialogService.ShowInfo("Печать", "Для печати необходимо сперва сформировать акт");
                return;
            }
            var reconciliationService = documentService.GetReconciliationService();
            var html = await reconciliationService.BuildHtml(reportData);
            await dialogService.ShowWindowAsync(DialogType.PrintPreview, html);
        }
        private void ClearForm()
        {
            OpeningBalance = null;
            ClosingBalance = null;
            Change = null;
            ChargedTotal = null;
            PaymentsTotal = null;
            Period = null;
            AccountLines.Clear();
            Payments.Clear();
            reportData = null;
        }
        private void ClearAll()
        {
            FromDate = null;
            ToDate = null;
            SelectedCompany = null;
            ClearForm();
        }
    }
}
