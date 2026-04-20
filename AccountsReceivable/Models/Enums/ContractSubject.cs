using System.ComponentModel.DataAnnotations;

namespace AccountsReceivable.Models.Enums
{
    public enum ContractSubject
    {
        [Display(Name = "Выполнение работ")]
        Work = 5,
        [Display(Name = "Оказание услуг")]
        Service = 4,
        [Display(Name = "Купля-продажа")]
        Sale = 3,
        [Display(Name = "Поставка")]
        Supply = 2,
        [Display(Name = "Аренда")]
        Rent = 1,
        [Display(Name = "Прочее")]
        Other = 0
    }
}
