using System;
using System.Collections.Generic;

namespace AccountsReceivable.Models
{
    public class ReconciliationReport
    {
        public required Company OurCompany { get; set; }
        public required Company Counterparty { get; set; }
        public Contract? Contract { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
        public decimal ChargedTotal { get; set; }
        public decimal PaymentsTotal { get; set; }
        public decimal Change { get; set; }
        public string FormattedPeriod => $"{FromDate:d} ─ {ToDate:d}";

        public List<Payment> Payments { get; set; } = new();
        public List<AccountLine> AccountLines { get; set; } = new();
        public List<ReconciliationEntry> ReconciliationEntries { get; set; } = new();

        
    }
    public class ReconciliationEntry
    {
        public DateTime Date { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        public decimal? Debit { get; set; }  
        public decimal? Credit { get; set; } 
    }
}