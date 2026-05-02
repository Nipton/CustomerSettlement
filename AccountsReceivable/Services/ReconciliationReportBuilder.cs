using AccountsReceivable.Data.Interfaces;
using AccountsReceivable.Helpers;
using AccountsReceivable.Interfaces;
using AccountsReceivable.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountsReceivable.Services
{
    public class ReconciliationReportBuilder : IReconciliationReportBuilder
    {
        public ReconciliationReport Build(List<AccountHeader> accounts, Contract? contract, DateTime fromDate, DateTime toDate)
        {
            if (accounts.Count == 0) throw new ArgumentException(nameof(accounts));

            ReconciliationReport report = new ReconciliationReport() { OurCompany = accounts.First().OwnerCompany, Counterparty = accounts.First().Company };
            if (contract != null)
                report.Contract = contract;

            var paymentBeforeDateSum = accounts.SelectMany(h => h.Payments).Where(p => p.Date < fromDate).Sum(p => p.Sum);
            var accountBeforeDateSum = accounts.SelectMany(h => h.AccountsList).Where(l => l.Period < fromDate).Sum(l => l.AmountWithVat);


            var payments = accounts.SelectMany(h => h.Payments).Where(p => p.Date >= fromDate && p.Date < toDate.Date.AddDays(1)).ToList();
            var accountLines = accounts.SelectMany(h => h.AccountsList).Where(l => l.Period >= fromDate && l.Period < toDate.Date.AddDays(1)).ToList();
            var paymentsSum = payments.Sum(p => p.Sum);
            var accountsSum = accountLines.Sum(l => l.AmountWithVat);

            report.ReconciliationEntries = ReconciliationEntryBuild(accountLines, payments);
            report.Payments = payments;
            report.AccountLines = accountLines;
            report.OpeningBalance = accountBeforeDateSum - paymentBeforeDateSum;
            report.Change = accountsSum - paymentsSum;
            report.ClosingBalance = report.OpeningBalance + report.Change;
            report.ChargedTotal = accountsSum;
            report.PaymentsTotal = paymentsSum;
            report.FromDate = fromDate;
            report.ToDate = toDate;
            return report;
        }
        private List<ReconciliationEntry> ReconciliationEntryBuild(List<AccountLine> accountLines, List<Payment> payments)
        {
            List<ReconciliationEntry> reconciliationEntries = new List<ReconciliationEntry>(accountLines.Count + payments.Count);
            foreach (var accountLine in accountLines)
            {
                reconciliationEntries.Add(new ReconciliationEntry
                {
                    Date = accountLine.Period,
                    Credit = null,
                    Debit = accountLine.AmountWithVat,
                    DocumentName = $"Акт №{accountLine.AccountHeader.Id} от {accountLine.AccountHeader.Date:d}"
                });
            }
            foreach (var payment in payments)
            {
                reconciliationEntries.Add(new ReconciliationEntry
                {
                    Date = payment.Date,
                    Credit = payment.Sum,
                    Debit = null,
                    DocumentName = $"П/п №{payment.Number} от {payment.Date:d}"
                });
            }
            return reconciliationEntries.OrderBy(x => x.Date).ThenByDescending(x => x.Debit.HasValue).ToList();
        }
    }
}
