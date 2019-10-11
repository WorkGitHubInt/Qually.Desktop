namespace BotQually
{
    public class GlobalSettings : BaseModel
    {
        public string[] MaleNames;
        public string[] FemaleNames;
        public string Sort { get; set; } = "age";
        public WorkType WorkType { get; set; } = WorkType.SingleOrder;
        public ClientType ClientType { get; set; } = ClientType.New;
        public bool ParallelHorse { get; set; } = false;
        public bool RandomPause { get; set; } = false;
        public bool Tray { get; set; } = true;
        public Settings Settings { get; set; } = new Settings();
    }
}
