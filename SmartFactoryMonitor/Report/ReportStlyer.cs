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
            => new FlowDocument
            {
                // A4 사이즈
                PageWidth = 794,
                PageHeight = 1123,
                ColumnWidth = double.PositiveInfinity,
                FontFamily = new FontFamily("Malgun Gothic"),

                PagePadding = new Thickness(30),
                Background = (SolidColorBrush)Application.Current.TryFindResource("MainBackgroundBrush")
                    ?? Brushes.Gray
            };

        public static Paragraph CreateTitle(string text)
        {
            Paragraph p = new Paragraph(new Run(text))
            {
                FontSize = 30,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Left,
                Margin = new Thickness(0)
            };

            return p;
        }

        public static Paragraph CreateSectionHeader(string text)
        {
            Paragraph p = new Paragraph(new Run(text))
            {
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Foreground = (SolidColorBrush)Application.Current.TryFindResource("TextOnPrimaryBrush")
                ?? Brushes.White,
                Background = (SolidColorBrush)Application.Current.TryFindResource("PrimaryBlueDarkBrush")
                ?? Brushes.DarkBlue,

                Padding = new Thickness(5),
                Margin = new Thickness(0)
            };

            return p;
        }

        public static TableCell CreateHeaderCell(string text, TextAlignment align = TextAlignment.Center)
        {
            Paragraph p = new Paragraph(new Run(text))
            {
                TextAlignment = align,
                FontWeight = FontWeights.Bold,
                FontSize = 13,
                Foreground = (SolidColorBrush)Application.Current.TryFindResource("TextSecondaryBrush")
                ?? Brushes.White,
                Margin = new Thickness(0)
            };

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
            Paragraph p = new Paragraph(new Run(text))
            {
                TextAlignment = align,
                FontWeight = FontWeights.SemiBold,
                FontSize = 13,
                Foreground = Brushes.Black,
                Margin = new Thickness(0)
            };

            return new TableCell(p)
            {
                Padding = new Thickness(5),
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(0, 0, 0, 1)
            };
        }

        public static TableCell CreateMultiLineCell(string mainText, string subText, bool mainBold = false, TextAlignment align = TextAlignment.Center, Brush mainColor = null)
        {
            Paragraph p = new Paragraph()
            {
                TextAlignment = align
            };

            p.Inlines.Add(new Run(mainText)
            {
                FontSize = 13,
                FontWeight = mainBold ? FontWeights.Bold : FontWeights.Normal,
                Foreground = mainColor ?? Brushes.Black,
            });

            p.Inlines.Add(new LineBreak());

            p.Inlines.Add(new Run(subText)
            {
                FontSize = 11,
                Foreground = Brushes.Black
            });

            return new TableCell(p)
            {
                Padding = new Thickness(5, 10, 5, 10),
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(0, 0, 0, 1)
            };
        }

        public static Border CreateSummaryBox(string title, string content, string subContent = "", bool isHighlight = false)
        {
            StackPanel panel = new StackPanel()
            {
                VerticalAlignment = VerticalAlignment.Center
            };

            // 제목
            panel.Children.Add(new TextBlock
            {
                Text = title,
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 5),
                HorizontalAlignment = HorizontalAlignment.Center
            });

            // 본문
            panel.Children.Add(new TextBlock
            {
                Text = content,
                FontSize = isHighlight ? 25 : 23,
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
                    Foreground = Brushes.Black,
                    Margin = new Thickness(0, 2, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Center
                });
            }

            // 모서리
            return new Border
            {
                Background = Brushes.LightGray,
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(10),
                Margin = new Thickness(0),
                Height = 100,
                Child = panel
            };
        }
    }
}