using SmartFactoryMonitor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace SmartFactoryMonitor.Report
{
    public class EquipInvReportGenerator : BaseReportGenerator
    {
        private readonly List<Equipment> activeEquips;
        private readonly List<Equipment> inactiveEquips;

        public EquipInvReportGenerator(List<Equipment> active, List<Equipment> inactive)
            : base("설비 보유현황 보고서", new List<string>
            {
                $"전체 설비: {active.Count + inactive.Count} (가동 {active.Count} / 미가동 {inactive.Count})"
            })
        {
            activeEquips = active;
            inactiveEquips = inactive;
        }

        protected override void CreateBody(FlowDocument doc)
        {
            doc.Blocks.Add(ReportStlyer.CreateSectionHeader("상세 설비 내역"));

            doc.Blocks.Add(CreateLabel($"가동 설비: {activeEquips.Count}건"));
            doc.Blocks.Add(CreateEquipTable(activeEquips));

            doc.Blocks.Add(CreateLabel($"미가동 설비: {inactiveEquips.Count}건"));
            doc.Blocks.Add(CreateEquipTable(inactiveEquips));
        }

        private Paragraph CreateLabel(string text)
            => new Paragraph(new Run(text))
            {
                TextAlignment = TextAlignment.Right,
                FontSize = 12,
                Margin = new Thickness(0, 10, 0, 0)
            };

        private Table CreateEquipTable(List<Equipment> dataList)
        {
            Table table = new Table
            {
                CellSpacing = 0,
                Margin = new Thickness(0),
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1)
            };

            // Grid Width="*"  -> new GridLength(1, GridUnitType.Star)
            // Grid Width="50" -> new GridLength(50)
            // 열 비율 설정
            table.Columns.Add(new TableColumn { Width = new GridLength(1.3, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1.2, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(0.5, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1.5, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });

            // 헤더 행
            TableRowGroup headerGroup = new TableRowGroup();
            TableRow headerRow = new TableRow
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E2E6EA"))
            };

            headerRow.Cells.Add(ReportStlyer.CreateHeaderCell("설비명", TextAlignment.Left));
            headerRow.Cells.Add(ReportStlyer.CreateHeaderCell("IP 주소", TextAlignment.Left));
            headerRow.Cells.Add(ReportStlyer.CreateHeaderCell("포트"));
            headerRow.Cells.Add(ReportStlyer.CreateHeaderCell("설치 위치"));
            headerRow.Cells.Add(ReportStlyer.CreateHeaderCell("등록일"));
            headerRow.Cells.Add(ReportStlyer.CreateHeaderCell("비고"));

            headerGroup.Rows.Add(headerRow);
            table.RowGroups.Add(headerGroup);

            // 데이터 행
            TableRowGroup dataGroup = new TableRowGroup();
            foreach (var item in dataList)
            {
                TableRow row = new TableRow();

                row.Cells.Add(ReportStlyer.CreateDataCell(item.EquipName, TextAlignment.Left));
                row.Cells.Add(ReportStlyer.CreateDataCell(item.IpAddress, TextAlignment.Left));
                row.Cells.Add(ReportStlyer.CreateDataCell(item.Port.ToString()));
                row.Cells.Add(ReportStlyer.CreateDataCell(item.Location));
                row.Cells.Add(ReportStlyer.CreateDataCell(
                    DateTime.Parse(item.CreateDate).ToString("yyyy-MM-dd\nHH:mm")));
                row.Cells.Add(ReportStlyer.CreateDataCell(""));

                dataGroup.Rows.Add(row);
            }
            table.RowGroups.Add(dataGroup);

            return table;
        }
    }
}