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

namespace SmartFactoryMonitor.Controls
{
    /// <summary>
    /// Interaction logic for EquipFormPanel.xaml
    /// </summary>
    public partial class EquipFormPanel : UserControl
    {
        private MainViewModel mainVM;

        public EquipFormPanel()
        {
            InitializeComponent();

            mainVM = DataContext is MainViewModel vm
                ? vm
                : null;
        }

        public async void BtnPanelSave_Click(object sender, RoutedEventArgs args)
            => await mainVM?.EquipManageVM.UpdateCurrentEquip();

        public async void BtnPanelDelete_Click(object sender, RoutedEventArgs args)
            => await mainVM?.EquipManageVM.DeleteCurrentEquip();

        private void BtnPanelCancel_Click(object sender, RoutedEventArgs e)
        {
            if (mainVM is null) return;

            mainVM.EquipManageVM.EditingEquip = null;
            mainVM.EquipManageVM.SelectedEquip = null;

            mainVM.IsPanelOpened = false;
        }
    }
}