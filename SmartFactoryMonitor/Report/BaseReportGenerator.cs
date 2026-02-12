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
    public abstract class BaseReportGenerator : IReportGenerator
    {
        protected string Title { get; set; }
        protected List<string> MetaDataList { get; set; }

        public BaseReportGenerator(string title, List<string> metaDataList = null)
        {
            Title = title;
            MetaDataList = metaDataList;
        }

        // 템플릿 메서드: 보고서 생성의 전체 흐름을 잡음
        public FlowDocument GenerateDocument()
        {
            // 1. 문서 생성
            FlowDocument doc = ReportStlyer.CreateReportBase();

            // 2. 제목 & 구분선
            doc.Blocks.Add(ReportStlyer.CreateTitle(Title));
            doc.Blocks.Add(new BlockUIContainer(new Separator()
            {
                Margin = new Thickness(0, 5, 0, 0),
                Background = (SolidColorBrush)Application.Current.TryFindResource("TextSecondaryBrush") ?? Brushes.Gray
            }));

            // 3. 추가 정보 (기본: 출력 일시)
            doc.Blocks.Add(CreateMetaData());

            // 4. 본문 내용
            CreateBody(doc);

            return doc;
        }

        protected Paragraph CreateMetaData()
        {
            Paragraph metaData = new Paragraph
            {
                TextAlignment = TextAlignment.Right,
                FontSize = 13,
                FontWeight = FontWeights.SemiBold,
                LineHeight = 20,
                Margin = new Thickness(0, 10, 0, 20)
            };

            string date = DateTime.Now.ToString("yyyy년 MM월 dd일 HH시 mm분");
            metaData.Inlines.Add(new Run($"출력 일시: {date}"));
            metaData.Inlines.Add(new LineBreak()); // LineBreak() = 한 줄 띄우기

            // 추가로 받은 정보가 있다면 삽입
            if (MetaDataList != null && MetaDataList.Count > 0)
                MetaDataList.ForEach(meta =>
                {
                    metaData.Inlines.Add(new Run(meta));
                    metaData.Inlines.Add(new LineBreak());
                });

            return metaData;
        }

        protected abstract void CreateBody(FlowDocument doc);
    }
}