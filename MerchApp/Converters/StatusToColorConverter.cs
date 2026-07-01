using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace MerchApp.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is RentalStatus status)
            {
                return status switch
                {
                    RentalStatus.Pending => new SolidColorBrush(Colors.Orange),
                    RentalStatus.Approved => new SolidColorBrush(Colors.Green),
                    RentalStatus.Rejected => new SolidColorBrush(Colors.Red),
                    RentalStatus.Returned => new SolidColorBrush(Colors.Gray),
                    _ => new SolidColorBrush(Colors.Transparent)
                };
            }
            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }
}
