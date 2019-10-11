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
                return "Несколько часов";
            } else if (years == 0 && months == 1)
            {
                return $"{months} месяц";
            } else if (years == 0 && months > 1 && months <= 4)
            {
                return $"{months} месяца";
            }
            else if (years == 0 && months > 1 && months > 4)
            {
                return $"{months} месяцев";
            }
            else if (years == 1 && months <= 0)
            {
                return $"{years} год";
            }
            else if (years == 1 && years < 4 && months == 1)
            {
                return $"{years} год {months} месяц";
            }
            else if (years == 1 && years < 4 && months > 1 && months <= 4)
            {
                return $"{years} год {months} месяца";
            }
            else if (years == 1 && years < 4 && months > 4)
            {
                return $"{years} год {months} месяцев";
            }
            else if (years > 0 && years < 4 && months <= 0)
            {
                return $"{years} года";
            }
            else if (years > 0 && years < 4 && months == 1)
            {
                return $"{years} года {months} месяц";
            }
            else if (years > 0 && years < 4 && months > 1 && months <= 4)
            {
                return $"{years} года {months} месяца";
            }
            else if (years > 0 && years < 4 && months > 4)
            {
                return $"{years} года {months} месяцев";
            }
            else if (years > 5 && months <= 0)
            {
                return $"{years} лет";
            }
            else if (years > 5 && months == 1)
            {
                return $"{years} лет {months} месяц";
            }
            else if (years > 5 && months > 1 && months <= 4)
            {
                return $"{years} лет {months} месяца";
            }
            else
            {
                return $"{years} лет {months} месяцев";
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
