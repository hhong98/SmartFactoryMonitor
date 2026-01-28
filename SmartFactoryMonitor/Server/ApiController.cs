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

        // /api/equipments/detail?equipid=id
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

        public async Task<ApiResult> SimulatorEquipments(ApiRequest req)
        {
            if (req.Method != "GET")
                return ApiResult.BadRequest("method_not_allowed");

            // 필요 최소 컬럼만
            string sql = @"
                SELECT EQUIP_ID, EQUIP_NAME, MIN_TEMP, MAX_TEMP, LOCATION, IS_ACTIVE
                FROM HHONG.EQUIPMENT
                WHERE IS_ACTIVE = 'Y'
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

            // logTime 파싱은 해두되, DB에는 일단 SYSTIMESTAMP로 박아도 됨(안정적)
            // (원하면 다음 단계에서 파라미터 바인딩으로 dto.LogTime을 그대로 넣게 개선 가능)
            if (!DateTime.TryParse(dto.LogTime, null, DateTimeStyles.RoundtripKind, out var parsedLogTime))
                parsedLogTime = DateTime.Now;

            // 설비 상태 검증
            var status = (dto.Status ?? "STABLE").ToUpperInvariant();
            var allowed = new HashSet<string> { "STABLE", "WARN", "ERROR", "NO_DATA" };
            if (!allowed.Contains(status))
                return ApiResult.BadRequest("invalid_status");

            // 설비 온도 INSERT
            string sql = $@"
                INSERT INTO HHONG.SENSOR_LOG (EQUIP_ID, TEMPERATURE, LOG_TIME, STATUS)
                VALUES ('{EscapeSql(dto.EquipId)}',
                        {dto.Temperature.Value.ToString(CultureInfo.InvariantCulture)},
                        SYSTIMESTAMP,
                        '{EscapeSql(status)}')
                ";

            // 네 OracleService에서 DML은 ExecuteQuery가 담당합니다. :contentReference[oaicite:4]{index=4}
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