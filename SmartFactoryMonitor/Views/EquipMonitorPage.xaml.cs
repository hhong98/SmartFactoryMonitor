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
        public List<string> FilterOptions => new List<string>
        {
            "정상", "주의", "위험", "연결 끊김"
        };

        public EquipMonitorPage()
        {
            InitializeComponent();
        }
    }
}