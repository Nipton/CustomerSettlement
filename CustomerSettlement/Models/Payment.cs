using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerSettlement.Models
{
    public class Payment : ICloneable
    {
		public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Number { get; set; } = string.Empty;
        [Precision(18, 2)]
        public decimal Sum { get; set; }
		public DateTime Date { get; set; }
		public int AccountHeaderId {  get; set; }
		public AccountHeader AccountHeader { get; set; } = null!;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
