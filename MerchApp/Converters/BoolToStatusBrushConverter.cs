using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace MerchApp.Converters
{
    public class BoolToStatusBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool b)
                return new SolidColorBrush(b
                    ? Windows.UI.Color.FromArgb(255, 92, 195, 92)    // #5cc35c green
                    : Windows.UI.Color.FromArgb(255, 255, 112, 100)); // #ff7064 red

            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }
}
