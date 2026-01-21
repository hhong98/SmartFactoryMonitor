using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFactoryMonitor.Services
{
    public class MonitoringService 
    {
        private Random _random = new Random();

        // 실시간 데이터를 랜덤으로 생성해서 반환
        // 실제로는 PCL이나 DB에서 데이터를 가져오는 역할
        public double GetLiveTemp() => Math.Round(_random.NextDouble() * 100, 1);
    }
}
