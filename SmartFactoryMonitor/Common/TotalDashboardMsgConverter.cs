using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SmartFactoryMonitor.Common
{
    public class TotalDashboardMsgConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is null || values.Length < 3) return "";

            return values[0] is bool isPanelOpened &&
                values[1] is int activeCount &&
                values[2] is int totalCount
                ? $"{activeCount}"
                //? isPanelOpened ? $"{activeCount}" : $"{activeCount}/{totalCount}"
                : "-";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}