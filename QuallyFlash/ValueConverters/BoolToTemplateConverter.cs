using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace QuallyFlash
{
    public class BoolToTemplateConverter : BaseValueConverter<BoolToTemplateConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return Application.Current.Resources["MultiLineTabControl"] as ControlTemplate;
            }
            else
            {
                return Application.Current.Resources["ScrollTabControl"] as ControlTemplate;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
