using Newtonsoft.Json;
using SmartFactoryMonitor.Model;
using SmartFactoryMonitor.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        // =========================
        // GET /api/equipments
        // =========================
        public async Task<ApiResult> ActiveEquipments(ApiRequest req)
        {
            switch (req.Method)
            {
                case "GET":
                    {
                        //string sql = "select * from EQUIPMENT";
                        string sql = @"select EQUIP_ID, EQUIP_NAME, MIN_TEMP, MAX_TEMP, LOCATION from EQUIPMENT where IS_ACTIVE='Y'";
                        var dt = await db.SelectQuery(sql).ConfigureAwait(false);

                        var rows = DataTableMapper.ToRows(dt);
                        return ApiResult.Ok(new { success = true, data = rows });
                    }
                default: break;
            }

            return null;
        }

        // =========================
        // GET /api/equipments/meta
        // =========================
        public async Task<ApiResult> ActiveEquipmentsMeta(ApiRequest req)
        {
            switch (req.Method)
            {
                case "GET":
                    {
                        //string sql = "select * from EQUIPMENT";
                        string sql = @"
                            SELECT
                                E.EQUIP_ID,
                                E.EQUIP_NAME,
                                E.LOCATION,
                                L.TEMPERATURE,
                                L.STATUS
                            FROM EQUIPMENT E
                            LEFT JOIN (
                                SELECT EQUIP_ID, TEMPERATURE, STATUS
                                FROM (
                                    SELECT
                                        SL.EQUIP_ID,
                                        SL.TEMPERATURE,
                                        SL.STATUS,
                                        ROW_NUMBER() OVER (
                                            PARTITION BY SL.EQUIP_ID
                                            ORDER BY SL.LOG_TIME DESC
                                        ) AS RN
                                    FROM SENSOR_LOG SL
                                )
                                WHERE RN = 1
                            ) L
                                ON L.EQUIP_ID = E.EQUIP_ID
                            WHERE E.IS_ACTIVE = 'Y'
                            ORDER BY E.EQUIP_ID
                            ";

                        var dt = await db.SelectQuery(sql).ConfigureAwait(false);

                        var rows = DataTableMapper.ToRows(dt);
                        return ApiResult.Ok(new { success = true, data = rows });
                    }
                default: break;
            }

            return null;
        }

        // =========================
        // GET /api/android/equipments/detail?equipid=id
        // =========================
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

        #region Simulator

        // =========================
        // GET /api/simulator/equipments
        // =========================
        public async Task<ApiResult> SimulatorEquipments(ApiRequest req)
        {
            if (req.Method != "GET")
                return ApiResult.BadRequest("method_not_allowed");

            // 필요 최소 컬럼만
            string sql = @"
                select EQUIP_ID, EQUIP_NAME, MIN_TEMP, MAX_TEMP, LOCATION, IS_ACTIVE
                from EQUIPMENT
                where IS_ACTIVE = 'Y'
                ";

            var dt = await db.SelectQuery(sql).ConfigureAwait(false);
            var rows = DataTableMapper.ToRows(dt);

            var result = rows
                .Select(r => new
                {
                    equipId = r.TryGetValue("EQUIP_ID", out var id) ? id?.ToString() : null,
                    equipName = r.TryGetValue("EQUIP_NAME", out var name) ? name?.ToString() : null,
                    minTemp = ToDoubleSafe(r, "MIN_TEMP"),
                    maxTemp = ToDoubleSafe(r, "MAX_TEMP"),
                    location = r.TryGetValue("LOCATION", out var loc) ? loc?.ToString() : null,
                    isActive = r.TryGetValue("IS_ACTIVE", out var act) ? act?.ToString() : null,
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.equipId))
                .ToList();

            return ApiResult.Ok(result);
        }

        // =========================
        // POST /api/simulator/temperature
        // body: { equipId, temperature, logTime }
        // =========================
        public async Task<ApiResult> SimulatorTemperature(ApiRequest req)
        {
            if (req.Method != "POST")
                return ApiResult.BadRequest("method_not_allowed");

            if (string.IsNullOrWhiteSpace(req.Body))
                return ApiResult.BadRequest("empty_body");

            SimulatorTempDto dto;
            try
            {
                dto = JsonConvert.DeserializeObject<SimulatorTempDto>(req.Body);
            }
            catch
            {
                return ApiResult.BadRequest("invalid_json");
            }

            if (dto == null || string.IsNullOrWhiteSpace(dto.EquipId))
                return ApiResult.BadRequest("equipId_required");

            if (!DateTime.TryParse(dto.LogTime, null, DateTimeStyles.RoundtripKind, out var parsedLogTime))
                parsedLogTime = DateTime.Now;

            // 설비 상태 검증
            var status = (dto.Status ?? "STABLE").ToUpperInvariant();
            var allowed = new HashSet<string> { "STABLE", "WARN", "ERROR", "NO DATA" };
            if (!allowed.Contains(status))
                return ApiResult.BadRequest("invalid_status");

            // 설비 온도 INSERT
            string sql = $@"
                insert into SENSOR_LOG (EQUIP_ID, TEMPERATURE, LOG_TIME, STATUS)
                values ('{EscapeSql(dto.EquipId)}',
                        {dto.Temperature.Value.ToString(CultureInfo.InvariantCulture)},
                        SYSTIMESTAMP,
                        '{EscapeSql(status)}')
                ";

            var execResult = await db.ExecuteQuery(sql).ConfigureAwait(false);

            // ExecuteQuery는 성공 시 "영향받은 row 수" 문자열, 실패 시 "에러 메시지" 문자열
            if (!int.TryParse(execResult, out var affected) || affected <= 0)
            {
                return ApiResult.ServerError(execResult);
            }

            return ApiResult.Created(new
            {
                ok = true,
                equipId = dto.EquipId,
                temperature = dto.Temperature,
                logTime = parsedLogTime.ToString("o"),
                status = status,
            });
        }

        private static double ToDoubleSafe(Dictionary<string, object> r, string key)
        {
            if (!r.TryGetValue(key, out var v) || v == null) return 0.0;
            if (v is double d) return d;
            if (v is decimal m) return (double)m;
            if (double.TryParse(Convert.ToString(v, CultureInfo.InvariantCulture), out var parsed)) return parsed;
            return 0.0;
        }

        private static string EscapeSql(string s) => (s ?? "").Replace("'", "''");

        #endregion Simulator
    }
}