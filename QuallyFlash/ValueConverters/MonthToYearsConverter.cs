using System;
using System.Globalization;

namespace QuallyFlash
{
    public class MonthToYearsConverter : BaseValueConverter<MonthToYearsConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int intValue = (int)value;
            int years = intValue / 12;
            int months = intValue % 12;
            if (years == 0 && months <= 0)
            {
                return Properties.Resources.TimeFewHours;
            } else if (years == 0 && months == 1)
            {
                return $"{months} {Properties.Resources.TimeMonth1}";
            } else if (years == 0 && months > 1 && months <= 4)
            {
                return $"{months} {Properties.Resources.TimeMonth2}";
            }
            else if (years == 0 && months > 1 && months > 4)
            {
                return $"{months} {Properties.Resources.TimeMonth3}";
            }
            else if (years == 1 && months <= 0)
            {
                return $"{years} {Properties.Resources.TimeYear1}";
            }
            else if (years == 1 && years < 4 && months == 1)
            {
                return $"{years} {Properties.Resources.TimeYear1} {months} {Properties.Resources.TimeMonth1}";
            }
            else if (years == 1 && years < 4 && months > 1 && months <= 4)
            {
                return $"{years} {Properties.Resources.TimeYear1} {months} {Properties.Resources.TimeMonth2}";
            }
            else if (years == 1 && years < 4 && months > 4)
            {
                return $"{years} {Properties.Resources.TimeYear1} {months} {Properties.Resources.TimeMonth3}";
            }
            else if (years > 0 && years < 4 && months <= 0)
            {
                return $"{years} {Properties.Resources.TimeYear2}";
            }
            else if (years > 0 && years < 4 && months == 1)
            {
                return $"{years} {Properties.Resources.TimeYear2} {months} {Properties.Resources.TimeMonth1}";
            }
            else if (years > 0 && years < 4 && months > 1 && months <= 4)
            {
                return $"{years} {Properties.Resources.TimeYear2} {months} {Properties.Resources.TimeMonth2}";
            }
            else if (years > 0 && years < 4 && months > 4)
            {
                return $"{years} {Properties.Resources.TimeYear2} {months} {Properties.Resources.TimeMonth3}";
            }
            else if (years > 5 && months <= 0)
            {
                return $"{years} {Properties.Resources.TimeYear3}";
            }
            else if (years > 5 && months == 1)
            {
                return $"{years} {Properties.Resources.TimeYear3} {months} {Properties.Resources.TimeMonth1}";
            }
            else if (years > 5 && months > 1 && months <= 4)
            {
                return $"{years} {Properties.Resources.TimeYear3} {months} {Properties.Resources.TimeMonth2}";
            }
            else
            {
                return $"{years} {Properties.Resources.TimeYear3} {months} {Properties.Resources.TimeMonth3}";
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
