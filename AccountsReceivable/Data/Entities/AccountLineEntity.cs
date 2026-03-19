using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsReceivable.Data.Entities
{
    [Table("AccountLines")]
    public class AccountLineEntity
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string? Nomenclature { get; set; }
        public string? Unit { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal VatRate { get; set; }
        public decimal Sum { get; set; }
        public DateTime? Period { get; set; }

        [ForeignKey(nameof(Number))]
        public virtual AccountHeaderEntity? AccountHeader { get; set; }
    }
}
