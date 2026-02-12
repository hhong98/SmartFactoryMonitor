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
        public string EquipId { get; set; } // C#의 자동 구현 Property

        private string equipName; // 필드(실제 데이터가 메모리에 저장되는 저장소), 자동 구현 Property 사용 x
        public string EquipName { get => equipName; set => SetProperty(ref equipName, value); } // 프로퍼티(외부에서 저장소에 접근하는 경로)

        private string ipAddress;
        public string IpAddress { get => ipAddress; set => SetProperty(ref ipAddress, value); }

        private int port;
        public int Port { get => port; set => SetProperty(ref port, value); }

        private double minTemp;
        public double MinTemp { get => minTemp; set => SetProperty(ref minTemp, value); }

        private double maxTemp;
        public double MaxTemp { get => maxTemp; set => SetProperty(ref maxTemp, value); }

        private string location;
        public string Location { get => location; set => SetProperty(ref location, value); }

        private string isActive = "N";

        public string IsActive
        {
            get => isActive;
            set
            {
                if (SetProperty(ref isActive, value))
                {
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        private string createDate;
        public string CreateDate { get => createDate; set => SetProperty(ref createDate, value); }

        #region Extra Properties

        private double currentTemp = 0d;

        public double CurrentTemp
        {
            get => currentTemp;
            set => SetProperty(ref currentTemp, value);
        }

        private string status = "NO DATA";

        public string Status
        {
            get
            {
                if (IsActive is "N") return "INACTIVE";
                if (string.IsNullOrEmpty(status)) return "NO DATA";
                return status;
            }
            set => SetProperty(ref status, value);
        }

        private DateTime lastUpdateTime;

        public DateTime LastUpdateTime
        {
            get => lastUpdateTime;
            set
            {
                if (SetProperty(ref lastUpdateTime, value)) RefreshUpdateTime();
            }
        }

        public string ListUpdateTimeTxt
        {
            get
            {
                if (LastUpdateTime == default) return "-";

                var diff = DateTime.Now - LastUpdateTime;
                if (diff.TotalSeconds < 60) return "방금 전";
                if (diff.TotalMinutes < 2) return $"{(int)diff.TotalMinutes}분 전";
                return LastUpdateTime.ToString("MM-dd HH:mm");
            }
        }

        public string ReportUpdateTimeTxt
            => LastUpdateTime == default ? "-" : LastUpdateTime.ToString("MM-dd HH:mm:ss");

        private TimeSpan totalRuntime;

        public TimeSpan TotalRuntime
        {
            get => totalRuntime;
            set => SetProperty(ref totalRuntime, value);
        }

        private double operatingRate;

        public double OperatingRate
        {
            get => operatingRate;
            set => SetProperty(ref operatingRate, value);
        }

        private bool isChecked = false;
        public bool IsChecked { get => isChecked; set => SetProperty(ref isChecked, value); }

        #endregion Extra Properties

        #region DTO

        public class DTO
        {
            [Column("EQUIP_NAME")] public string EquipName { get; set; }
            [Column("IP_ADDRESS")] public string IpAddress { get; set; }
            [Column("PORT")] public int Port { get; set; }
            [Column("MIN_TEMP")] public double MinTemp { get; set; }
            [Column("MAX_TEMP")] public double MaxTemp { get; set; }
            [Column("IS_ACTIVE")] public string IsActive { get; set; }
            [Column("LOCATION")] public string Location { get; set; }
            [Column("CREATE_DATE")] public string CreateDate { get; set; }

            public static Dictionary<string, object> GetChangedColumns(DTO original, DTO current)
            {
                var changedColumns = new Dictionary<string, object>();
                var properties = typeof(DTO).GetProperties();

                foreach (var prop in properties)
                {
                    var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
                    if (columnAttr is null) continue;

                    object oldVal = prop.GetValue(original);
                    object newVal = prop.GetValue(current);

                    if (!Equals(oldVal, newVal))
                    {
                        changedColumns.Add(columnAttr.Name ?? prop.Name, newVal);
                    }
                }
                return changedColumns;
            }

            public static DTO Convert(Equipment equip)
                => new DTO
                {
                    EquipName = equip.EquipName,
                    IpAddress = equip.IpAddress,
                    Port = equip.Port,
                    MinTemp = equip.MinTemp,
                    MaxTemp = equip.MaxTemp,
                    Location = equip.Location,
                    IsActive = equip.IsActive,
                    CreateDate = equip.CreateDate,
                };

            public void Apply(Equipment target)
            {
                target.EquipName = EquipName;
                target.IpAddress = IpAddress;
                target.Port = Port;
                target.MinTemp = MinTemp;
                target.MaxTemp = MaxTemp;
                target.Location = Location;
                target.IsActive = IsActive;
            }
        }

        #endregion DTO

        public void RefreshUpdateTime() => OnPropertyChanged(nameof(ListUpdateTimeTxt));
    }
}