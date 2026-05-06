using CustomerSettlement.Data.Interfaces;
using CustomerSettlement.Helpers;
using CustomerSettlement.Interfaces;
using CustomerSettlement.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerSettlement.Services
{
    public class ActDocumentService : BaseAccountDocumentService, IDocumentService<AccountHeader>
    {
        public ActDocumentService(ITemplateRepository templateRepository) : base(templateRepository)  {}
        public async Task<string> BuildHtml(AccountHeader header)
        {
            var html = await LoadTemplate("WorkCompletionCertificate.html");

            html = html.Replace("{{Title}}", $"{header.Id} от {header.Date:d MMMM yyyy} года");
            html = html.Replace("{{Contractor}}", StringFormattingHelper.GenerateCompanyInfo(header.OwnerCompany));
            html = html.Replace("{{Customer}}", StringFormattingHelper.GenerateCompanyInfo(header.Company));
            html = html.Replace("{{Contract}}", $"{header.Contract.Number} от {header.Contract.Date:d}");
            html = html.Replace("{{TableRows}}", GetTableRows(header.AccountsList));

            var totals = CalculateTotals(header);
            html = html.Replace("{{TotalSumWithVat}}", header.Sum.ToString("N2"));
            html = html.Replace("{{TotalSum}}", totals.totalWithoutVat.ToString("N2"));
            html = html.Replace("{{SumVat}}", totals.totalVat.ToString("N2"));

            html = html.Replace("{{FinalStatement}}", GetFinalStatement(header));
            html = html.Replace("{{DirectorOurCompany}}", StringFormattingHelper.GenerateFIO(header.OwnerCompany));
            html = html.Replace("{{DirectorCounterparty}}", StringFormattingHelper.GenerateFIO(header.Company));

            return html;
        }
    }
}