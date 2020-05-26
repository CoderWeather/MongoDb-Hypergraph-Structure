using System;
using System.Globalization;
using System.Windows.Data;

namespace HyperGraphSharp.Converters
{
    public class EqualityToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => 
            Equals(value, parameter);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => 
            (bool) value ? parameter : Binding.DoNothing;
    }
}