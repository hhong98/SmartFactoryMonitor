using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFactoryMonitor.Model
{
    public class SimulatorTempDto
    {
        [JsonProperty("equipId")] public string EquipId { get; set; }
        [JsonProperty("temperature")] public double? Temperature { get; set; }
        [JsonProperty("logTime")] public string LogTime { get; set; }
        [JsonProperty("status")] public string Status { get; set; }
    }
}