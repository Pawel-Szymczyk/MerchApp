using Microsoft.UI.Xaml.Data;
using System;

namespace MerchApp.Converters
{
    public class BoolToConnectionTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => value is true ? "Connected to SharePoint" : "Unable to connect to SharePoint";

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }
}
