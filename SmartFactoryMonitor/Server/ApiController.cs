using SmartFactoryMonitor.Model;
using SmartFactoryMonitor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFactoryMonitor.Server
{
    public class ApiController
    {
        private readonly OracleService db;

        public ApiController(OracleService dbService)
        {
            db = dbService ?? throw new ArgumentNullException(nameof(db));
        }

        //public Task<ApiResult> Health(ApiRequest req)
        //    => Task.FromResult(ApiResult.Ok(new { ok = true, time = DateTime.Now }));

        // /api/equipments
        public async Task<ApiResult> Equipments(ApiRequest req)
        {
            switch (req.Method)
            {
                case "GET":
                    {
                        //string sql = "select * from EQUIPMENT";
                        string sql = @"select * from EQUIPMENT where IS_ACTIVE='Y'";
                        var dt = await db.SelectQuery(sql).ConfigureAwait(false);

                        var rows = DataTableMapper.ToRows(dt);
                        return ApiResult.Ok(new { success = true, data = rows });
                    }
                default: break;
            }

            return null;
        }

        public async Task<ApiResult> EquipmentDetail(ApiRequest req)
        {
            string id = req.Query["id"];

            switch (req.Method)
            {
                case "GET":
                    {
                        string sql = @$"select EQUIP_ID, EQUIP_NAME, IP_ADDRESS, PORT, MIN_TEMP, MAX_TEMP, LOCATION from EQUIPMENT where EQUIP_ID='{id}'";
                        var dt = await db.SelectQuery(sql).ConfigureAwait(false);

                        var rows = DataTableMapper.ToRows(dt);
                        return ApiResult.Ok(new { success = true, data = rows.FirstOrDefault() });
                    }
                default: break;
            }

            return null;
        }
    }
}