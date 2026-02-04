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
    public class EquipManageViewModel
    {
        private readonly EquipRepository _repo;
        private readonly EquipService _eService;

        public ObservableCollection<Equipment> Equipments => _repo.Equipments;

        public EquipManageViewModel(EquipRepository equipRepository, EquipService eService)
        {
            _repo = equipRepository;
            _eService = eService;
        }

        /* Equipment 생성 */

        public async Task AddEquip(Equipment.Add_DTO newEquip)
        {
            try
            {
                DbResult result = await _eService.Add(newEquip);

                if (result.IsSuccess)
                {
                    await _repo.LoadAll();
                    MessageBox.Show("성공적으로 설비를 등록했습니다");
                }
                else
                {
                    MessageBox.Show($"등록 실패: {result.Message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"등록 중 오류 발생: {ex.Message}");
            }
        }

        /* Equipment 수정 */

        public async Task UpdateEquip(Equipment selectedEquip)
        {
            try
            {
                DbResult result = await _eService.Update(selectedEquip);
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

        public async Task DeleteEquip()
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

            if (MessageBox.Show($"{selectedIds.Count}개의 설비를 삭제하시겠습니까?",
                "삭제 확인", MessageBoxButton.YesNo) == MessageBoxResult.No) { return; }

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
    }
}