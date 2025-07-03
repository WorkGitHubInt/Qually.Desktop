namespace BotQually
{
    public class Settings : BaseModel
    {
        #region Случки Ж
        public bool HorsingFemale { get; set; } = false;
        public string HorsingFemalePrice { get; set; } = "500";
        public string Breeder { get; set; } = string.Empty;
        public bool ClearBlood { get; set; } = false;
        public bool SelfMale { get; set; } = false;
        public bool BuyWheat { get; set; } = false;
        public bool HorsingFemaleCommand { get; set; } = false;
        public string GPEdge { get; set; } = "1000";
        #endregion

        #region Случки М
        public bool HorsingMale { get; set; } = false;
        public bool HorsingMaleCommand { get; set; } = false;
        public string HorsingMalePrice { get; set; } = "500";
        public bool Carrot { get; set; } = false;
        #endregion

        #region Роды
        public string MaleName { get; set; } = "Муж";
        public string FemaleName { get; set; } = "Жен";
        public string Affix { get; set; } = string.Empty;
        public string Farm { get; set; } = string.Empty;
        public bool RandomNames { get; set; } = false;
        #endregion

        #region КСК
        public string CentreDuration { get; set; } = "3";
        public bool CentreHay { get; set; } = false;
        public bool CentreOat { get; set; } = false;
        #endregion

        #region Резерв КСК
        public string ReserveID { get; set; } = string.Empty;
        public string ReserveDuration { get; set; } = string.Empty;
        public string ContinueDuration { get; set; } = string.Empty;
        public bool SelfReserve { get; set; } = false;
        public bool WriteToAll { get; set; } = false;
        public bool Continue { get; set; } = false;
        #endregion

        #region Покупка/Продажа
        public string BuyHay { get; set; } = "500";
        public string BuyOat { get; set; } = "500";
        public ProductType MainProductToSell { get; set; } = ProductType.Wheat;
        public ProductType SubProductToSell { get; set; } = ProductType.None;
        public bool SellShit { get; set; } = false;
        #endregion

        public bool Mission { get; set; } = false;
        public bool OldHorses { get; set; } = false;
        public string HealthEdge { get; set; } = "0";
        public int SkipIndex { get; set; } = 0;
        public bool LoadSleep { get; set; } = true;
        public bool GoBabies { get; set; } = false;
        public bool Stroke { get; set; } = false;
        public bool MissionOld { get; set; } = false;
        public bool Sharing { get; set; } = false;

        public Settings Copy()
        {
            return MemberwiseClone() as Settings;
        }
    }
}
