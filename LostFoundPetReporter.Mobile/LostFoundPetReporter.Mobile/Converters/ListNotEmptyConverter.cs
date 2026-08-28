using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace LostFoundPetReporter.Mobile.Converters
{
    public class ListNotEmptyConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is ICollection { Count: > 0 } || value is IEnumerable en && en.Cast<object>().Any();
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
