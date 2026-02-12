using SmartFactoryMonitor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace SmartFactoryMonitor.Report
{
    public class EquipStatusReportGenerator : BaseReportGenerator
    {
        private readonly List<Equipment> statusList;

        public EquipStatusReportGenerator(List<Equipment> status)
            : base("설비 가동현황 보고서", new List<string>
            {
                MakeOpSummary(status)
            })
        {
            statusList = status;
        }

        protected override void CreateBody(FlowDocument doc)
        {
            doc.Blocks.Add(ReportStlyer.CreateSectionHeader("가동현황 요약"));
            doc.Blocks.Add(CreateDashboardSection());

            doc.Blocks.Add(ReportStlyer.CreateSectionHeader("상세 가동 내역"));
            doc.Blocks.Add(CreateStatusTable());
        }

        private Table CreateDashboardSection()
        {
            Table table = new Table()
            {
                CellSpacing = 10
            };
            table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });

            TableRowGroup group = new TableRowGroup();
            TableRow row = new TableRow();

            // 왼쪽 박스 : 가동 현황
            Border leftBox = ReportStlyer.CreateSummaryBox(
                "가동 현황",
                $"총 가동 설비: {statusList.Count}대",
                $"정상: {statusList.Where(s => s.Status is "STABLE").Count()} / "
                    + $"주의: {statusList.Where(s => s.Status is "WARN").Count()}&#x0a;"
                    + $"위험: {statusList.Where(s => s.Status is "ERROR").Count()} / "
                    + $"연결 x: {statusList.Where(s => s.Status is "NO DATA").Count()})");
            row.Cells.Add(new TableCell(new BlockUIContainer(leftBox)));

            // 오른쪽 박스 : 전체 평균 가동률
            double avgRate = statusList.Count > 0
                ? statusList.Average(s => s.OperatingRate)
                : 0;
            Border rightBox = ReportStlyer.CreateSummaryBox(
                "전체 평균 가동률",
                $"{avgRate:F1} %",
                string.Empty,
                isHighlight: true);
            row.Cells.Add(new TableCell(new BlockUIContainer(rightBox)));

            group.Rows.Add(row);
            table.RowGroups.Add(group);
            return table;
        }

        private Table CreateStatusTable()
        {
            Table table = new Table
            {
                CellSpacing = 0,
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(0, 1, 0, 1)
            };

            // 열 비율 설정
            table.Columns.Add(new TableColumn { Width = new GridLength(2, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1.5, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1.5, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1.5, GridUnitType.Star) });

            // 헤더 열
            TableRowGroup headerGroup = new TableRowGroup();
            TableRow headerRow = new TableRow
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E2E6EA"))
            };

            headerRow.Cells.Add(ReportStlyer.CreateHeaderCell("설비명 (위치)"));
            headerRow.Cells.Add(ReportStlyer.CreateHeaderCell("통신 (수신)"));
            headerRow.Cells.Add(ReportStlyer.CreateHeaderCell("온도 (범위)"));
            headerRow.Cells.Add(ReportStlyer.CreateHeaderCell("가동률 (누적)"));

            headerGroup.Rows.Add(headerRow);
            table.RowGroups.Add(headerGroup);

            // 데이터 행
            TableRowGroup dataGroup = new TableRowGroup();
            foreach (var status in statusList)
            {
                TableRow row = new TableRow();

                // 설비명 + 위치 (2줄)
                row.Cells.Add(ReportStlyer.CreateMultiLineCell(status.EquipName, status.Location, true));

                // 통신상태 + 시간
                string connection = string.Equals(status.Status, "NO DATA") ? "○ 끊김" : "● 정상";
                row.Cells.Add(ReportStlyer.CreateMultiLineCell(
                    connection,
                    status.ReportUpdateTimeTxt,
                    false,
                    string.Equals(status.Status, "NO DATA") ? Brushes.Red : Brushes.Green));

                // 온도 + 범위
                row.Cells.Add(ReportStlyer.CreateMultiLineCell($"{status.CurrentTemp:F1}℃", $"({status.MinTemp:F1}~{status.MaxTemp:F1})"));

                // 가동률 + 시간
                row.Cells.Add(ReportStlyer.CreateMultiLineCell($"{status.OperatingRate}%", $"({status.TotalRuntime})"));

                dataGroup.Rows.Add(row);
            }
            table.RowGroups.Add(dataGroup);

            return table;
        }

        private static string MakeOpSummary(List<Equipment> status)
        {
            double avgRate = status.Count > 0
                ? status.Average(s => s.OperatingRate)
                : 0;

            return $"전체 평균 가동률: {avgRate:F1}$";
        }
    }
}