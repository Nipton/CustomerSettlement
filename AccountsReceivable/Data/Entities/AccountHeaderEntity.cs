using AccountsReceivable.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsReceivable.Data.Entities
{
    [Table("AccountHeaders")]
    [Index(nameof(Id))]
    public class AccountHeaderEntity
    {
        public int Id { get; set; }
        public int? ContractID { get; set; }
        public string Company { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Today;
        public decimal Sum { get; set; }
        public decimal? Payment { get; set; }
        public bool? PaymentStatus { get; set; }
        public bool? ActStatus { get; set; }
        [ForeignKey(nameof(ContractID))]
        public virtual ContractEntity? Contract { get; set; }
        public virtual ICollection<AccountLineEntity> AccountsList { get; set; } = new List<AccountLineEntity>();
    }
}
