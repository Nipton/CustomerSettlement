using AccountsReceivable.Models;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace AccountsReceivable.Helpers
{
    public static class StringFormattingHelper
    {
        public static string GenerateFIO(Company company)
        {
            if (company?.DirectorFullName == null)
                return "";
            string[] fioArr = company.DirectorFullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (fioArr.Length == 3 && fioArr[1].Length > 0 && fioArr[2].Length > 0)
                return $"{fioArr[1][0]}.{fioArr[2][0]}. {fioArr[0]}";
            if (fioArr.Length == 2 && fioArr[1].Length > 0)
                return $"{fioArr[1][0]}. {fioArr[0]}";
            if (fioArr.Length == 1)
                return fioArr[0];
            return "";
        }
        public static string GetTitleFromHtml(string html)
        {
            var match = Regex.Match(html, @"<title>(.*?)</title>", RegexOptions.IgnoreCase);
            if (match.Success)
                return match.Groups[1].Value;

            match = Regex.Match(html, @"<h1>(.*?)</h1>", RegexOptions.IgnoreCase);
            if (match.Success)
                return match.Groups[1].Value;

            return "Отчёт";
        }
        public static string GenerateCompanyInfo(Company company)
        {
            if (company == null) return string.Empty;

            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(company.Name))
                sb.Append(company.Name);
            if (!string.IsNullOrEmpty(company.Inn))
                sb.Append($", ИНН {company.Inn}");

            if (!string.IsNullOrEmpty(company.Kpp))
                sb.Append($", КПП {company.Kpp}");

            if (!string.IsNullOrEmpty(company.LegalAddress))
                sb.Append($", {company.LegalAddress}");
            if (!string.IsNullOrEmpty(company.Phone))
                sb.Append($", тел: {company.Phone}");

            return sb.ToString();
        }
    }
}
