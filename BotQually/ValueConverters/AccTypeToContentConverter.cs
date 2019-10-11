﻿using System;
using System.Globalization;

namespace BotQually
{
    public class AccTypeToContentConverter : BaseValueConverter<AccTypeToContentConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if ((AccountType)value == AccountType.Co)
                {
                    return "Выйти из соу";
                }
                else
                {
                    return "Войти в соу";
                }
            } else
            {
                return "Войти в соу";
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
