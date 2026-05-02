using AccountsReceivable.Models;
using System;
using System.Text.RegularExpressions;

namespace AccountsReceivable.Helpers
{
    public static class FormationStrings
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
    }
}
