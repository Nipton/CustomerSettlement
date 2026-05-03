using AccountsReceivable.Data.Interfaces;
using AccountsReceivable.Helpers;
using AccountsReceivable.Interfaces;
using AccountsReceivable.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsReceivable.Services
{
    public class ActDocumentService : IDocumentService<AccountHeader>
    {
        private readonly ITemplateRepository templateRepository;
        public ActDocumentService(ITemplateRepository templateRepository)
        {
            this.templateRepository = templateRepository;
        }
        public async Task<string> BuildHtml(AccountHeader header)
        {
            var html = await templateRepository.GetTemplateAsync("WorkCompletionCertificate.html");
            var css = await templateRepository.GetTemplateAsync("style.css");

            html = html.Replace("{{Styles}}", $"<style>{css}</style>");

            html = html.Replace("{{Title}}", $"{header.Id} от {header.Date:d MMMM yyyy} года");
            html = html.Replace("{{Contractor}}", StringFormattingHelper.GenerateCompanyInfo(header.OwnerCompany));
            html = html.Replace("{{Customer}}", StringFormattingHelper.GenerateCompanyInfo(header.Company));
            html = html.Replace("{{Contract}}", $"{header.Contract.Number} от {header.Contract.Date:d}");
            html = html.Replace("{{TableRows}}", GetTableRows(header.AccountsList));

            html = html.Replace("{{TotalSumWithVat}}", header.Sum.ToString("N2"));
            decimal sumWithoutVat = header.AccountsList.Sum(x => x.AmountWithoutVat);
            decimal sumVat = header.Sum - sumWithoutVat;
            html = html.Replace("{{TotalSum}}", sumWithoutVat.ToString("N2"));
            html = html.Replace("{{SumVat}}", sumVat.ToString("N2"));

            var finalStatement = $"Всего выполнено работ (оказано услуг) {header.AccountsList.Count}, на сумму {header.Sum:N2} руб. ({RusCurrency.Str(header.Sum)}).";
            finalStatement += sumVat == 0 ? " НДС не облагается." : $" В том числе НДС - {sumVat:N2} руб. ({RusCurrency.Str(sumVat)})";
            html = html.Replace("{{FinalStatement}}", finalStatement);

            html = html.Replace("{{DirectorOurCompany}}", StringFormattingHelper.GenerateFIO(header.OwnerCompany));
            html = html.Replace("{{DirectorCounterparty}}", StringFormattingHelper.GenerateFIO(header.Company));

            return html;
        }
        private string GetTableRows(ICollection<AccountLine> lines)
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
    }
}