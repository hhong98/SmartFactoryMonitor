using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SmartFactoryMonitor.Report
{
    /// <summary>
    /// Interaction logic for ReportWindow.xaml
    /// </summary>
    public partial class ReportWindow : Window
    {
        public ReportWindow(UserControl report)
        {
            InitializeComponent();

            LoadReport(report);
        }

        private void LoadReport(UserControl report)
        {
            FixedDocument fixedDoc = new FixedDocument();
            fixedDoc.DocumentPaginator.PageSize = new Size(794, 1123); // A4 사이즈

            FixedPage fixedPage = new FixedPage { Width = 794, Height = 1123 };

            report.Width = 794;
            report.Height = 1123;
            fixedPage.Children.Add(report);

            PageContent content = new PageContent();
            ((IAddChild)content).AddChild(fixedPage);
            fixedDoc.Pages.Add(content);

            docViewer.Document = fixedDoc;
        }
    }
}