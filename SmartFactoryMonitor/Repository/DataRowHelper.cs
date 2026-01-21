using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFactoryMonitor.Repository
{
    public static class Util
    {
        public static string AsString(this DataRow row, string columnName)
        {
            if (row.Table.Columns.Contains(columnName) && row.IsNull(columnName) == false)
            {
                return row[columnName].ToString().Trim();
            }
            else
            {
                return string.Empty;
            }
        }
    }
}

