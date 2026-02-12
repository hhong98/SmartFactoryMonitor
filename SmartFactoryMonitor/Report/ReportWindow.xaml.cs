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
        public ReportWindow(IReportGenerator generator)
        {
            InitializeComponent();

            docReader.Document = generator.GenerateDocument();
        }

        private void BtnPrintClick(object sender, RoutedEventArgs e)
        {
            // TODO : Print Dialog 띄우기
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() is true)
            {
                IDocumentPaginatorSource idp = docReader.Document as IDocumentPaginatorSource;
                if (idp is null) { MessageBox.Show("올바르지 않는 형식입니다\n다시 시도하세요"); return; }

                printDialog.PrintDocument(idp.DocumentPaginator, "Smart Factory Report");
                MessageBox.Show("출력이 요청되었습니다");
            }
        }

        private void BtnSavePdf_Click(object sender, RoutedEventArgs e)
        {
            // TODO : File Dialog 띄우기
            MessageBox.Show("PDF가 저장되었습니다");
        }
    }
}