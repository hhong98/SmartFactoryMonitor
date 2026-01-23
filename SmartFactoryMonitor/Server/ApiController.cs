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

        // GET /api/equipments
        public async Task<ApiResult> GetEquipments(ApiRequest req)
        {
            switch (req.Method)
            {
                case "GET":
                    {
                        string sql = "SELECT * FROM EQUIPMENT";
                        var dt = await db.SelectQuery(sql).ConfigureAwait(false);

                        var rows = DataTableMapper.ToRows(dt);
                        return ApiResult.Ok(new { success = true, data = rows });
                    }
                default: break;
            }

            return null;
        }
    }
}