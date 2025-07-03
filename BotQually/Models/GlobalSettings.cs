namespace BotQually
{
    public class GlobalSettings : BaseModel
    {
        public static GlobalSettings instance;

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
        public WorkType WorkType { get; set; } = WorkType.SingleOrder;
        public ClientType ClientType { get; set; } = ClientType.New;
        public bool ParallelHorse { get; set; } = false;
        public bool RandomPause { get; set; } = false;
        public bool Tray { get; set; } = true;
        public bool MoneyNotification { get; set; } = false;
        public Localization Localization { get; set; } = Localization.ru;
        public Settings Settings { get; set; } = new Settings();
    }
}
