using AccountsReceivable.Models;
using System;
using System.Collections.Generic;

namespace AccountsReceivable.Interfaces
{
    public interface IReconciliationReportBuilder
    {
        ReconciliationReport Build(List<AccountHeader> accounts, DateTime fromDate, DateTime toDate);
    }
}
