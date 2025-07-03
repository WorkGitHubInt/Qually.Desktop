using System;
using System.Globalization;

namespace QuallyFlash
{
    public class SchemeTypeToEnableConverter : BaseValueConverter<SchemeTypeToEnableConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((SchemeType)value == SchemeType.HalfPair)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
