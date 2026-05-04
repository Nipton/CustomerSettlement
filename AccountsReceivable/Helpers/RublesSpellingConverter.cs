using System;
using System.Collections.Generic;

public static class RublesSpellingConverter
{
    private static readonly string[] UnitsMale = { "", "один", "два", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять" };
    private static readonly string[] UnitsFemale = { "", "одна", "две", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять" };
    private static readonly string[] Teens = { "десять", "одиннадцать", "двенадцать", "тринадцать", "четырнадцать", "пятнадцать", "шестнадцать", "семнадцать", "восемнадцать", "девятнадцать" };
    private static readonly string[] Tens = { "", "", "двадцать", "тридцать", "сорок", "пятьдесят", "шестьдесят", "семьдесят", "восемьдесят", "девяносто" };
    private static readonly string[] Hundreds = { "", "сто", "двести", "триста", "четыреста", "пятьсот", "шестьсот", "семьсот", "восемьсот", "девятьсот" };

    private static readonly string[] ThousandsForms = { "тысяча", "тысячи", "тысяч" };
    private static readonly string[] MillionsForms = { "миллион", "миллиона", "миллионов" };
    private static readonly string[] BillionsForms = { "миллиард", "миллиарда", "миллиардов" };

    private static readonly string[] RubleForms = { "рубль", "рубля", "рублей" };
    private static readonly string[] KopeckForms = { "копейка", "копейки", "копеек" };

    public static string Convert(decimal sum)
    {
        if (sum < 0)
            throw new ArgumentException("Отрицательные числа не поддерживаются", nameof(sum));

        sum = Math.Round(sum, 2, MidpointRounding.AwayFromZero);
        
        long integerPart = (long)sum;
        int fractionalPart = (int)((sum - integerPart) * 100 + 0.001m); 

        // Формируем результат
        string rubles = NumberToWords(integerPart, false) + " " + GetCurrencyForm(integerPart, RubleForms);
        string kopecks = fractionalPart.ToString("D2") + " " + GetCurrencyForm(fractionalPart, KopeckForms);

        return rubles + " " + kopecks;
    }

    private static string NumberToWords(long number, bool isFemale)
    {
        if (number == 0)
            return "ноль";

        List<string> parts = new List<string>();

        if (number >= 1000000000)
        {
            long billions = number / 1000000000;
            parts.Add(ConvertHundreds(billions, false) + " " + GetCurrencyForm(billions, BillionsForms));
            number %= 1000000000;
        }

        if (number >= 1000000)
        {
            long millions = number / 1000000;
            parts.Add(ConvertHundreds(millions, false) + " " + GetCurrencyForm(millions, MillionsForms));
            number %= 1000000;
        }

        if (number >= 1000)
        {
            long thousands = number / 1000;
            parts.Add(ConvertHundreds(thousands, true) + " " + GetCurrencyForm(thousands, ThousandsForms));
            number %= 1000;
        }

        if (number > 0)
        {
            parts.Add(ConvertHundreds(number, isFemale));
        }

        return string.Join(" ", parts).Trim();
    }

    private static string ConvertHundreds(long number, bool isFemale)
    {
        if (number < 0 || number > 999)
            throw new ArgumentException("Число должно быть в диапазоне 0-999", nameof(number));

        if (number == 0)
            return "";

        List<string> parts = new List<string>();

        int hundreds = (int)(number / 100);
        if (hundreds > 0)
        {
            parts.Add(Hundreds[hundreds]);
            number %= 100;
        }

        if (number >= 10 && number <= 19)
        {
            parts.Add(Teens[number - 10]);
        }
        else
        {
            int tens = (int)(number / 10);
            if (tens > 0)
            {
                parts.Add(Tens[tens]);
            }

            int units = (int)(number % 10);
            if (units > 0)
            {
                parts.Add(isFemale ? UnitsFemale[units] : UnitsMale[units]);
            }
        }

        return string.Join(" ", parts).Trim();
    }

    private static string GetCurrencyForm(long number, string[] forms)
    {
        number = Math.Abs(number);
        long lastDigit = number % 10;
        long lastTwoDigits = number % 100;

        if (lastTwoDigits >= 11 && lastTwoDigits <= 19)
            return forms[2];

        if (lastDigit == 1)
            return forms[0];

        if (lastDigit >= 2 && lastDigit <= 4)
            return forms[1];

        return forms[2];
    }
}