using CustomerSettlement.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerSettlement.Interfaces
{
    public interface IReconciliationReportBuilder
    {
        ReconciliationReport Build(List<AccountHeader> accounts, Contract? contract, DateTime fromDate, DateTime toDate);
    }
}
