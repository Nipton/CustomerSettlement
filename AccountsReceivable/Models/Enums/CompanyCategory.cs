using System.ComponentModel.DataAnnotations;


namespace AccountsReceivable.Models.Enums
{
    public enum CompanyCategory
    {
        [Display(Name = "Бюджет")]
        Budget = 2,
        [Display(Name = "Население")]
        Population = 1,
        [Display(Name = "Прочее")]
        Other = 0
    }
}
