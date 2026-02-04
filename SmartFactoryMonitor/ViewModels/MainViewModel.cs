using SmartFactoryMonitor.Model;
using SmartFactoryMonitor.Repository;
using SmartFactoryMonitor.Server;
using SmartFactoryMonitor.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SmartFactoryMonitor.ViewModels
{
    public class MainViewModel : IDisposable, INotifyPropertyChanged
    {
        private readonly EquipRepository Repository;

        private readonly OracleService _dbService = new OracleService();
        private readonly MonitoringService _mService;
        private readonly EquipService _eService;

        public MonitoringViewModel MonitorVM { get; }
        public EquipManageViewModel EquipManageVM { get; }

        public HttpServer server { get; }

        private Window mainWin;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private bool _isPanelOpened;

        public bool IsPanelOpened
        {
            get => _isPanelOpened;
            set
            {
                if (_isPanelOpened == value) return;
                _isPanelOpened = value;
                OnPropertyChanged(nameof(IsPanelOpened));

                if (mainWin != null)
                {
                    mainWin.Width = _isPanelOpened ? 1115 : 900;
                }
            }
        }

        public MainViewModel()
        {
            // DB 의존성 주입
            _mService = new MonitoringService(_dbService);

            Repository = new EquipRepository(_dbService);
            _eService = new EquipService(_dbService);

            // Repository 의존성 주입
            MonitorVM = new MonitoringViewModel(Repository, _mService);
            EquipManageVM = new EquipManageViewModel(Repository, _eService);

            // 패널 조작용 MainWinodw 객체 가져오기
            mainWin = Application.Current?.MainWindow;

            // 초기 데이터 불러오기
            InitializeData(Repository);

            server = new HttpServer(_dbService);
            server.Start();
        }

        private async void InitializeData(EquipRepository repo)
        {
            await repo.LoadAll();
        }

        public void Dispose()
        {
            try { server?.Stop(); } catch { }
            try { server?.Dispose(); } catch { }
        }
    }
}