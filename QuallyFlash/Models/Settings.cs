using System.Collections.ObjectModel;
using System;

namespace QuallyFlash
{
    public class Settings : BaseModel, ICloneable 
    {
        #region Общие
        public bool Game { get; set; } = false;
        public bool MissionAfter2 { get; set; } = false;
        public bool MissionAfterTrain { get; set; } = false;
        public bool Abortion { get; set; } = true;
        public Limit LimitType { get; set; } = QuallyFlash.Limit.OR;
        public int Limit { get; set; } = 0;
        public HorsingEdge HorsingEdge { get; set; } = HorsingEdge.NLNP;
        public double HealthEdge { get; set; } = 90;
        public bool Pause { get; set; } = false;
        #endregion

        #region Training
        public Specialization Specialization { get; set; } = Specialization.Classic;
        public int Amunition { get; set; } = 1;
        public bool Headrest { get; set; } = false;
        public bool Bandages { get; set; } = false;
        public bool Whip { get; set; } = false;
        public bool Heavy { get; set; } = false;
        public SchemeType SchemeType { get; set; } = SchemeType.HalfPair;
        public bool ParallelPair { get; set; } = false;
        public TrainType TrainType { get; set; } = TrainType.Team;
        public ObservableCollection<Training> Scheme { get; set; } = new ObservableCollection<Training>();
        #endregion

        #region Horsing
        public int HorsingNum { get; set; } = 0;
        public string MaleName { get; set; } = "Муж";
        public string FemaleName { get; set; } = "Жен";
        public string Affix { get; set; } = string.Empty;
        public string Farm { get; set; } = string.Empty;
        public string NameSkill { get; set; } = "vitality";
        public bool RandomNames { get; set; } = false;
        public bool DisableGP { get; set; } = true;
        #endregion

        #region Center
        public string Duration { get; set; } = "3";
        public bool Reserve { get; set; } = false;
        public string ReserveID { get; set; } = string.Empty;
        public bool SelfReserve { get; set; } = false;
        public bool Carrot { get; set; } = false;
        public bool Mash { get; set; } = false;
        public bool Saddle { get; set; } = false;
        public bool Bridle { get; set; } = false;
        public bool Forest { get; set; } = false;
        public bool Mountain { get; set; } = false;
        public bool Beach { get; set; } = false;
        public bool Ramp { get; set; } = false;
        public bool Shower { get; set; } = true;
        public bool Bowl { get; set; } = true;
        public bool WriteOut { get; set; } = false;
        #endregion

        public bool BuyFood { get; set; } = false;
        public ProductType MainProductToSell { get; set; } = ProductType.None;
        public ProductType SubProductToSell { get; set; } = ProductType.None;
        public bool BuyCarrotMash { get; set; } = false;

        public object Clone()
        {
            return MemberwiseClone() as Settings;
        }
    }
}
