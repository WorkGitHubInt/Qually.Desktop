using System.Collections.Generic;

namespace QuallyFlash
{
    public class GlobalSettings : BaseModel
    {
        private static GlobalSettings instance;

        public GlobalSettings() { }

        public static GlobalSettings GetInstance()
        {
            if (instance == null)
            {
                instance = XmlHelper.LoadGlobalSettingsFromFile();
            }
            return instance;
        }

        public string Sort { get; set; } = "age";
        public List<string> MaleNames { get; set; } = new List<string>();
        public List<string> FemaleNames { get; set; } = new List<string>();
        public bool Pause { get; set; } = false;
        public Localization Localization { get; set; } = Localization.ru;
    }
}
