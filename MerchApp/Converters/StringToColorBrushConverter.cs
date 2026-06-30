using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI;

namespace MerchApp.Converters
{
    public class StringToColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string hex)
            {
                try
                {
                    hex = hex.TrimStart('#');
                    var color = Color.FromArgb(
                        255,
                        System.Convert.ToByte(hex[0..2], 16),
                        System.Convert.ToByte(hex[2..4], 16),
                        System.Convert.ToByte(hex[4..6], 16));
                    return new SolidColorBrush(color);
                }
                catch { }
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }
}
