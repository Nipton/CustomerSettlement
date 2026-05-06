using CustomerSettlement.Data.Interfaces;
using CustomerSettlement.Helpers;
using CustomerSettlement.Interfaces;
using CustomerSettlement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerSettlement.Services
{
    public class InvoiceDocumentService : BaseAccountDocumentService, IDocumentService<AccountHeader>
    {  
        public InvoiceDocumentService(ITemplateRepository templateRepository) : base(templateRepository) { }
        public async Task<string> BuildHtml(AccountHeader header)
        {
            var html = await LoadTemplate("PaymentInvoice.html");

            var ownerCompany = header.OwnerCompany;
            html = html.Replace("{{RecipientBank}}", ownerCompany.Bank);
            html = html.Replace("{{BIK}}", ownerCompany.Bik);
            html = html.Replace("{{RS}}", ownerCompany.Rs);
            html = html.Replace("{{INN}}", ownerCompany.Inn);
            html = html.Replace("{{KPP}}", ownerCompany.Kpp);
            html = html.Replace("{{KS}}", ownerCompany.Ks);
            html = html.Replace("{{Recipient}}", ownerCompany.Name);
            html = html.Replace("{{AccountNumber}}", $"{header.Id} от {header.Date:d} г.");

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

            return html;
        }
    }
}
