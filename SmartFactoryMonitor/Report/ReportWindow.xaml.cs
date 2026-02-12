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
        private readonly IReportGenerator generator;

        public ReportWindow(IReportGenerator generator)
        {
            InitializeComponent();

            this.generator = generator;

            docReader.Document = generator.GenerateDocument(); // 표시용 문서
        }

        private void BtnPrintClick(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() is true)
            {
                IDocumentPaginatorSource idp = generator.GenerateDocument(); // 출력용 문서
                if (idp is null) { MessageBox.Show("올바르지 않는 형식입니다\n다시 시도하세요"); return; }

                MessageBox.Show("출력이 요청되었습니다");
                printDialog.PrintDocument(idp.DocumentPaginator, "Smart Factory Report");
            }
        }
    }
}