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
    public class MainViewModel
    {
        private readonly EquipRepository Repository;

        private readonly OracleService _dbService = new OracleService();
        private readonly MonitoringService _mService;
        private readonly EquipService _eService;

        public MonitoringViewModel MonitorVM { get; }
        public EquipManageViewModel EquipManageVM { get; }

        // [TEMP] Android 앱 개발 용도 (REST API)
        public HttpServer server { get; }

        public MainViewModel()
        {
            // DB 의존성 주입
            _mService = new MonitoringService(_dbService);

            Repository = new EquipRepository(_dbService);
            _eService = new EquipService(_dbService);

            // Repository 의존성 주입
            MonitorVM = new MonitoringViewModel(Repository, _mService);
            EquipManageVM = new EquipManageViewModel(Repository, _eService);

            // 초기 데이터 불러오기
            InitializeData(Repository);

            // [TEMP] Android 앱 개발 용도 (REST API)
            server = new HttpServer(_dbService);
            server.Start();
        }

        private async void InitializeData(EquipRepository repo)
        {
            await repo.LoadAll();
        }
    }
}