using System;
using System.ComponentModel.DataAnnotations;

namespace AccountsReceivable.Models
{
    public class Contract
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public required string Number { get; set; }
        public DateOnly Date {  get; set; } 
        public int NomenclatureId { get; set; }
        public int CompanyId { get; set; }
        public virtual Nomenclature Nomenclature { get; set; } = null!;
        public virtual Company Company { get; set; } = null!;
    }
}
