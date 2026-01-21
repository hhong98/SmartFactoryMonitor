using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SmartFactoryMonitor.Model
{
    public abstract class ObservableModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            /* 
            * ref T storage -> 변수의 주소, 값 복사 x
            * T -> 제네릭 타입, 여러가지 자료형의 속성 처리
            * [CallerMemberName] -> C# Attribute 중 하나, 호출한 속성 이름 추출
            */

            if (Equals(storage, value)) return false; // 값이 기존값과 동일한 경우 갱신 x
            storage = value; // 새로운 값을 실제 변수(ex) _isActive)에 저장
            OnPropertyChanged(propertyName); // UI에 값 변경 알림
            return true; // 값 변경, 이후 값 변경 시 수행할 로직 응용 가능
        }
    }
}
