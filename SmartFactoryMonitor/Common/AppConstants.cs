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
        public static readonly List<String> Locations = new List<string>
        {
            // A-구역
            "A-구역 원자재 창고", "A-구역 정밀 가공실", "A-구역 금형 보관실",
            "A-구역 절삭 공정 라인", "A-구역 부품 세척실", "A-구역 품질 검사소",

            // B-구역
            "B-구역 조립라인", "B-구역 회로 기판 실장실", "B-구역 납땜 작업실",
            "B-구역 모터 조립실", "B-구역 컨베이어 제어실", "B-구역 중간 부품 적재소",

            // C-구역
            "C-구역 펌프실", "C-구역 공조 제어실", "C-구역 배전반 및 전기실",
            "C-구역 냉각수 순환장치", "C-구역 보일러실", "C-구역 비상 발전기실",

            // D-구역
            "D-구역 열처리실", "D-구역 표면 처리실", "D-구역 도장 스프레이실",
            "D-구역 건조 및 경화실", "D-구역 최종 포장라인", "D-구역 출하 검사 구역"
        };
    }
}
