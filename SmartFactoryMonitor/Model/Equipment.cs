using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SmartFactoryMonitor.Model
{
    public class Equipment : ObservableModelBase
    {
        public string EquipId { get; set; } // C#의 자동 구현 Property

        private string _equipName; // 필드(실제 데이터가 메모리에 저장되는 저장소), 자동 구현 Property 사용 x
        public string EquipName { get => _equipName; set => SetProperty(ref _equipName, value); } // 프로퍼티(외부에서 저장소에 접근하는 경로)

        private string _ipAddress;
        public string IpAddress { get => _ipAddress; set => SetProperty(ref _ipAddress, value); }

        private int _port;
        public int Port { get => _port; set => SetProperty(ref _port, value); }

        private double _minTemp;
        public double MinTemp { get => _minTemp; set => SetProperty(ref _minTemp, value); }

        private double _maxTemp;
        public double MaxTemp { get => _maxTemp; set => SetProperty(ref _maxTemp, value); }

        private string _location;
        public string Location { get => _location; set => SetProperty(ref _location, value); }

        private string _isActive;
        public string IsActive { get => _isActive; set => SetProperty(ref _isActive, value); }

        private double _currentTemp = 0d;

        public double CurrentTemp
        {
            get => _currentTemp;
            set
            {
                if (SetProperty(ref _currentTemp, value))
                {
                    OnPropertyChanged(nameof(IsOverHeat)); // 현재 온도 변경 시, 과열 여부 알림
                }
            }
        }

        public bool IsOverHeat => CurrentTemp > MaxTemp;

        private bool _isChecked = false;
        public bool IsChecked { get => _isChecked; set => SetProperty(ref _isChecked, value); }

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
    }
}