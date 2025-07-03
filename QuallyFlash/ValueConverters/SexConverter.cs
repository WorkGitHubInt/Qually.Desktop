using System;
using System.Globalization;

namespace QuallyFlash
{
    public class SexConverter : BaseValueConverter<SexConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((HorseSex)value == HorseSex.Male)
            {
                return Properties.Resources.SexMale;
            }
            else
            {
                return Properties.Resources.SexFemale;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
