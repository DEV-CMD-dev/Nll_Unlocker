using System.Windows.Media;
using System.Globalization;
using System.Windows.Data;

namespace Nll_Unlocker.Classes
{
    public class RiskToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int risk = (int)value;
            if (risk < 30) return Brushes.LightGreen;
            if (risk < 70) return Brushes.Yellow;
            return Brushes.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }
}
