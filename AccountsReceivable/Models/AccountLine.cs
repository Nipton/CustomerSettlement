using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsReceivable.Models
{
    public class AccountLine
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string? Nomenclature { get; set; }
        public string? Unit { get; set; }
        public decimal Quantity { get; set; } = 1m;
        public decimal Price { get; set; } = 0m;
        public decimal VatRate { get; set; } = 0m;
        public decimal Sum { get; set; }
        public DateTime? Period { get; set; } = null;
        public decimal? SumWithoutVat { get; set; }
        public decimal? SumVat { get; set; }
    }
}
