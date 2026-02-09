using Newtonsoft.Json;
using SmartFactoryMonitor.Model;
using SmartFactoryMonitor.Repository;
using SmartFactoryMonitor.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
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

        public ICollectionView FilteredMonitors { get; }

        /* 대시보드 요약 */
        public int TotalCount => Equipments.Count;
        public int ActiveCount => Equipments.Count(e => e.IsActive == "Y");
        public int StableCount => ActiveCount - (WarnCount + DangerCount + DisConnectCount);
        public int WarnCount => Equipments.Count(e => e.IsActive == "Y" && e.Status == "WARN");
        public int DangerCount => Equipments.Count(e => e.IsActive == "Y" && e.Status == "ERROR");
        public int DisConnectCount => Equipments.Count(e => e.IsActive == "Y" && e.Status == "NO DATA");

        public DateTime WorkStart => DateTime.Today.Add(Properties.Settings.Default.WorkStartTime);
        public DateTime WorkEnd => DateTime.Today.Add(Properties.Settings.Default.WorkEndTime);
        private DateTime LunchStart => DateTime.Today.Add(Properties.Settings.Default.LunchStartTime);
        private DateTime LunchEnd => DateTime.Today.Add(Properties.Settings.Default.LunchEndTime);

        public double PlannedTime
        {
            get
            {
                var workTime = (WorkEnd - WorkStart).TotalMinutes;
                var lunchTime = (LunchEnd - LunchStart).TotalMinutes;
                return Math.Max(workTime - lunchTime, 1);
            }
        }

        private Equipment selectedMonitor;

        public Equipment SelectedMonitor
        {
            get => selectedMonitor;
            set
            {
                if (SetProperty(ref selectedMonitor, value))
                {
                    OnPropertyChanged(nameof(FormHeaderTxt));
                }
            }
        }

        public string FormHeaderTxt
        {
            get
            {
                if (SelectedMonitor is null) return "설비를 선택하세요";
                if (string.IsNullOrEmpty(SelectedMonitor.EquipId)) return "정보 조회 실패";
                return SelectedMonitor.EquipName;
            }
        }

        private string searchTxt;

        public string SearchTxt
        {
            get => searchTxt;
            set
            {
                if (SetProperty(ref searchTxt, value))
                {
                    FilteredMonitors.Refresh();
                }
            }
        }

        public MonitoringViewModel(EquipRepository equipRepository, MonitoringService mService)
        {
            //임시로 세팅값 초기화하는 경우 사용! (평소에는 주석처리)
            //Properties.Settings.Default.Reset();
            //Properties.Settings.Default.Reload();

            _repo = equipRepository;
            _mService = mService;

            FilteredMonitors = new ListCollectionView(Equipments);
            FilteredMonitors.Filter = FilterMonitors;

            // DB 최신 상태 5초마다 반영
            refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            refreshTimer.Tick += async (s, e) => await _repo.LoadAll();

            // 5초 주기 DB 동기화 및 0.5초 온도 모니터링 루프 가동
            StartMonitoring();
        }

        public void RefreshDashboard()
        {
            FilteredMonitors?.Refresh();

            OnPropertyChanged(nameof(TotalCount));
            OnPropertyChanged(nameof(ActiveCount));
            OnPropertyChanged(nameof(StableCount));
            OnPropertyChanged(nameof(WarnCount));
            OnPropertyChanged(nameof(DangerCount));
            OnPropertyChanged(nameof(DisConnectCount));
        }

        private bool FilterMonitors(object obj)
        {
            if (!(obj is Equipment equip)) return false;
            if (equip.IsActive != "Y") return false;
            if (string.IsNullOrWhiteSpace(SearchTxt)) return true;

            return equip.EquipName.IndexOf(SearchTxt, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        // 설비 온도 모니터링 시작
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
                    var activeEquips = Equipments
                        .Where(e => e.IsActive is "Y")
                        .ToList();

                    if (activeEquips.Count > 0)
                    {
                        var equipTempInfoList = await _mService.GetLatestTemp(activeEquips.Select(ae => ae.EquipId).ToList());

                        foreach (var equip in activeEquips)
                        {
                            var info = equipTempInfoList
                                .FirstOrDefault(info => info.equipId == equip.EquipId);

                            if (info.equipId == null || info.status == "NO DATA")
                            {
                                equip.Status = "NO DATA";
                                continue;
                            }

                            equip.CurrentTemp = info.temperature;
                            equip.Status = info.status;
                            equip.LastUpdateTime = info.logTime;
                            equip.RefreshUpdateTime();

                            if (DateTime.Now > WorkStart && equip.Status != "NO DATA")
                                UpdateOperatingMetrics(equip);
                        }

                        if (DateTime.Now.Second is 0) SaveCurrentData();
                        RefreshDashboard();
                    }
                    await Task.Delay(TimeSpan.FromSeconds(1), token);
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

        // 설비 온도 모니터링 종료
        public void StopMonitoring()
        {
            _cts?.Cancel();
        }

        private void UpdateOperatingMetrics(Equipment equip)
        {
            equip.TotalRuntime += TimeSpan.FromSeconds(1);
            equip.OperatingRate = Math.Min(Math.Round(equip.TotalRuntime.TotalMinutes / PlannedTime * 100, 1), 100);
        }

        public void SaveCurrentData()
        {
            try
            {
                var data = Equipments.ToDictionary(equip => equip.EquipId, equip => equip.TotalRuntime.TotalSeconds);

                Properties.Settings.Default.SavedRuntimeData = JsonConvert.SerializeObject(data);
                Properties.Settings.Default.LastSaveDate = DateTime.Today;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"저장 실패: {ex.Message}");
            }
        }

        public void LoadSavedData()
        {
            if (Properties.Settings.Default.LastSaveDate.Date != DateTime.Today) return;

            string json = Properties.Settings.Default.SavedRuntimeData;
            if (string.IsNullOrEmpty(json)) return;

            try
            {
                var data = JsonConvert.DeserializeObject<Dictionary<string, double>>(json);

                foreach (var equip in Equipments)
                {
                    if (data.ContainsKey(equip.EquipId))
                    {
                        equip.TotalRuntime = TimeSpan.FromSeconds(data[equip.EquipId]);
                    }
                }
            }
            catch
            {
                Properties.Settings.Default.SavedRuntimeData = "";
                Properties.Settings.Default.Save();
            }
        }
    }
}