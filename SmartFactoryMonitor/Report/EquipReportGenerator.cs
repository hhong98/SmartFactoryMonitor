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
    public class EquipReportGenerator : IReportGenerator
    {
        private readonly List<Equipment> activeEquips;
        private readonly List<Equipment> inactiveEquips;

        public EquipReportGenerator(List<Equipment> activeEquips, List<Equipment> inactiveEquips)
        {
            this.activeEquips = activeEquips;
            this.inactiveEquips = inactiveEquips;
        }

        public FlowDocument GenerateDocument()
        {
            // 1. 기본 문서 생성 (A4)
            FlowDocument doc = ReportStlyer.CreateReportBase();

            // 2. 제목
            doc.Blocks.Add(ReportStlyer.CreateTitle("설비 보유현황 보고서"));
            doc.Blocks.Add(new BlockUIContainer(new Separator()));

            // -------------------------------------------------------
            // 3. 메타 정보
            // -------------------------------------------------------
            Paragraph metaInfo = new Paragraph();
            metaInfo.TextAlignment = TextAlignment.Right;
            metaInfo.FontSize = 13;
            metaInfo.FontWeight = FontWeights.SemiBold;
            metaInfo.LineHeight = 20;

            string date = DateTime.Now.ToString("yyyy년 MM월 dd일 HH mm분");
            metaInfo.Inlines.Add(new Run($"출력 일시: {date}"));
            metaInfo.Inlines.Add(new LineBreak()); // LineBreak() = 줄바꿈

            int activeCount = activeEquips.Count;
            int inactiveCount = inactiveEquips.Count;
            int totalCount = activeCount + inactiveCount;
            metaInfo.Inlines.Add(new Run($"전체 설비: {totalCount}건 (가동 {activeCount} / 미가동 {inactiveCount})"));

            doc.Blocks.Add(metaInfo);

            // -------------------------------------------------------
            // 4. 섹션 헤더
            // -------------------------------------------------------
            doc.Blocks.Add(ReportStlyer.CreateSectionHeader("상세 설비 내역"));

            // -------------------------------------------------------
            // 5. 가동 설비 섹션
            // -------------------------------------------------------
            Paragraph activeLabel = new Paragraph(new Run($"가동 설비: {activeEquips.Count}건"));
            activeLabel.TextAlignment = TextAlignment.Right;
            activeLabel.FontSize = 12;
            activeLabel.Margin = new Thickness(0, 10, 0, 0);
            doc.Blocks.Add(activeLabel);

            // 가동 설비 테이블
            doc.Blocks.Add(CreateEquipTable(activeEquips));

            // -------------------------------------------------------
            // 6. 미가동 설비 섹션
            // -------------------------------------------------------
            Paragraph inactiveLabel = new Paragraph(new Run($"미가동 설비: {inactiveEquips.Count}건"));
            inactiveLabel.TextAlignment = TextAlignment.Right;
            inactiveLabel.FontSize = 12;
            inactiveLabel.Margin = new Thickness(0, 10, 0, 0);
            doc.Blocks.Add(inactiveLabel);

            // 미가동 설비 테이블
            doc.Blocks.Add(CreateEquipTable(inactiveEquips));

            return doc;
        }

        private Table CreateEquipTable(List<Equipment> dataList)
        {
            Table table = new Table();
            table.CellSpacing = 0;
            table.Margin = new Thickness(0);
            table.BorderBrush = Brushes.LightGray;
            table.BorderThickness = new Thickness(1);

            // Grid Width="*"  -> new GridLength(1, GridUnitType.Star)
            // Grid Width="50" -> new GridLength(50)
            // 열 너비 지정
            table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });

            // 헤더 행
            TableRowGroup headerGroup = new TableRowGroup();
            TableRow headerRow = new TableRow();
            headerRow.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E2E6EA"));

            headerRow.Cells.Add(ReportStlyer.CreateHeaderCell("설비명"));
            headerRow.Cells.Add(ReportStlyer.CreateHeaderCell("IP 주소"));
            headerRow.Cells.Add(ReportStlyer.CreateHeaderCell("포트"));
            headerRow.Cells.Add(ReportStlyer.CreateHeaderCell("설치 위치"));
            headerRow.Cells.Add(ReportStlyer.CreateHeaderCell("가동"));
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
                row.Cells.Add(ReportStlyer.CreateDataCell(item.IsActive));
                row.Cells.Add(ReportStlyer.CreateDataCell(""));

                dataGroup.Rows.Add(row);
            }
            table.RowGroups.Add(dataGroup);

            return table;
        }
    }
}