using Microsoft.VisualStudio.Shell;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VS.Common.DoctestTestAdapter.Options
{
    public abstract class BaseOptionsPage : DialogPage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (field.Equals(value))
            {
                return;
            }

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
