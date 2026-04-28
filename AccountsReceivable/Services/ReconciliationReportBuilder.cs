using AccountsReceivable.Interfaces;
using AccountsReceivable.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccountsReceivable.Services
{
    public class ReconciliationReportBuilder : IReconciliationReportBuilder
    {
        public ReconciliationReport Build(List<AccountHeader> accounts, DateTime fromDate, DateTime toDate)
        {
            if (accounts.Count == 0) throw new ArgumentException(nameof(accounts));
            ReconciliationReport report = new ReconciliationReport();
            var paymentBeforeDateSum = accounts.SelectMany(h => h.Payments).Where(p => p.Date < fromDate).Sum(p => p.Sum);
            var accountBeforeDateSum = accounts.SelectMany(h => h.AccountsList).Where(l => l.Period < fromDate).Sum(l => l.AmountWithVat);

            var paymentsSum = accounts.SelectMany(h => h.Payments).Where(p => p.Date >= fromDate && p.Date < toDate.Date.AddDays(1)).Sum(p => p.Sum);
            var accountsSum = accounts.SelectMany(h => h.AccountsList).Where(l => l.Period >= fromDate && l.Period < toDate.Date.AddDays(1)).Sum(l => l.AmountWithVat);

            report.OpeningBalance = accountBeforeDateSum - paymentBeforeDateSum;
            report.Change = accountsSum - paymentsSum;
            report.ClosingBalance = report.OpeningBalance + report.Change;
            report.ChargedTotal = accountsSum;
            report.PaymentsTotal = paymentsSum;
            report.FromDate = fromDate;
            report.ToDate = toDate;
            return report;
        }
    }
}
