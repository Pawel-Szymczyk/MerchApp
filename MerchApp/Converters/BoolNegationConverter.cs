using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MerchApp.Converters
{
    public class BoolNegationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => value is bool b && !b;

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => value is bool b && !b;
    }
}
