using System;
using System.Windows.Markup;

namespace BotQually
{
    public class EnumValues : MarkupExtension
    {
        private readonly Type _enumType;

        public EnumValues(Type enumType)
        {
            _enumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(_enumType);
        }
    }
}
