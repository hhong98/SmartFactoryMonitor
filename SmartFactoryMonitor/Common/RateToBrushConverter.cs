using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SmartFactoryMonitor.Common
{
    public class RateToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string brushKey = string.Empty;

            if (value is double rate)
            {
                if (rate >= 80) brushKey = "StatusStableBrush";
                else if (rate >= 30) brushKey = "StatusWarnBrush";
                else brushKey = "StatusErrorBrush";
            }

            return Application.Current.TryFindResource(brushKey) ?? Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}