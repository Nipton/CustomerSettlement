using AccountsReceivable.Data.Interfaces;
using AccountsReceivable.Helpers;
using AccountsReceivable.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace AccountsReceivable.Services
{
    public abstract class BaseAccountDocumentService
    {
        protected readonly ITemplateRepository templateRepository;

        protected BaseAccountDocumentService(ITemplateRepository templateRepository)
        {
            this.templateRepository = templateRepository;
        }
        protected async Task<string> LoadTemplate(string templateName)
        {
            var html = await templateRepository.GetTemplateAsync(templateName);
            var css = await templateRepository.GetTemplateAsync("style.css");
            return html.Replace("{{Styles}}", $"<style>{css}</style>");
        }
        protected string GetTableRows(ICollection<AccountLine> lines)
        {
            int i = 1;
            var sb = new StringBuilder();
            foreach (var line in lines)
            {
                sb.Append(@$"
                            <tr>
                                <td>{i++}</td>
                                <td>{line.Nomenclature?.Name}</td>
                                <td>{line.Quantity:N1}</td>
                                <td>{line.Nomenclature?.Unit}</td>
                                <td>{line.Price:N2}</td>
                                <td>{line.AmountWithoutVat:N2}</td>
                            </tr>");
            }

            return sb.ToString();
        }
        protected string GetFinalStatement(AccountHeader header)
        {
            decimal totalVat = header.Sum - header.AccountsList.Sum(x => x.AmountWithoutVat);
            string finalStatement = $"Всего выполнено работ (оказано услуг) {header.AccountsList.Count}, на сумму {header.Sum:N2} руб. ({RublesSpellingConverter.Convert(header.Sum)}).";
            finalStatement += totalVat == 0 ? " НДС не облагается." : $" В том числе НДС - {totalVat:N2} руб. ({RublesSpellingConverter.Convert(totalVat)})";
            return finalStatement;
        }
        protected (decimal totalWithoutVat, decimal totalVat) CalculateTotals(AccountHeader header)
        {
            decimal sumWithoutVat = header.AccountsList.Sum(x => x.AmountWithoutVat);
            decimal sumVat = header.Sum - sumWithoutVat;
            return (sumWithoutVat, sumVat);
        }
    }
}
