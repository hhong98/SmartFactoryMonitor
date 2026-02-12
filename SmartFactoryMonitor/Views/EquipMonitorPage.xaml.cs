using SmartFactoryMonitor.Report;
using SmartFactoryMonitor.ViewModels;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmartFactoryMonitor.Views
{
    /// <summary>
    /// Interaction logic for EquipMonitor.xaml
    /// </summary>
    public partial class EquipMonitorPage : Page
    {
        private MainViewModel mainVM;

        public List<string> FilterOptions => new List<string>
        {
            "전체", "정상", "주의", "위험", "연결 끊김"
        };

        public EquipMonitorPage()
        {
            InitializeComponent();

            mainVM = DataContext is MainViewModel vm
                ? vm
                : null;
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            if (mainVM is null) return;

            var generator = new EquipStatusReportGenerator(
                mainVM.EquipManageVM.ActiveEquipments.ToList());

            var reportWin = new ReportWindow(generator);
            reportWin.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            reportWin.Show();
        }
    }
}