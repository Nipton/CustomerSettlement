using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsReceivable.Models
{
    [Table("AccountHeaders")]
    public class AccountHeader
    {
        public int Id { get; set; }    
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; } = null!;
        public DateTime Date { get; set; } = DateTime.Today;
        [Precision(18, 2)]
        public decimal Sum { get; set; }
        [Precision(18, 2)]
        public decimal? Payment { get; set; }
        public bool? PaymentStatus { get; set; }
        public bool? ActStatus { get; set; }

        public int? ContractId { get; set; }
        public virtual Contract? Contract { get; set; }

        public virtual ICollection<AccountLine> AccountsList { get; set; } = new List<AccountLine>();
    }
}
