using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CustomerSettlement.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        protected void SetProperty<T>(Action<T> setter, T value, [CallerMemberName] string propertyName = "")
        {
            setter(value);
            OnPropertyChanged(propertyName);
        }
    }
}
