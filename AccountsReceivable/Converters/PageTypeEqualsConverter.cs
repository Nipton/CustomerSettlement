using AccountsReceivable.Models.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace AccountsReceivable.Converters
{
    public class PageTypeToBooleanConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is not PageType currentPage || values[1] == null)
                return false;
            if (values[1] is PageType page)
                return currentPage == page;
            if (values[1] is IEnumerable<PageType> pages)
                return pages.Contains(currentPage);

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
