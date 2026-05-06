using CustomerSettlement.Data.Interfaces;
using CustomerSettlement.Models;
using CustomerSettlement.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerSettlement.Data.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IDbContextFactory<ApplicationContext> factory;
        public AccountRepository(IDbContextFactory<ApplicationContext> factory)
        {
            this.factory = factory;               
        }
        public async Task<IEnumerable<AccountHeader>> GetAccountsByDateAsync(DateTime fromDate, DateTime toDate)
        {
            using var context = await factory.CreateDbContextAsync();
            return await context.AccountHeaders.Where(x => x.Date < toDate.Date.AddDays(1) && x.Date >= fromDate.Date).Include(x => x.Company).Include(h => h.OwnerCompany).Include(x => x.Contract).OrderByDescending(x=> x.Id).ToListAsync();
        }
        public async Task<IEnumerable<AccountHeader>> GetAccountsByCompanyAsync(int companyId, int? contractId = null)
        {
            using var context = await factory.CreateDbContextAsync();
            var query = context.AccountHeaders.Where(h => h.CompanyId == companyId);
            if (contractId != null)
                query = query.Where(h => h.ContractId == contractId);
            return await query.Include(h => h.AccountsList).ThenInclude(l => l.Nomenclature).Include(h => h.Payments).Include(h => h.Company).Include(h => h.OwnerCompany).Include(h => h.Contract).ToArrayAsync();
        }
        public async Task<List<AccountLine>> GetAccountLinesByHeaderIdAsync(int headerId)
        {
            using var context = await factory.CreateDbContextAsync();
            return await context.AccountLines.Where(x=> x.AccountHeaderId == headerId).Include(x=> x.Nomenclature).ToListAsync();
        }
        public async Task<List<Payment>> GetPaymentsByHeaderIdAsync(int headerId)
        {
            using var context = await factory.CreateDbContextAsync();
            return await context.Payments.Where(x => x.AccountHeaderId == headerId).ToListAsync();
        }
        public async Task DeleteAccountHeaderById(int headerId)
        {
            using var context = await factory.CreateDbContextAsync();
            await context.AccountHeaders.Where(x=> x.Id == headerId).ExecuteDeleteAsync();
        }
        public async Task DeleteAccountLinesAsync(List<int> linesIdToDelete, AccountHeader accountHeader)
        {
            using var context = await factory.CreateDbContextAsync();
            var trackedHeader = await context.AccountHeaders.Include(h => h.AccountsList).FirstAsync(h => h.Id == accountHeader.Id);
            var linesToDelete = trackedHeader.AccountsList.Where(x => linesIdToDelete.Contains(x.Id)).ToList();
            foreach (var line in linesToDelete)
                trackedHeader.AccountsList.Remove(line);
            trackedHeader.Sum = accountHeader.Sum;
            trackedHeader.PaymentStatus = accountHeader.PaymentStatus;
            await context.SaveChangesAsync();
        }
        public async Task AddPaymentAsync(Payment payment, AccountHeader accountHeader)
        {
            using var context = await factory.CreateDbContextAsync();
            await UpdateHeaderAsync(context, accountHeader);
            await context.Payments.AddAsync(payment);
            await context.SaveChangesAsync();
        }
        public async Task DeletePaymentsAsync(List<int> paymentsIdToDelete, AccountHeader accountHeader)
        {
            using var context = await factory.CreateDbContextAsync();
            var trackedHeader = await context.AccountHeaders.Include(h => h.Payments).FirstAsync(h => h.Id == accountHeader.Id);
            var paymentsToDelete = trackedHeader.Payments.Where(x => paymentsIdToDelete.Contains(x.Id)).ToList();
            foreach (var payment in paymentsToDelete)
                trackedHeader.Payments.Remove(payment);
            trackedHeader.PaymentSum = accountHeader.PaymentSum;
            trackedHeader.PaymentStatus = accountHeader.PaymentStatus;
            await context.SaveChangesAsync();
        }
        public async Task EditPaymentAsync(Payment payment, AccountHeader accountHeader)
        {
            using var context = await factory.CreateDbContextAsync();
            await UpdateHeaderAsync(context, accountHeader);
            context.Update(payment);
            await context.SaveChangesAsync();
        }
        private async Task UpdateHeaderAsync(ApplicationContext context, AccountHeader header)
        {
            var trackedHeader = await context.AccountHeaders.FirstAsync(h => h.Id == header.Id);
            trackedHeader.PaymentSum = header.PaymentSum;
            trackedHeader.PaymentStatus = header.PaymentStatus;
        }
        public async Task<ReportData> GetServiceStatisticsAsync(DateTime fromDate, DateTime toDate, int? nomenclatureId = null, CompanyCategory? companyCategory = null)
        {
            using var context = await factory.CreateDbContextAsync();
            var query = context.AccountLines.Where(l => l.Period >= fromDate && l.Period < toDate.Date.AddDays(1));
            if (nomenclatureId != null)
                query = query.Where(l => l.NomenclatureId == nomenclatureId);
            if (companyCategory != null)
                query = query.Where(l => l.AccountHeader.Company.Category == companyCategory);

            var result = await query.GroupBy(l => 1).Select(g => new ReportData { Sum = g.Sum(l => l.AmountWithVat), Volume = g.Sum(l => l.Quantity), HasData = true }).FirstOrDefaultAsync();
            return result ?? new ReportData { HasData = false };
        }
    }
}
