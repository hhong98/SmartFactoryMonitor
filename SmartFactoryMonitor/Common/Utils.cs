using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFactoryMonitor.Common
{
    public class Utils
    {
        public static string ConvertToStatus(string option)
            => option switch
            {
                "정상" => "STABLE",
                "주의" => "WARN",
                "위험" => "ERROR",
                "연결 끊김" => "NO DATA",
                _ => string.Empty
            };

        public static string ConvertToActive(string option)
            => option switch
            {
                "사용" => "Y",
                "미사용" => "N",
                _ => string.Empty
            };
    }
}