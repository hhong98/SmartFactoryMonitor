using SmartFactoryMonitor.Model;
using SmartFactoryMonitor.Repository;
using SmartFactoryMonitor.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SmartFactoryMonitor.ViewModels
{
    public class EquipManageViewModel : ObservableModelBase
    {
        private readonly EquipRepository _repo;
        private readonly EquipService _eService;

        public ObservableCollection<Equipment> Equipments => _repo.Equipments;
        public ICollectionView FilteredEquipments { get; }
        public IEnumerable<Equipment> ActiveEquipments =>
            Equipments.Where(e => e.IsActive is "Y");
        public IEnumerable<Equipment> InactiveEquipments =>
            Equipments.Where(e => e.IsActive is "N");

        private Equipment selectedEquip;

        public Equipment SelectedEquip
        {
            get => selectedEquip;
            set
            {
                if (SetProperty(ref selectedEquip, value))
                {
                    EditingEquip = value is null
                        ? null
                        : Equipment.DTO.Convert(value);

                    OnPropertyChanged(nameof(FormHeaderTxt));
                }
            }
        }

        private Equipment.DTO editingEquip;

        public Equipment.DTO EditingEquip
        {
            get => editingEquip;
            set => SetProperty(ref editingEquip, value);
        }

        public string FormHeaderTxt =>
            SelectedEquip is null ? "설비를 선택하세요" :
            string.IsNullOrEmpty(SelectedEquip.EquipId) ? "새로운 설비 등록" :
            SelectedEquip.EquipName;

        private string searchTxt;

        public string SearchTxt
        {
            get => searchTxt;
            set
            {
                if (SetProperty(ref searchTxt, value))
                {
                    FilteredEquipments.Refresh();
                }
            }
        }

        private string filterOption;

        public string FilterOption
        {
            get => filterOption;
            set
            {
                if (SetProperty(ref filterOption, value))
                {
                    FilteredEquipments.Refresh();
                }
            }
        }

        public EquipManageViewModel(EquipRepository equipRepository, EquipService eService)
        {
            _repo = equipRepository;
            _eService = eService;

            FilteredEquipments = new ListCollectionView(Equipments);
            FilteredEquipments.Filter = FilterEquip;
        }

        #region Equipment Add, Update, Delete

        /* Equipment 생성 */

        public async Task AddEquip()
        {
            try
            {
                DbResult result = await _eService.Add(editingEquip);

                if (result.IsSuccess)
                {
                    ClearSelection();
                    MessageBox.Show("성공적으로 설비를 등록했습니다");
                }
                else
                {
                    MessageBox.Show($"등록 실패: {result.Message}");
                }

                await _repo.LoadAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"등록 중 오류 발생: {ex.Message}");
            }
        }

        /* Equipment 수정 */

        public async Task UpdateCurrentEquip()
        {
            if (selectedEquip is null || editingEquip is null) return;

            if (MessageBox.Show($"수정하시겠습니까?", "수정", MessageBoxButton.YesNo)
                is MessageBoxResult.Yes)
            {
                try
                {
                    var changedColumns = Equipment.DTO.GetChangedColumns(
                        Equipment.DTO.Convert(selectedEquip),
                        editingEquip);

                    if (changedColumns.Count is 0)
                    {
                        MessageBox.Show("변경사항이 없습니다\n다시 시도해주세요");
                        return;
                    }

                    DbResult result = await _eService.UpdateChangedColumns(selectedEquip.EquipId, changedColumns);
                    if (result.IsSuccess)
                    {
                        MessageBox.Show("수정 성공했습니다");

                        editingEquip.Apply(selectedEquip);
                        FilteredEquipments.Refresh();
                    }
                    else
                    {
                        MessageBox.Show($"수정 실패: {result.Message}");
                    }
                    await _repo.LoadAll();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"수정 중 오류 발생: {ex.Message}");
                }
            }
        }

        public async Task UpdateEquip(Equipment selectedEquip)
        {
            try
            {
                DbResult result = await _eService.UpdateAllColumns(selectedEquip);
                if (result.IsSuccess)
                {
                    MessageBox.Show("수정 성공했습니다");
                }
                else
                {
                    MessageBox.Show($"수정 실패: {result.Message}");
                }

                await _repo.LoadAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"수정 중 오류 발생: {ex.Message}");
            }
        }

        /* Equipment 삭제 */

        public async Task DeleteCurrentEquip()
        {
            if (selectedEquip is null) return;
            if (selectedEquip.IsActive is "Y")
            {
                MessageBox.Show("사용중인 설비는 삭제할 수 없습니다\n먼저 사용을 중단해주세요");
                return;
            }

            var equipName = selectedEquip.EquipName;

            if (MessageBox.Show($"{equipName}을(를) 삭제하시겠습니까?", "삭제", MessageBoxButton.YesNo)
                is MessageBoxResult.Yes)
            {
                try
                {
                    DbResult result = await _eService.Delete(new List<string> { selectedEquip.EquipId });

                    if (result.IsSuccess)
                    {
                        ClearSelection();
                        MessageBox.Show($"{equipName}이/가 성공적으로 삭제되었습니다.");
                    }
                    else
                    {
                        MessageBox.Show($"삭제 실패: {result.Message}");
                    }

                    await _repo.LoadAll();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"삭제 중 오류 발생: {ex.Message}");
                }
            }
        }

        public async Task DeleteSelectedEquips()
        {
            var selectedEquips = Equipments
                .Where(e => e.IsChecked)
                .ToList();

            if (selectedEquips.Count is 0)
            {
                MessageBox.Show("삭제할 설비를 선택하세요");
                return;
            }
            if (selectedEquips.Any(e => e.IsActive is "Y"))
            {
                MessageBox.Show("사용중인 설비가 포함되어있습니다\n먼저 사용을 중단해주세요");
                return;
            }

            if (MessageBox.Show($"{selectedEquips.Count}개의 설비를 삭제하시겠습니까?", "삭제 확인", MessageBoxButton.YesNo)
                is MessageBoxResult.No) { return; }

            try
            {
                DbResult result = await _eService.Delete(
                    selectedEquips.Select(e => e.EquipId).ToList());

                if (result.IsSuccess)
                {
                    MessageBox.Show($"{result.AffectedRows}건이 성공적으로 삭제되었습니다.");
                }
                else
                {
                    MessageBox.Show($"삭제 실패: {result.Message}");
                }

                await _repo.LoadAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"삭제 중 오류 발생: {ex.Message}");
            }
        }

        #endregion Equipment Add, Update, Delete

        #region Utils

        public void ResetSelection()
         => SelectedEquip = new Equipment();

        public void ClearSelection()
        {
            SelectedEquip = null;
            EditingEquip = null;
        }

        private bool FilterEquip(object obj)
        {
            if (!(obj is Equipment equip)) return false;

            return MatchEquip(equip);
        }

        private bool MatchEquip(Equipment equip)
        {
            bool matchSearch = string.IsNullOrWhiteSpace(SearchTxt) ||
              equip.EquipName.IndexOf(SearchTxt, StringComparison.OrdinalIgnoreCase) >= 0;

            bool matchActive = string.IsNullOrWhiteSpace(FilterOption) ||
                FilterOption is "전체";

            if (!matchActive)
            {
                string isActive = Common.Utils.ConvertToActive(FilterOption);
                matchActive = equip.IsActive == isActive;
            }

            return matchSearch && matchActive;
        }

        public bool ValidateForm(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(editingEquip.EquipName))
            {
                errorMessage = "설비명을 입력해주세요";
                return false;
            }
            if (string.IsNullOrWhiteSpace(editingEquip.IpAddress))
            {
                errorMessage = "IP 주소를 입력해주세요";
                return false;
            }
            if (editingEquip.Port <= 0 || editingEquip.Port is 0)
            {
                errorMessage = "올바른 포트 번호를 입력해주세요";
                return false;
            }
            if (editingEquip.MinTemp > editingEquip.MaxTemp)
            {
                errorMessage = "올바른 온도 범위를 입력해주세요";
                return false;
            }
            if (string.IsNullOrWhiteSpace(editingEquip.Location))
            {
                errorMessage = "설치 위치를 입력해주세요";
                return false;
            }

            return true;
        }

        #endregion Utils
    }
}