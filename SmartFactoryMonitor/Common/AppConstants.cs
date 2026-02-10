using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFactoryMonitor.Common
{
    public static class AppConstants
    {
        /* 설비 위치 리스트 */

        public class LocationItem
        {
            public string Group { get; set; }
            public string Name { get; set; }

            public string FullName => $"{Group} {Name}";
        }

        public static readonly List<LocationItem> Locations = new List<LocationItem>
        {
            // A-구역
            new LocationItem { Group = "A-구역", Name = "원자재 창고" },
            new LocationItem { Group = "A-구역", Name = "정밀 가공실" },
            new LocationItem { Group = "A-구역", Name = "금형 보관실" },
            new LocationItem { Group = "A-구역", Name = "절삭 공정 라인" },
            new LocationItem { Group = "A-구역", Name = "부품 세척실" },
            new LocationItem { Group = "A-구역", Name = "품질 검사소" },

            // B-구역
            new LocationItem { Group = "B-구역", Name = "조립라인" },
            new LocationItem { Group = "B-구역", Name = "회로 기판 실장실" },
            new LocationItem { Group = "B-구역", Name = "납땜 작업실" },
            new LocationItem { Group = "B-구역", Name = "모터 조립실" },
            new LocationItem { Group = "B-구역", Name = "컨베이어 제어실" },
            new LocationItem { Group = "B-구역", Name = "중간 부품 적재소" },

            // C-구역
            new LocationItem { Group = "C-구역", Name = "펌프실" },
            new LocationItem { Group = "C-구역", Name = "공조 제어실" },
            new LocationItem { Group = "C-구역", Name = "배전반 및 전기실" },
            new LocationItem { Group = "C-구역", Name = "냉각수 순환장치" },
            new LocationItem { Group = "C-구역", Name = "보일러실" },
            new LocationItem { Group = "C-구역", Name = "비상 발전기실" },

            // D-구역
            new LocationItem { Group = "D-구역", Name = "열처리실" },
            new LocationItem { Group = "D-구역", Name = "표면 처리실" },
            new LocationItem { Group = "D-구역", Name = "도장 스프레이실" },
            new LocationItem { Group = "D-구역", Name = "건조 및 경화실" },
            new LocationItem { Group = "D-구역", Name = "최종 포장라인" },
            new LocationItem { Group = "D-구역", Name = "출하 검사 구역" }
        };
    }
}