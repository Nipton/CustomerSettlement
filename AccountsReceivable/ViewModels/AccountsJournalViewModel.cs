using AccountsReceivable.Data.Interfaces;
using AccountsReceivable.Interfaces;
using AccountsReceivable.Models;
using AccountsReceivable.Models.Enums;
using AccountsReceivable.ViewModels.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace AccountsReceivable.ViewModels
{
    public class AccountsJournalViewModel : ViewModelBase, ILoadable
    {
        private readonly IAccountRepository accountRepository;
        private readonly IDialogService dialogService;
        private readonly IDocumentServiceFactory documentService;
        private DateTime fromDate;
        private DateTime toDate;
        private AccountHeader? selectedAccHeader;
        private int loadVersion;
        private bool isLoaded;
        private string searchTerm = string.Empty;
        private Dictionary<int, List<AccountLine>> linesCache = new();
        private Dictionary<int, List<Payment>> paymentCache = new();
        public DateTime FromDate { get => fromDate; set {
                if (Set(ref fromDate, value))
                   _ = LoadAccountsByDateAsync(); } }
        public DateTime ToDate { get => toDate; set { 
                if(Set(ref toDate, value))
                   _ = LoadAccountsByDateAsync(); } }
        public string SearchTerm 
        { 
            get => searchTerm;
            set { if (Set(ref searchTerm, value)) AccountHeadersView.Refresh(); }
        } 
        public AccountHeader? SelectedAccHeader { get => selectedAccHeader; set { if (Set(ref selectedAccHeader, value)) _ = LoadAccountDataAfterSelectAsync(); } }
        public IList SelectedAccountLines { get; set; } = new List<AccountLine>();
        public IList SelectedPayments { get; set; } = new List<Payment>();
        public ICollectionView AccountHeadersView { get; }
        public ObservableCollection<AccountHeader> AccountHeaders { get; set; } = new();
        public ObservableCollection<AccountLine> AccountLines { get; set; } = new();
        public ObservableCollection<Payment> Payments { get; set; } = new();
        public ICommand OpenAccountEditorCommand { get; }
        public ICommand DeleteAccHeaderCommand { get; }
        public ICommand EditAccountHeaderCommand { get; }
        public ICommand DeleteAccLinesCommand { get; }
        public ICommand AddPaymentCommand { get; }
        public ICommand DeletePaymentsCommand { get; }
        public ICommand EditPaymentCommand { get; }
        public ICommand PrintAct {  get; }
        public ICommand PrintInvoice {  get; }
        public AccountsJournalViewModel(IDialogService dialogService, IAccountRepository accountRepository, IDocumentServiceFactory documentService)
        {
            this.dialogService = dialogService;
            this.accountRepository = accountRepository;
            this.documentService = documentService;
            AccountHeadersView = CollectionViewSource.GetDefaultView(AccountHeaders);
            AccountHeadersView.Filter = FilterAccount;
            ToDate = DateTime.Today;
            FromDate = ToDate.AddMonths(-3);
            OpenAccountEditorCommand = new AsyncRelayCommand(AddNewAccountHeaderAsync);
            DeleteAccHeaderCommand = new AsyncRelayCommand(DeleteAccountHeaderAsync, _ => SelectedAccHeader != null);
            EditAccountHeaderCommand = new AsyncRelayCommand(EditAccountHeaderAsync, _ => SelectedAccHeader != null);
            DeleteAccLinesCommand = new AsyncRelayCommand(DeleteAccountLinesAsync, _ => SelectedAccountLines.Count > 0);
            AddPaymentCommand = new AsyncRelayCommand(AddPaymentAsync, _ => SelectedAccHeader != null);
            DeletePaymentsCommand = new AsyncRelayCommand(DeletePaymentAsync, _ => SelectedPayments.Count > 0);
            EditPaymentCommand = new AsyncRelayCommand(EditPaymentAsync, _ => SelectedPayments.Count == 1);
            PrintAct = new AsyncRelayCommand(PrintActAsync, _ => SelectedAccHeader != null);
            PrintInvoice = new AsyncRelayCommand(PrintInvoiceAsync, _ => SelectedAccHeader != null);
        }
        public async Task LoadAsync()
        {
            SelectedAccHeader = null;
            if (isLoaded) return;
            await LoadAccountsByDateAsync();
            isLoaded = true;
        }
        private async Task AddNewAccountHeaderAsync()
        {
            try
            {
                AccountHeader accountHeader = new AccountHeader();
                var result = await dialogService.ShowWindowAsync(DialogType.AccountEditor, accountHeader);
                if (!result) return;
                AccountHeaders.Insert(0, accountHeader);
                SelectedAccHeader = accountHeader;
                linesCache[accountHeader.Id] = accountHeader.AccountsList.ToList();
                ReloadAccountLines(accountHeader.AccountsList);
            }
            catch (Exception)
            {
                dialogService.ShowError("Ошибка", "Не удалось создать новый счёт.");
            }
        }
        private async Task EditAccountHeaderAsync()
        {
            if (SelectedAccHeader == null)
            {
                dialogService.ShowInfo("Редактирование", "Необходимо выделить счёт для редактирования.");
                return;
            }
            var header = SelectedAccHeader;
            var result = await dialogService.ShowWindowAsync(DialogType.AccountEditor, header);
            if (!result) return;
            linesCache[header.Id] = header.AccountsList.ToList();
            ReloadAccountLines(header.AccountsList);
            AccountHeadersView.Refresh();
        }
        private async Task DeleteAccountHeaderAsync()
        {
            if (SelectedAccHeader == null)
            {
                dialogService.ShowInfo("Удаление", "Необходимо выделить счёт для удаления.");
                return;
            }
            var header = SelectedAccHeader;
            if (dialogService.ShowConfirmation("Подтверждение удаления", "Вы уверены, что хотите удалить счёт вместе со всеми позициями и оплатами?"))
            {
                await accountRepository.DeleteAccountHeaderById(header.Id);
                AccountHeaders.Remove(header);
                linesCache.Remove(header.Id);
                paymentCache.Remove(header.Id);
            }
        }
        private async Task DeleteAccountLinesAsync()
        {
            if (SelectedAccHeader == null || SelectedAccountLines.Count == 0)
            {
                dialogService.ShowInfo("Удаление", "Не выбраны позиции для удаления.");
                return;
            }
            if (!dialogService.ShowConfirmation("Подтверждение удаления", "Вы уверены, что хотите удалить выбранные позиции?"))
                return;
            var editableHeader = SelectedAccHeader;
            var linesToDelete = SelectedAccountLines.Cast<AccountLine>().ToList();
            if (!linesCache.TryGetValue(editableHeader.Id, out var cachedLines))
            {
                dialogService.ShowInfo("Инфо", "Позиции не найдены в кэше.");
                return;
            }
            foreach (var lineToDelete in linesToDelete)
                cachedLines.Remove(lineToDelete);
            Recalculation(editableHeader);
            try
            {
                var idToDelete = linesToDelete.Select(x => x.Id).ToList();
                await accountRepository.DeleteAccountLinesAsync(idToDelete, editableHeader);
            }
            catch (Exception)
            {
                dialogService.ShowError("Ошибка", "Не удалось удалить позиции счета");
                foreach (var lineToDelete in linesToDelete)
                    cachedLines.Add(lineToDelete);
                Recalculation(editableHeader);
            }
            finally
            {
                AccountHeadersView.Refresh();
                ReloadAccountLines(cachedLines);
            }
        }
        private async Task AddPaymentAsync()
        {
            if (SelectedAccHeader == null)
            {
                dialogService.ShowInfo("Добавление оплаты", "Необходимо выделить счёт, чтобы добавить оплату.");
                return;
            }
            var header = SelectedAccHeader;
            Payment payment = new Payment() { AccountHeaderId = header.Id};
            var result = await dialogService.ShowWindowAsync(DialogType.PaymentEditor, payment);
            if (!result)
                return;
            try
            {
                if (paymentCache.TryGetValue(header.Id, out var payments))
                    payments.Add(payment);
                else
                {
                    List<Payment> list = new List<Payment>() { payment };
                    paymentCache[header.Id] = list;
                }
                Recalculation(header);
                await accountRepository.AddPaymentAsync(payment, header);
                Payments.Add(payment);
            }
            catch(Exception)
            {
                if (paymentCache.TryGetValue(header.Id, out var cached))
                    cached.Remove(payment);
                Recalculation(header);
                Payments.Remove(payment);
                dialogService.ShowError("Ошибка", "Не удалось сохранить оплату.");
            }
            finally
            {
                AccountHeadersView.Refresh();
            }
        }
        private async Task EditPaymentAsync()
        {
            if (SelectedAccHeader == null || SelectedPayments.Count != 1)
            {
                dialogService.ShowInfo("Редактирование", "Для редактирования необходимо выбрать один элемент.");
                return;
            }
            if (!paymentCache.TryGetValue(SelectedAccHeader.Id, out var cachedPayment))
            {
                dialogService.ShowInfo("Инфо", "Оплата не найдена в кэше.");
                return;
            }
            var header = SelectedAccHeader;
            var payment = SelectedPayments.Cast<Payment>().First();
            Payment? oldPayment =  payment.Clone() as Payment;
            var result = await dialogService.ShowWindowAsync(DialogType.PaymentEditor, payment);
            if (!result) return;
            try
            {
                Recalculation(header);
                await accountRepository.EditPaymentAsync(payment, header);
                ReloadPayments(cachedPayment);
            }
            catch
            {
                cachedPayment.Remove(payment);
                cachedPayment.Add(oldPayment!);
                Recalculation(header);
                ReloadPayments(cachedPayment);
                dialogService.ShowError("Ошибка", "Не удалось изменить оплату.");
            }
            finally
            {
                AccountHeadersView.Refresh();
            }
        }
        private async Task DeletePaymentAsync()
        {
            if (SelectedAccHeader == null || SelectedPayments.Count == 0)
            {
                dialogService.ShowInfo("Удаление", "Не выбраны позиции для удаления.");
                return;
            }
            if (!dialogService.ShowConfirmation("Подтверждение удаления", "Вы уверены, что хотите удалить выбранные позиции?"))
                return;
            var editableHeader = SelectedAccHeader;
            var paymentsToDelete = SelectedPayments.Cast<Payment>().ToList();
            if (!paymentCache.TryGetValue(editableHeader.Id, out var cachedPayment))
            {
                dialogService.ShowInfo("Инфо", "Оплата не найдена в кэше.");
                return;
            }
            foreach (var payment in paymentsToDelete) 
                cachedPayment.Remove(payment);
            Recalculation(editableHeader);
            try
            {
                var idToDelete = paymentsToDelete.Select(x => x.Id).ToList();
                await accountRepository.DeletePaymentsAsync(idToDelete, editableHeader);
            }
            catch (Exception)
            {
                dialogService.ShowError("Ошибка", "Не удалось удалить позиции счета");
                foreach (var paymentToDelete in paymentsToDelete)
                    cachedPayment.Add(paymentToDelete);
                Recalculation(editableHeader);
            }
            finally
            {
                AccountHeadersView.Refresh();
                ReloadPayments(cachedPayment);
            }
        }
        private void Recalculation(AccountHeader accountHeader)
        {
            if (linesCache.TryGetValue(accountHeader.Id, out var lines))
                accountHeader.Sum = lines.Sum(x => x.AmountWithVat);
            if (paymentCache.TryGetValue(accountHeader.Id, out var payments))
                accountHeader.PaymentSum = payments.Sum(x => x.Sum);
            accountHeader.PaymentStatus = accountHeader.Sum > 0 && accountHeader.PaymentSum >= accountHeader.Sum;
        }
        private void ReloadAccountLines(IEnumerable<AccountLine> accountLines)
        {
            AccountLines.Clear();
            foreach (var accountLine in accountLines)
                AccountLines.Add(accountLine);
        }
        private void ReloadPayments(IEnumerable<Payment> payments)
        {
            Payments.Clear();
            foreach (var payment in payments)
                Payments.Add(payment);
        }
        private async Task LoadAccountsByDateAsync()
        {
            int version = ++loadVersion;
            try
            {
                var loadedHeaders = await accountRepository.GetAccountsByDateAsync(FromDate, ToDate);
                if (version != loadVersion)
                    return;
                AccountHeaders.Clear();
                foreach (var accountHeader in loadedHeaders)
                    AccountHeaders.Add(accountHeader);
            }
            catch (Exception)
            {
                dialogService.ShowError("Ошибка загрузки", "Не удалось загрузить список счетов.");
            }
        }
        private async Task LoadAccountDataAfterSelectAsync()
        {
            if (SelectedAccHeader == null)
            {
                AccountLines.Clear();
                Payments.Clear();
                return;
            }
            try
            {
                List<AccountLine> accountLines;
                if (!linesCache.TryGetValue(SelectedAccHeader.Id, out accountLines!))
                {
                    accountLines = await accountRepository.GetAccountLinesByHeaderIdAsync(SelectedAccHeader.Id);
                    linesCache[SelectedAccHeader.Id] = accountLines;
                }
                ReloadAccountLines(accountLines);
            }
            catch (Exception)
            {
                dialogService.ShowError("Ошибка загрузки", "Не удалось загрузить список позиций счета .");
            }
            try
            {
                List<Payment> payments;
                if (!paymentCache.TryGetValue(SelectedAccHeader.Id, out payments!))
                {
                    payments = await accountRepository.GetPaymentsByHeaderIdAsync(SelectedAccHeader.Id);
                    paymentCache[SelectedAccHeader.Id] = payments;
                }
                ReloadPayments(payments);
            }
            catch (Exception)
            {
                dialogService.ShowError("Ошибка загрузки", "Не удалось загрузить список оплаты.");
            }
        }
        private bool FilterAccount(object obj)
        {
            if (obj is not AccountHeader account)
                return false;
            if (string.IsNullOrWhiteSpace(SearchTerm))
                return true;
            if (account.Company.Name != null && account.Company.Name.IndexOf(SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
            if (account.Contract.Number != null && account.Contract.Number.IndexOf(SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
            return false;
        }
        private async Task PrintActAsync()
        {
            if (SelectedAccHeader == null)
            {
                dialogService.ShowInfo("Печать акта", "Для печати акта необходимо выбрать один счёт.");
                return;
            }
            var header = SelectedAccHeader;
            header.AccountsList = linesCache[header.Id];
            var actService = documentService.GetActService();
            try
            {
                var html = await actService.BuildHtml(header);
                await dialogService.ShowWindowAsync(DialogType.PrintPreview, html);
            }
            catch (Exception)
            {
                dialogService.ShowError("Ошибка", "Ошибка во время печати.");
            }
        }
        private async Task PrintInvoiceAsync()
        {
            if (SelectedAccHeader == null)
            {
                dialogService.ShowInfo("Печать счёта на оплату", "Для печати счёта на оплату необходимо выбрать один счёт.");
                return;
            }
            var header = SelectedAccHeader;
            header.AccountsList = linesCache[header.Id];
            var invoiceService = documentService.GetInvoiceService();
            try
            {
                var html = await invoiceService.BuildHtml(header);
                await dialogService.ShowWindowAsync(DialogType.PrintPreview, html);
            }
            catch (Exception)
            {
                dialogService.ShowError("Ошибка", "Ошибка во время печати.");
            }
        }
    }
}