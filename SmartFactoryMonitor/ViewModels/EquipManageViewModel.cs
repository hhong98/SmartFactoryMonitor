using SmartFactoryMonitor.Model;
using SmartFactoryMonitor.Repository;
using SmartFactoryMonitor.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SmartFactoryMonitor.ViewModels
{
    public class EquipManageViewModel : ObservableModelBase
    {
        private readonly EquipRepository _repo;
        private readonly EquipService _eService;

        public ObservableCollection<Equipment> Equipments => _repo.Equipments;

        private Equipment selectedEquip;

        public Equipment SelectedEquip
        {
            get => selectedEquip;
            set
            {
                if (SetProperty(ref selectedEquip, value))
                {
                    if (value != null)
                        EditingEquip = Equipment.DTO.Convert(value);

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

        public string FormHeaderTxt
        {
            get
            {
                if (SelectedEquip is null) return "설비를 선택하세요";
                if (string.IsNullOrEmpty(SelectedEquip.EquipId)) return "새로운 설비 등록";
                return SelectedEquip.EquipName;
            }
        }

        public EquipManageViewModel(EquipRepository equipRepository, EquipService eService)
        {
            _repo = equipRepository;
            _eService = eService;
        }

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
                        ClearSelection();
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
            var selectedIds = Equipments
                .Where(e => e.IsChecked)
                .Select(e => e.EquipId)
                .ToList();

            if (selectedIds.Count == 0)
            {
                MessageBox.Show("삭제할 설비를 선택하세요");
                return;
            }

            if (MessageBox.Show($"{selectedIds.Count}개의 설비를 삭제하시겠습니까?", "삭제 확인", MessageBoxButton.YesNo)
                is MessageBoxResult.No) { return; }

            try
            {
                DbResult result = await _eService.Delete(selectedIds);

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

        public void ResetSelection()
         => SelectedEquip = new Equipment();

        public void ClearSelection()
        {
            SelectedEquip = null;
            EditingEquip = null;
        }
    }
}