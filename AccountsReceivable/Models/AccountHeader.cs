using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsReceivable.Models
{
    public class AccountHeader
    {
        public int Id { get; set; }
        public Contract? Contract { get; set; }
        public string Company { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Today;
        public  decimal Sum { get; set; }
        public decimal? Payment { get; set; }
        public bool? PaymentStatus { get; set; }
        public bool? ActStatus { get; set; }
        public List<AccountLine> AccountsList { get; set; } = new();
    }
}
