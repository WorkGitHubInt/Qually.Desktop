using System;
using System.Globalization;

namespace QuallyFlash
{
    public class AccTypeToStringConverter : BaseValueConverter<AccTypeToStringConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((AccountType)value == AccountType.Co)
            {
                return Properties.Resources.CoPostfix;
            } else
            {
                return "";
            }     
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
