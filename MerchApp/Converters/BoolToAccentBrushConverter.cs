using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace MerchApp.Converters
{
    public class BoolToAccentBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => value is true
                ? new SolidColorBrush(Windows.UI.Color.FromArgb(255, 208, 109, 31))  // #d06d1f
                : new SolidColorBrush(Windows.UI.Color.FromArgb(40, 255, 255, 255)); // subtle border

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }
}
