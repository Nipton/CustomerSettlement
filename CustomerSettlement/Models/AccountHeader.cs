using CustomerSettlement.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerSettlement.Models
{
    [Table("AccountHeaders")]
    public class AccountHeader
    {
        public int Id { get; set; } 
        
        public int OwnerCompanyId { get; set; } = Constants.OWN_COMPANY_ID;
        public int CompanyId { get; set; }
        /// <summary>
        /// Контрагент (покупатель/заказчик)
        /// </summary>
        public virtual Company Company { get; set; } = null!;
        /// <summary>
        /// Наша компания (от лица которой работаем)
        /// </summary>
        public virtual Company OwnerCompany { get; set; } = null!;
        public DateTime Date { get; set; }
        [Precision(18, 2)]
        public decimal Sum { get; set; }
        public decimal PaymentSum { get; set; }
        public bool PaymentStatus { get; set; }
        public bool ActStatus { get; set; }

        public int ContractId { get; set; }
        public virtual Contract Contract { get; set; } = null!;
        
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<AccountLine> AccountsList { get; set; } = new List<AccountLine>();
    }
}
