using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsReceivable.Models
{
    [Table("AccountLines")]
    public class AccountLine
    {
        public int Id { get; set; }
        public int NomenclatureId { get; set; }
        public virtual Nomenclature Nomenclature { get; set; } = null!;
        [Precision(18, 3)]
        public decimal Quantity { get; set; } = 1m;
        [Precision(18, 2)]
        public decimal Price { get; set; } = 0m;
        [Precision(5, 2)]
        public decimal VatRate { get; set; } = 0m;
        [Precision(18, 2)]
        public decimal AmountWithVat { get; set; }
        public DateOnly Period { get; set; }
        public int AccountHeaderId { get; set; }
        public virtual AccountHeader AccountHeader { get; set; } = null!;
        public void RecalculateAmount()
        {
            AmountWithVat = Quantity * Price * (1 + VatRate / 100);
        }
    }
}
