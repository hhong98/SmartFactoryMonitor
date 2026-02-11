using Oracle.DataAccess.Client;
using SmartFactoryMonitor.Services;
using SmartFactoryMonitor.ViewModels;
using SmartFactoryMonitor.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

namespace SmartFactoryMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            BtnMonitor_Click();
        }

        private void BtnShowList_Click(object sender = null, RoutedEventArgs e = null)
            => MainFrame.Navigate(new EquipListPage());

        private void BtnMonitor_Click(object sender = null, RoutedEventArgs e = null)
            => MainFrame.Navigate(new EquipMonitorPage());

        //protected override void OnClosing(CancelEventArgs e)
        //{
        //    try
        //    {
        //        if (DataContext is MainViewModel mainVm)
        //        {
        //            mainVm.MonitorVM?.StopMonitoring();
        //            mainVm.server?.Stop();
        //            mainVm.server?.Dispose();

        //            mainVm?.Dispose();
        //        }
        //    }
        //    catch { }
        //    base.OnClosing(e);
        //}
    }
}