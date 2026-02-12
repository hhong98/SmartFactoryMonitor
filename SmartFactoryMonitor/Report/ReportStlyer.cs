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

        public static TableCell CreateMultiLineCell(string mainText, string subText, bool mainBold = false, Brush mainColor = null)
        {
            StackPanel panel = new StackPanel();

            // 윗줄
            panel.Children.Add(new TextBlock
            {
                Text = mainText,
                FontSize = 11,
                FontWeight = mainBold ? FontWeights.Bold : FontWeights.Normal,
                Foreground = mainColor ?? Brushes.Black,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 2)
            });

            // 아랫줄
            panel.Children.Add(new TextBlock
            {
                Text = subText,
                FontSize = 10,
                Foreground = Brushes.Gray,
                HorizontalAlignment = HorizontalAlignment.Center
            });

            return new TableCell(new BlockUIContainer(panel))
            {
                Padding = new Thickness(5, 10, 5, 10),
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(0, 0, 0, 1)
            };
        }

        public static Border CreateSummaryBox(string title, string content, string subContent = "", bool isHighlight = false)
        {
            StackPanel panel = new StackPanel();

            // 제목
            panel.Children.Add(new TextBlock
            {
                Text = title,
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Center
            });

            // 본문
            panel.Children.Add(new TextBlock
            {
                Text = content,
                FontSize = isHighlight ? 30 : 22,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Black,
                HorizontalAlignment = HorizontalAlignment.Center
            });

            // 보조 텍스트
            if (!string.IsNullOrEmpty(subContent))
            {
                panel.Children.Add(new TextBlock
                {
                    Text = subContent,
                    FontSize = 12,
                    Foreground = Brushes.Gray,
                    Margin = new Thickness(0, 5, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Center
                });
            }

            // 모서리
            return new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F8F9FA")),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(20),
                Margin = new Thickness(5)
            };
        }
    }
}