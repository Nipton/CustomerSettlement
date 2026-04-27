using System;
using System.Globalization;

namespace AccountsReceivable.Helpers
{
    public static class DecimalHelper
    {
        public static decimal SafeParse(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;
            value = value.Replace(',', '.');
            return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? result : 0;
        }
        public static decimal SafeMultiply(decimal? a, decimal? b)
        {
            if(!a.HasValue || !b.HasValue) return 0;
            try
            {
                return a.Value * b.Value;
            }
            catch (OverflowException)
            {
                return 0;
            }
        }
        public static decimal SafeAdd(decimal a, decimal b)
        {
            try
            {
                return a + b;
            }
            catch (OverflowException)
            {
                return 0;
            }
        }
    }
}
