using AccountsReceivable.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AccountsReceivable.Data.Interfaces
{
    public interface IAccountRepository
    {
        Task<IEnumerable<AccountHeader>> GetAccountsByDateAsync(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<AccountHeader>> GetAccountsByCompanyAsync(int companyId, int? contractId = null);
        Task<List<AccountLine>> GetAccountLinesByHeaderIdAsync(int headerId);
        Task<List<Payment>> GetPaymentsByHeaderIdAsync(int headerId);
        Task DeleteAccountHeaderById(int headerId);
        Task DeleteAccountLinesAsync(List<int> linesIdToDelete, AccountHeader accountHeader);
        Task AddPaymentAsync(Payment payment, AccountHeader accountHeader);
        Task DeletePaymentsAsync(List<int> paymentsIdToDelete, AccountHeader accountHeader);
        Task EditPaymentAsync(Payment payment, AccountHeader accountHeader);
    }
}
