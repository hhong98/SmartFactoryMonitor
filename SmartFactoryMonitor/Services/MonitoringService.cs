using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFactoryMonitor.Services
{
    public class MonitoringService
    {
        private OracleService db;
        private Random _random = new Random();

        public MonitoringService(OracleService db)
        {
            this.db = db;
        }

        public async Task<List<(string equipId, double temperature, string status, DateTime logTime)>> GetLatestTemp(List<string> equipIds)
        {
            var ids = string.Join(", ", equipIds.Select(id => $"'{id}'"));
            var sql = @$"
                select EQUIP_ID, TEMPERATURE, STATUS, LOG_TIME
                from (
                    select
                        SL.EQUIP_ID,
                        SL.TEMPERATURE,
                        SL.STATUS,
                        SL.LOG_TIME,
                        ROW_NUMBER() OVER (PARTITION BY SL.EQUIP_ID ORDER BY SL.LOG_TIME DESC) AS RN
                    from SENSOR_LOG SL
                    where SL.EQUIP_ID in ({ids})
                )
                where RN = 1
            ";

            var dt = await db.SelectQuery(sql).ConfigureAwait(false);
            if (dt is null || dt.Rows.Count is 0) return new List<(string equipId, double temperature, string status, DateTime logTime)>();

            return dt.AsEnumerable().Select(row => (
                 equipId: row.Field<string>("EQUIP_ID"),
                 temperature: Convert.ToDouble(row.Field<object>("TEMPERATURE")), // DB 타입에 따라 안전하게 변환
                 status: row.Field<string>("STATUS"),
                 logTime: Convert.ToDateTime(row.Field<object>("LOG_TIME"))
             )).ToList();
        }
    }
}