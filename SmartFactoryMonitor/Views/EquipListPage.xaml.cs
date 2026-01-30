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

namespace SmartFactoryMonitor.Views
{
    /// <summary>
    /// EquipListPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EquipListPage : Page
    {
        public EquipListPage()
        {
            InitializeComponent();

            DataContext = App.Current.MainWindow.DataContext;
        }

        public async void BtnDelete_Click(object sender, RoutedEventArgs args)
        {
            // DataContext 확인 - MainViewModel의 EquipVM 공유
            if (DataContext is MainViewModel mainVM)
            {
                await mainVM.EquipManageVM.DeleteEquip();
            }
        }

        public void BtnAdd_Click(object sender, RoutedEventArgs args)
        {
            EquipFormWindow equipFormWindow = new EquipFormWindow
            {
                Owner = Application.Current.MainWindow
            };

            // DataContext로 MainViewModel 전달
            equipFormWindow.EquipAddForm.DataContext = DataContext;

            equipFormWindow.ShowDialog();
        }

        private void PanelToggle_Click(object sender, MouseButtonEventArgs e)
        {
        }
    }

    /*
        * MVVM 패턴 - 화면(Page)은 ViewModel의 데이터를 바라보기만 한다
        * 메서드를 직접 호출할 필요 없이 ViewModel의 속성을 UI(xaml)에 바인딩
        *
           중복 방지: MainViewModel에서 이미 타이머가 돌아가고 있는데
               MonitorPage에서 또 메서드를 호출하면 리소스를 낭비

           데이터 일치: MainViewModel은 하나인데 여러 곳에서 메서드를 각자 실행하면
               데이터 값이 꼬일 가능성 존재

           코드 깔끔: Page의 비하인드 코드(.xaml.cs)는
               가급적 비워두는 것이 유지보수에 유리
        */
}