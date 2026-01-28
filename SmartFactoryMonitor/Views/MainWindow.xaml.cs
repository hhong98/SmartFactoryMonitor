using Oracle.DataAccess.Client;
using SmartFactoryMonitor.Services;
using SmartFactoryMonitor.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace SmartFactoryMonitor.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            BtnHome_Click(null, null); // 켜지자마자 홈 페이지 표시
        }

        private void BtnShowList_Click(object sender, RoutedEventArgs e)
            => MainFrame.Navigate(new EquipListPage());

        private void BtnHome_Click(object sender, RoutedEventArgs e)
            => MainFrame.Navigate(new MainPage());

        private void BtnMonitor_Click(object sender, RoutedEventArgs e)
            => MainFrame.Navigate(new MonitorPage());

        protected override void OnClosing(CancelEventArgs e)
        {
            if (DataContext is MainViewModel mainVm)
            {
                mainVm.MonitorVM.StopMonitoring();
            }
            base.OnClosing(e);
        }
    }
}