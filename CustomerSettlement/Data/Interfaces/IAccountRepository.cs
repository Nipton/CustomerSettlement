using CustomerSettlement.Models;
using CustomerSettlement.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerSettlement.Data.Interfaces
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
        Task<ReportData> GetServiceStatisticsAsync(DateTime fromDate, DateTime toDate, int? nomenclatureId = null, CompanyCategory? companyCategory = null);
    }
}
