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
using System.Windows.Shapes;

namespace SmartFactoryMonitor.Views
{
    /// <summary>
    /// EquipUpdateWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EquipUpdateWindow : Window
    {
        private Equipment _selectedEquip;

        // 현재는 표시 항목이 동일하므로 사용 X
        //private Equipment.Update_DTO _selectedEquip; 

        public EquipUpdateWindow(Equipment equip)
        {
            InitializeComponent();

            _selectedEquip = equip;

            Location.ItemsSource = AppConstants.Locations;
            SetDisplayData();
        }

        private void SetDisplayData()
        {
            EquipName.Text = _selectedEquip.EquipName;
            IpAddress.Text = _selectedEquip.IpAddress;
            Port.Text = _selectedEquip.Port.ToString();
            MinTemp.Text = _selectedEquip.MinTemp.ToString("F1");
            MaxTemp.Text = _selectedEquip.MaxTemp.ToString("F1");
            Location.SelectedItem = _selectedEquip.Location;
            IsActive.IsChecked = string.Equals(_selectedEquip.IsActive, "Y");
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            ToggleEditMode(true);
        }

        private async void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("변경된 내용을 저장하시겠습니까?", "확인", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _selectedEquip.EquipName = EquipName.Text;
                _selectedEquip.IpAddress = IpAddress.Text;
                if (int.TryParse(Port.Text, out int port)) _selectedEquip.Port = port;
                if (double.TryParse(MinTemp.Text, out double min)) _selectedEquip.MinTemp = min;
                if (double.TryParse(MaxTemp.Text, out double max)) _selectedEquip.MaxTemp = max;
                _selectedEquip.Location = Location.SelectedItem.ToString();
                _selectedEquip.IsActive = (IsActive.IsChecked ?? false) ? "Y" : "N";

                if(DataContext is MainViewModel mainVm)
                {
                    await mainVm.EquipManageVM.UpdateEquip(_selectedEquip);
                }

                ToggleEditMode(false);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            SetDisplayData();
            ToggleEditMode(false); 
        }

        private void ToggleEditMode(bool _isEdit)
        {
            EquipName.IsReadOnly = !_isEdit;
            IpAddress.IsReadOnly = !_isEdit;
            Port.IsReadOnly = !_isEdit;
            MinTemp.IsReadOnly = !_isEdit;
            MaxTemp.IsReadOnly = !_isEdit;
            Location.IsEnabled = _isEdit;
            IsActive.IsHitTestVisible = _isEdit;

            BtnEdit.Visibility = _isEdit ? Visibility.Collapsed : Visibility.Visible;
            EditActionPanel.Visibility = _isEdit ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
