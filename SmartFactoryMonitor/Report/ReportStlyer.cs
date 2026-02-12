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
    public class ReportStlyer
    {
        public static FlowDocument CreateReportBase()
        {
            FlowDocument doc = new FlowDocument();

            // A4 사이즈
            doc.PageWidth = 794;
            doc.PageHeight = 1123;
            doc.ColumnWidth = double.PositiveInfinity;
            doc.FontFamily = new FontFamily("Malgun Gothic");

            doc.PagePadding = new Thickness(30);
            doc.Background = (SolidColorBrush)Application.Current.TryFindResource("MainBackgroundBrush")
                ?? Brushes.Gray;

            return doc;
        }

        public static Paragraph CreateTitle(string text)
        {
            Paragraph p = new Paragraph(new Run(text));
            p.FontSize = 30;
            p.FontWeight = FontWeights.Bold;
            p.TextAlignment = TextAlignment.Left;
            p.Margin = new Thickness(0);

            return p;
        }

        public static Paragraph CreateSectionHeader(string text)
        {
            Paragraph p = new Paragraph(new Run(text));
            p.FontSize = 14;
            p.FontWeight = FontWeights.Bold;
            p.TextAlignment = TextAlignment.Center;
            p.Foreground = (SolidColorBrush)Application.Current.TryFindResource("TextOnPrimaryBrush")
                ?? Brushes.White;
            p.Background = (SolidColorBrush)Application.Current.TryFindResource("PrimaryBlueDarkBrush")
                ?? Brushes.DarkBlue;

            p.Padding = new Thickness(5);
            p.Margin = new Thickness(0);

            return p;
        }

        public static TableCell CreateHeaderCell(string text, TextAlignment align = TextAlignment.Center)
        {
            Paragraph p = new Paragraph(new Run(text));
            p.TextAlignment = align;
            p.FontWeight = FontWeights.Bold;
            p.FontSize = 11;
            p.Foreground = (SolidColorBrush)Application.Current.TryFindResource("TextSecondaryBrush")
                ?? Brushes.White;
            p.Margin = new Thickness(0);

            return new TableCell(p)
            {
                Background = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString("#E2E6EA")),
                Padding = new Thickness(5),
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(0, 0, 0, 1),
            };
        }

        public static TableCell CreateDataCell(string text, TextAlignment align = TextAlignment.Center)
        {
            Paragraph p = new Paragraph(new Run(text));
            p.TextAlignment = align;
            p.FontWeight = FontWeights.SemiBold;
            p.FontSize = 10;
            p.Foreground = Brushes.Black;
            p.Margin = new Thickness(0);

            return new TableCell(p)
            {
                Padding = new Thickness(5),
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(0, 0, 0, 1)
            };
        }
    }
}