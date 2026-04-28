using System;

namespace AccountsReceivable.Models
{
    public class ReconciliationReport
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
        public decimal ChargedTotal { get; set; }
        public decimal PaymentsTotal { get; set; }
        public decimal Change { get; set; }
        public string FormattedPeriod => $"{FromDate:d} ─ {ToDate:d}";
    }
}