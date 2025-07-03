using System;
using System.Globalization;

namespace QuallyFlash
{
    public class AccTypeToContentConverter : BaseValueConverter<AccTypeToContentConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((AccountType)value == AccountType.Co)
            {
                return Properties.Resources.LogoutCoBtn;
            }
            else
            {
                return Properties.Resources.LoginCoBtn;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
