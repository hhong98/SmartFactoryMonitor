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
    public class StatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string status))
                return Brushes.Transparent;

            string brushKey = status switch
            {
                "STABLE" => "StatusStableBrush",
                "WARN" => "StatusWarnBrush",
                "ERROR" => "StatusErrorBrush",
                "NO DATA" => "StatusNoDataBrush",
                _ => "StatusInactiveBrush"
            };

            return Application.Current.TryFindResource(brushKey) ?? Brushes.Magenta;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}