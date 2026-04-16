using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsReceivable.Models
{
    public class Payment
    {
		public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public required string Number { get; set; }
        [Precision(18, 2)]
        public decimal Sum { get; set; }
		public DateTime Date { get; set; }
		public int AccountHeaderId {  get; set; }
		public AccountHeader AccountHeader { get; set; } = null!;
    }
}
