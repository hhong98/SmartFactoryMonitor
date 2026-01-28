using SmartFactoryMonitor.Model;
using SmartFactoryMonitor.Repository;
using SmartFactoryMonitor.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SmartFactoryMonitor.ViewModels
{
    public class MonitoringViewModel : ObservableModelBase
    {
        private readonly EquipRepository _repo;
        private readonly MonitoringService _mService;
        private DispatcherTimer refreshTimer;

        private CancellationTokenSource _cts;

        public ObservableCollection<Equipment> Equipments => _repo.Equipments;

        /* 대시보드 요약 */
        public int TotalCount => Equipments.Count;
        public int ActiveCount => Equipments.Count(e => string.Equals(e.IsActive, "Y"));
        public int OverHeatCount => Equipments.Count(e => e.IsOverHeat);

        public MonitoringViewModel(EquipRepository equipRepository, MonitoringService mService)
        {
            // DB 최신 상태 5초마다 반영
            refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            refreshTimer.Tick += async (s, e) => await _repo.LoadAll();

            _repo = equipRepository;
            _mService = mService;

            StartMonitoring();
            // 5초 주기 DB 동기화 및 0.5초 온도 모니터링 루프 가동
        }

        public void RefreshDashboard()
        {
            OnPropertyChanged(nameof(TotalCount));
            OnPropertyChanged(nameof(ActiveCount));
            OnPropertyChanged(nameof(OverHeatCount));
        }

        /* 설비 온도 모니터링 시작 */

        private async void StartMonitoring()
        {
            refreshTimer.Start();

            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    // TODO : 이걸 필드 및 프로퍼티로 만들어도 될 듯
                    var activeEquips = Equipments
                        .Where(e => e.IsActive == "Y")
                        .ToList();

                    if (activeEquips.Count > 0)
                    {
                        foreach (var equip in activeEquips)
                        {
                            double newTemp = _mService.GetLiveTemp();

                            equip.CurrentTemp = newTemp;
                        }

                        RefreshDashboard();
                    }
                    await Task.Delay(TimeSpan.FromSeconds(0.5), token);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("모니터링이 종료되었습니다");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"모니터링 중 오류: {ex.Message}");
            }
        }

        /* 설비 온도 모니터링 종료 */

        public void StopMonitoring()
        {
            _cts?.Cancel();
        }
    }
}