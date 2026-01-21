using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFactoryMonitor.Model
{
    public class DbResult
    {
        public bool IsSuccess { get; set; }
        public int AffectedRows { get; set; }
        public string Message { get; set; }
    }
}
