using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace QuallyFlash
{
    public class SchemeConverter : BaseValueConverter<SchemeConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((ObservableCollection<Training>)value == null)
            {
                return MainViewModel.GetInstance().Account.Settings.Scheme;
            }
            else
            {
                return value;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
