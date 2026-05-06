using CustomerSettlement.Interfaces;
using CustomerSettlement.Models;
using CustomerSettlement.ViewModels.Commands;
using System;
using System.Windows.Input;

namespace CustomerSettlement.ViewModels
{
    public class PaymentViewModel : ViewModelBase
    {
        private readonly Payment payment;
        private readonly IDialogService dialogService;
        private string paymentNumber = string.Empty;
        private DateTime date = DateTime.Today;
        private decimal? paymentSum = 0;
        public string PaymentNumber { get => paymentNumber; set => Set(ref paymentNumber, value); }
        public DateTime Date { get => date; set => Set(ref date, value); }
        private bool isValidSum;
        public decimal? PaymentSum
        {
            get => paymentSum; set
            {
                if (value == null)
                {
                    isValidSum = false;
                    throw new ArgumentNullException("Неверный ввод.");
                }
                if (value < 0)
                {
                    isValidSum = false;
                    throw new ArgumentOutOfRangeException("Сумма не может быть отрицательной.");
                }
                if(Set(ref paymentSum, value))
                    isValidSum = true;
            }
        }
        public ICommand CancelCommand { get; }
        public ICommand ApplyCommand { get; }
        public PaymentViewModel(Payment payment, IDialogService dialogService) 
        {
            this.payment = payment;
            this.dialogService = dialogService;
            if (payment.Id != 0)
            {
                PaymentNumber = payment.Number;
                PaymentSum = payment.Sum;
                Date = payment.Date;
            }
            CancelCommand = new RelayCommand(_ => CancelWindow());
            ApplyCommand = new RelayCommand(_ =>  Apply(), _ => IsValidForm());
        }
        private void Apply()
        {
            if (!IsValidForm()) return;
            payment.Sum = PaymentSum!.Value;
            payment.Number = PaymentNumber;
            payment.Date = Date;
            dialogService.CloseWindow(this, true);
        }
        private bool IsValidForm()
        {
            return !string.IsNullOrWhiteSpace(PaymentNumber) && isValidSum;
        }
        private void CancelWindow()
        {
            dialogService.CloseWindow(this, false);
        }
    }
}
