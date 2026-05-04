using AccountsReceivable.Data.Interfaces;
using AccountsReceivable.Helpers;
using AccountsReceivable.Interfaces;
using AccountsReceivable.Models;
using System;
using System.Text;
using System.Threading.Tasks;

namespace AccountsReceivable.Services
{
    public class ReconciliationDocumentService : IDocumentService<ReconciliationReport>
    {
        private readonly ITemplateRepository templateRepository;
        public ReconciliationDocumentService(ITemplateRepository templateRepository)
        {
            this.templateRepository = templateRepository;
        }
        public async Task<string> BuildHtml(ReconciliationReport report)
        {
            var html = await templateRepository.GetTemplateAsync("ReconciliationReport.html");
            var css = await templateRepository.GetTemplateAsync("style.css");

            html = html.Replace("{{Styles}}", $"<style>{css}</style>");

            html = html.Replace("{{ToDate}}", report.ToDate.ToString("d"));
            html = html.Replace("{{OurCompany}}", report.OurCompany.Name);
            html = html.Replace("{{InnOurCompany}}", report.OurCompany.Inn);
            html = html.Replace("{{Counterparty}}", report.Counterparty.Name);
            html = html.Replace("{{InnCounterparty}}", report.Counterparty.Inn);
            if (report.Contract != null)
            {
                html = html.Replace("{{ContractString}}", $"по договору {report.Contract.Number} от {report.Contract.Date:d}");
            }
            else
            {
                html = html.Replace("<p>{{ContractString}}</p>", "");
            }
            html = html.Replace("{{OrganizationHeader}}", $"{report.OurCompany.Position} {report.OurCompany.Name} {report.OurCompany.DirectorFullName}".Trim());
            html = html.Replace("{{CounterpartyHeader}}", $"{report.Counterparty.Position} {report.Counterparty.Name} {report.Counterparty.DirectorFullName}".Trim());
            html = html.Replace("{{PositionOurCompany}}", report.OurCompany.Position);
            html = html.Replace("{{DirectorOurCompany}}", StringFormattingHelper.GenerateFIO(report.OurCompany));
            html = html.Replace("{{PositionCounterparty}}", report.Counterparty.Position);
            html = html.Replace("{{DirectorCounterparty}}", StringFormattingHelper.GenerateFIO(report.Counterparty));
            html = html.Replace("{{FinalDebtStatement}}", GetFinalDebtStatement(report));
            html = html.Replace("{{TableRows}}", GetTableRows(report));
            return html;
        }
        private string GetFinalDebtStatement(ReconciliationReport report)
        {
            if (report.ClosingBalance > 0)
                return $"на {report.ToDate:d} задолженность в пользу {report.OurCompany.Name} {report.ClosingBalance:N2} руб. ({RublesSpellingConverter.Convert(report.ClosingBalance)})";
            else if (report.ClosingBalance < 0)
                return $"на {report.ToDate:d} задолженность в пользу {report.Counterparty.Name} {Math.Abs(report.ClosingBalance):N2} руб. ({RublesSpellingConverter.Convert(Math.Abs(report.ClosingBalance))})";
            else
                return $"на {report.ToDate:d} задолженность отсутствует";
        }
        private string GetTableRows(ReconciliationReport report)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetOpeningBalanceString(report.OpeningBalance));

            foreach (var item in report.ReconciliationEntries)
            {
                sb.Append($"<tr><td>{item.Date:d}</td><td>{item.DocumentName}</td><td>{item.Debit:N2}</td><td>{item.Credit:N2}</td><td>{item.Date:d}</td><td>{item.DocumentName}</td><td>{item.Credit:N2}</td><td>{item.Debit:N2}</td></tr>");
            }
            sb.Append($@"
                    <tr class=""balance-row"">
                        <td colspan=""2"">Обороты за период</td>
                        <td>{report.ChargedTotal:N2}</td>
                        <td>{report.PaymentsTotal:N2}</td>
                        <td colspan=""2"">Обороты за период</td>
                        <td>{report.PaymentsTotal:N2}</td>
                        <td>{report.ChargedTotal:N2}</td>
                    </tr>");
            sb.Append(GetClosingBalanceString(report.ClosingBalance));
            return sb.ToString();
        }
        private string GetOpeningBalanceString(decimal openingBalance)
        {
            if (openingBalance > 0)
                return $@"
                    <tr class=""balance-row"">
                        <td colspan=""2"">Сальдо начальное</td>
                        <td>{openingBalance:N2}</td>
                        <td></td>
                        <td colspan=""2"">Сальдо начальное</td>
                        <td></td>
                        <td>{openingBalance:N2}</td>
                    </tr>";
            else if (openingBalance < 0)
                return $@"<tr class=""balance-row""><td colspan=""2"">Сальдо начальное</td><td></td><td>{Math.Abs(openingBalance):N2}</td><td colspan=""2"">Сальдо начальное</td><td>{Math.Abs(openingBalance):N2}</td><td></td></tr>";
            else
                return $@"<tr class=""balance-row""><td colspan=""2"">Сальдо начальное</td><td></td><td></td><td colspan=""2"">Сальдо начальное</td><td></td><td></td></tr>";
        }
        private string GetClosingBalanceString(decimal closingBalance)
        {
            if (closingBalance > 0)
                return $@"
                    <tr class=""balance-row"">
                        <td colspan=""2"">Сальдо конечное</td>
                        <td>{closingBalance:N2}</td>
                        <td></td>
                        <td colspan=""2"">Сальдо конечное</td>
                        <td></td>
                        <td>{closingBalance:N2}</td>
                    </tr>";
            else if (closingBalance < 0)
                return $@"<tr class=""balance-row""><td colspan=""2"">Сальдо конечное</td><td></td><td>{Math.Abs(closingBalance):N2}</td><td colspan=""2"">Сальдо конечное</td><td>{Math.Abs(closingBalance):N2}</td><td></td></tr>";
            else
                return $@"<tr class=""balance-row""><td colspan=""2"">Сальдо конечное</td><td></td><td></td><td colspan=""2"">Сальдо конечное</td><td></td><td></td></tr>";
        }
    }
}
