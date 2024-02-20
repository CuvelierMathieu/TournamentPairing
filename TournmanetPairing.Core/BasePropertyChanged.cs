using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TournmanetPairing.Core
{
    public abstract class BasePropertyChanged : INotifyPropertyChanged
    {
        private readonly Dictionary<string, object?> _propertyValues = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void Set<T>(T value, [CallerMemberName] string? propertyName = null)
        {
            ArgumentNullException.ThrowIfNull(propertyName);

            _propertyValues[propertyName] = value;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected T Get<T>([CallerMemberName] string? propertyName = null)
        {
            ArgumentNullException.ThrowIfNull(propertyName);

            if (_propertyValues.TryGetValue(propertyName, out object? value))
                return (T)value;

            return default;
        }
    }
}
