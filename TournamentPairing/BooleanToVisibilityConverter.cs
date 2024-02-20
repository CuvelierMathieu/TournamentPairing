using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TournamentPairing
{
    internal class BooleanToVisibilityConverter : IValueConverter
    {
        public Visibility WhenTrue { get; set; }
        public Visibility WhenFalse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool boolean)
                return DependencyProperty.UnsetValue;

            if (boolean)
                return WhenTrue;

            return WhenFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
