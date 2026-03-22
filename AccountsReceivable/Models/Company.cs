using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AccountsReceivable.Models
{
    [Table("Companies")]
    public class Company : ICloneable
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string ShortName { get; set; } = string.Empty;
        [MaxLength(500)]
        public string LegalAddress { get; set; } = string.Empty;
        [MaxLength(500)]
        public string ActualAddress { get; set; } = string.Empty;
        [MaxLength(12)]
        public string Inn { get; set; } = string.Empty;
        [MaxLength(9)]
        public string Kpp { get; set; } = string.Empty;
        [MaxLength(15)]
        public string Ogrn { get; set; } = string.Empty;
        [MaxLength(200)]
        public string Bank { get; set; } = string.Empty;
        [MaxLength(20)]
        public string Rs { get; set; } = string.Empty;
        [MaxLength(20)]
        public string Ks { get; set; } = string.Empty;
        [MaxLength(9)]
        public string Bik { get; set; } = string.Empty;
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;
        [MaxLength(100)]
        public string Position { get; set; } = string.Empty;
        [MaxLength(200)]
        public string DirectorFullName { get; set; } = string.Empty;

        public int? CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
        public void CopyFrom(Company edited)
        {
            Id = edited.Id;
            Name = edited.Name;
            ShortName = edited.ShortName;
            LegalAddress = edited.LegalAddress;
            ActualAddress = edited.ActualAddress;
            Inn = edited.Inn;
            Kpp = edited.Kpp;
            Ogrn = edited.Ogrn;
            Bank = edited.Bank;
            Rs = edited.Rs;
            Ks = edited.Ks;
            Bik = edited.Bik;
            Phone = edited.Phone;
            Position = edited.Position;
            DirectorFullName = edited.DirectorFullName;
            CategoryId = edited.CategoryId;
            Category = edited.Category;
        }
    }
}
