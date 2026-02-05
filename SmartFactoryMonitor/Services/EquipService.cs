using SmartFactoryMonitor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFactoryMonitor.Services
{
    public class EquipService
    {
        private readonly OracleService _db;

        public EquipService(OracleService oracleService)
        {
            _db = oracleService;
        }

        public async Task<DbResult> Add(Equipment.Add_DTO equip)
        {
            string sql = $@"Insert Into EQUIPMENT (EQUIP_NAME, IP_ADDRESS, PORT, MIN_TEMP, MAX_TEMP, LOCATION)
                            Values('{equip.EquipName}', '{equip.IpAddress}',
                            {equip.Port}, {equip.MinTemp}, {equip.MaxTemp},
                            '{equip.Location}')";

            string result = await _db.ExecuteQuery(sql);
            return int.TryParse(result, out int rows)
                ? new DbResult { IsSuccess = true, AffectedRows = rows }
                : new DbResult { IsSuccess = false, Message = result };
        }

        public async Task<DbResult> UpdateChangedColumns(string equipId, Dictionary<string, object> changedColumns)
        {
            var sql = "Update EQUIPMENT Set ";
            var updateStatements = new List<string>();

            foreach (KeyValuePair<string, object> column in changedColumns)
            {
                if (column.Value is null) continue;

                string valueStr = (column.Value is string)
                      ? $"'{column.Value}'"
                      : column.Value?.ToString();

                updateStatements.Add($"{column.Key} = {valueStr}");
            }
            if (updateStatements.Count is 0) return new DbResult { IsSuccess = true, AffectedRows = 0 };

            sql += $"{string.Join(", ", updateStatements)} Where EQUIP_ID = '{equipId}'";

            var result = await _db.ExecuteQuery(sql);

            return int.TryParse(result, out int rows)
               ? new DbResult { IsSuccess = true, AffectedRows = rows }
               : new DbResult { IsSuccess = false, Message = result };
        }

        public async Task<DbResult> UpdateAllColumns(Equipment equip)
        {
            string sql = $@"Update EQUIPMENT
                        Set EQUIP_NAME = '{equip.EquipName}',
                            IP_ADDRESS = '{equip.IpAddress}',
                            PORT = {equip.Port},
                            MIN_TEMP = {equip.MinTemp},
                            MAX_TEMP = {equip.MaxTemp},
                            LOCATION = '{equip.Location}',
                            IS_ACTIVE = '{equip.IsActive}'
                        Where EQUIP_ID = '{equip.EquipId}'";

            var result = await _db.ExecuteQuery(sql);

            return int.TryParse(result, out int rows)
               ? new DbResult { IsSuccess = true, AffectedRows = rows }
               : new DbResult { IsSuccess = false, Message = result };
        }

        public async Task<DbResult> Delete(List<string> ids)
        {
            string sql = $"DELETE FROM EQUIPMENT WHERE EQUIP_ID " +
                    $"IN ({ string.Join(", ", ids.Select(id => $"'{id}'"))})";

            string result = await _db.ExecuteQuery(sql);

            return int.TryParse(result, out int rows)
               ? new DbResult { IsSuccess = true, AffectedRows = rows }
               : new DbResult { IsSuccess = false, Message = result };
        }
    }
}