using System.ComponentModel;

namespace AccountHolder
{
    public enum Localization
    {
        [Description("Русский")]
        ru = 0,
        [Description("English")]
        en = 1
    }

    public class Settings
    {
        public Localization Localization { get; set; } = Localization.ru;
    }
}
