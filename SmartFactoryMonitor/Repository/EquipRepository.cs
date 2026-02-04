using SmartFactoryMonitor.Model;
using SmartFactoryMonitor.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFactoryMonitor.Repository
{
   public class EquipRepository
    {
        private readonly OracleService _db;

        public ObservableCollection<Equipment> Equipments { get; } = new ObservableCollection<Equipment>();

        public EquipRepository(OracleService dbService)
        {
            _db = dbService;
        }

        public async Task LoadAll()
        {
            string sql = "select * from EQUIPMENT";
            DataTable dt = await _db.SelectQuery(sql);

            foreach(DataRow row in dt.Rows)
            {
                string id = row["EQUIP_ID"].ToString();
                var existingEquip = Equipments.FirstOrDefault(e => e.EquipId == id);

                if (existingEquip != null)
                {
                    // 현재 DB 값으로 최신화
                    existingEquip.EquipName = row.AsString("EQUIP_NAME");
                    existingEquip.IpAddress = row["IP_ADDRESS"].ToString();
                    existingEquip.Port = Convert.ToInt32(row["PORT"]);
                    existingEquip.MinTemp = Convert.ToDouble(row["MIN_TEMP"]);
                    existingEquip.MaxTemp = Convert.ToDouble(row["MAX_TEMP"]);
                    existingEquip.Location = row["LOCATION"].ToString();
                    existingEquip.IsActive = row["IS_ACTIVE"].ToString();
                    existingEquip.CreateDate = row["CREATE_DATE"].ToString();
                }
                else
                {
                    // 추가된 Equipment 반영
                    Equipments.Add(new Equipment
                    {
                        EquipId = id,
                        EquipName = row["EQUIP_NAME"].ToString(),
                        IpAddress = row["IP_ADDRESS"].ToString(),
                        Port = Convert.ToInt32(row["PORT"]),
                        MinTemp = Convert.ToDouble(row["MIN_TEMP"]),
                        MaxTemp = Convert.ToDouble(row["MAX_TEMP"]),
                        Location = row["LOCATION"].ToString(),
                        IsActive = row["IS_ACTIVE"].ToString(),
                        CreateDate = row["CREATE_DATE"].ToString(),
                });
                }
            }

            // 삭제된 Equipment 반영
            var dbIds = dt.AsEnumerable()
                .Select(r => r["EQUIP_ID"])
                .ToList();
            var removedEquip = Equipments.Where(e => !dbIds.Contains(e.EquipId)).ToList();
            foreach (var equip in removedEquip) { Equipments.Remove(equip); }
        }
    }
}
