using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsReceivable.Models
{
    [Table("Nomenclatures")]
    public class Nomenclature
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = "";
        [Required]
        [MaxLength(50)]
        public string Unit { get; set; } = "";

        public virtual ICollection<AccountLine> AccountList { get; set; } = new List<AccountLine>();
    }
}
