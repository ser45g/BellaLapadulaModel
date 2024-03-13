using MultipleUserLoginForm.LocalizationHelper;
using MultipleUserLoginForm.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MultipleUserLoginForm.ValueConvertors
{
    public class EnumToStringConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null) { return null; }
            return LocalizedStrings.Instance[$"SecurityMark{value.ToString()}"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str=value as string;
            foreach(object enumValue in Enum.GetValues(typeof(SecurityMark)))
            {
                if(str== LocalizedStrings.Instance[$"SecurityMark{value.ToString()}"])
                {
                    return enumValue;
                }
            }
            return new Exception();
        }
    }
}
