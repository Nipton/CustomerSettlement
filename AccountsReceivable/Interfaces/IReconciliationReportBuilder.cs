using AccountsReceivable.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountsReceivable.Interfaces
{
    public interface IReconciliationReportBuilder
    {
        ReconciliationReport Build(List<AccountHeader> accounts, Contract? contract, DateTime fromDate, DateTime toDate);
    }
}
