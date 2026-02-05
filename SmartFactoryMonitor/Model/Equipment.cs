using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SmartFactoryMonitor.Model
{
    public class Equipment : ObservableModelBase
    {
        #region DB Columns

        [Column("EQUIP_ID")]
        public string EquipId { get; set; } // C#의 자동 구현 Property

        private string _equipName; // 필드(실제 데이터가 메모리에 저장되는 저장소), 자동 구현 Property 사용 x

        [Column("EQUIP_NAME")]
        public string EquipName { get => _equipName; set => SetProperty(ref _equipName, value); } // 프로퍼티(외부에서 저장소에 접근하는 경로)

        private string _ipAddress;

        [Column("IP_ADDRESS")]
        public string IpAddress { get => _ipAddress; set => SetProperty(ref _ipAddress, value); }

        private int _port;

        [Column("PORT")]
        public int Port { get => _port; set => SetProperty(ref _port, value); }

        private double _minTemp;

        [Column("MIN_TEMP")]
        public double MinTemp { get => _minTemp; set => SetProperty(ref _minTemp, value); }

        private double _maxTemp;

        [Column("MAX_TEMP")]
        public double MaxTemp { get => _maxTemp; set => SetProperty(ref _maxTemp, value); }

        private string _location;

        [Column("LOCATION")]
        public string Location { get => _location; set => SetProperty(ref _location, value); }

        private string _isActive;

        [Column("IS_ACTIVE")]
        public string IsActive { get => _isActive; set => SetProperty(ref _isActive, value); }

        private string _createDate;

        [Column("CREATE_DATE")]
        public string CreateDate { get => _createDate; set => SetProperty(ref _createDate, value); }

        #endregion DB Columns

        private double _currentTemp = 0d;

        public double CurrentTemp
        {
            get => _currentTemp;
            set => SetProperty(ref _currentTemp, value);
        }

        private string _status = "NO DATA";
        public string Status { get => _status; set => SetProperty(ref _status, value); }

        private bool _isChecked = false;
        public bool IsChecked { get => _isChecked; set => SetProperty(ref _isChecked, value); }

        public static Dictionary<string, object> GetChangedColumns(Equipment original, Equipment current)
        {
            var changedColumns = new Dictionary<string, object>();

            var properties = typeof(Equipment).GetProperties();

            foreach (var prop in properties)
            {
                if (prop.Name is "EquipId") continue;

                if (prop.Name is "Location") continue; // [TEMP] Panel에 Location 추가하면 지우기

                if (!(prop.GetCustomAttribute<ColumnAttribute>() is ColumnAttribute columnAttr))
                    continue;

                object oldVal = prop.GetValue(original);
                object newVal = prop.GetValue(current);

                if (!Equals(oldVal, newVal))
                    changedColumns.Add(columnAttr.Name, newVal);
            }

            return changedColumns;
        }

        #region DTO

        /* 설비 추가 DTO */

        public class Add_DTO
        {
            public string EquipName { get; set; }
            public string IpAddress { get; set; }
            public int Port { get; set; }
            public double MinTemp { get; set; }
            public double MaxTemp { get; set; }
            public string Location { get; set; }
        }

        /* 설비 수정 DTO */

        public class Update_DTO
        {
            public string EquipName { get; set; }
            public string IpAddress { get; set; }
            public int Port { get; set; }
            public double MinTemp { get; set; }
            public double MaxTemp { get; set; }
            public string Location { get; set; }

            // Equipment to Update_DTO
            public static Update_DTO Convert(Equipment equip)
                => new Update_DTO
                {
                    EquipName = equip.EquipName,
                    IpAddress = equip.IpAddress,
                    Port = equip.Port,
                    MinTemp = equip.MinTemp,
                    MaxTemp = equip.MaxTemp,
                    Location = equip.Location,
                };
        }

        #endregion DTO
    }
}