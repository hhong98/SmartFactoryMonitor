using SmartFactoryMonitor.Common;
using SmartFactoryMonitor.Model;
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

namespace SmartFactoryMonitor
{
    /// <summary>
    /// Control - 설비 등록 Form
    /// </summary>
    public partial class EquipAddForm : UserControl
    {
        public Equipment.Add_DTO NewEquipment;
        
        public EquipAddForm()
        {
            InitializeComponent();

            NewEquipment = new Equipment.Add_DTO();
            Location.ItemsSource = AppConstants.Locations;
        }

        private async void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if(DataContext is MainViewModel MainVm)
            {
                // Form에서 입력된 값 받아오기
                NewEquipment.EquipName = EquipName.Text;
                NewEquipment.IpAddress = IpAddress.Text;
                if (int.TryParse(Port.Text, out int port)) { NewEquipment.Port = port; }
                if (double.TryParse(MinTemp.Text, out double minTemp)) { NewEquipment.MinTemp = minTemp; }
                if (double.TryParse(MaxTemp.Text, out double maxTemp)) { NewEquipment.MaxTemp = maxTemp; }
                NewEquipment.Location = Location.SelectedItem.ToString();

                // EquipVM으로 전달
                await MainVm.EquipManageVM.AddEquip(NewEquipment);
                BtnReset_Click(null, null);

                // 부모 창을 찾아 성공 신호 전송
                var parentWindow = Window.GetWindow(this);
                if (parentWindow != null)
                {
                    // DialogResult를 true로 설정하면 ShowDialog()를 호출했던 곳으로 신호가 가고, 
                    // 창은 자동으로 닫힘
                    parentWindow.DialogResult = true;
                }
            }
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            EquipName.Clear();
            IpAddress.Clear();
            Port.Clear();
            MinTemp.Clear();
            MaxTemp.Clear();
            Location.SelectedIndex = -1; 
        }
    }
}
