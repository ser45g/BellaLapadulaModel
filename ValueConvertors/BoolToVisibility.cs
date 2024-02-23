using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MultipleUserLoginForm.ValueConvertors
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(targetType != typeof(Visibility)) { return null; }
            bool fromBoolValue=bool.Parse(value.ToString());
            return (fromBoolValue == true ? Visibility.Visible : Visibility.Collapsed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool)) { return null; }
            Visibility visibility = (Visibility)Enum.Parse(typeof(Visibility),value.ToString());
            return (visibility == Visibility.Visible? true: false);
        }
    }
}
