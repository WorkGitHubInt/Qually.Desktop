using QuallyFlash.Properties;
using System.ComponentModel;

namespace QuallyFlash
{
    public enum AccountType { Normal = 0, Co = 1 }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum Server
    {
        [LocalizedDescription("ServerAustralia", typeof(Resources))]
        Australia = 0,
        [LocalizedDescription("ServerEngland", typeof(Resources))]
        England = 1,
        [LocalizedDescription("ServerArabic", typeof(Resources))]
        Arabic = 2,
        [LocalizedDescription("ServerBulgaria", typeof(Resources))]
        Bulgaria = 3,
        [LocalizedDescription("ServerInternational", typeof(Resources))]
        International = 4,
        [LocalizedDescription("ServerSpain", typeof(Resources))]
        Spain = 5,
        [LocalizedDescription("ServerCanada", typeof(Resources))]
        Canada = 6,
        [LocalizedDescription("ServerGermany", typeof(Resources))]
        Germany = 7,
        [LocalizedDescription("ServerNorway", typeof(Resources))]
        Norway = 8,
        [LocalizedDescription("ServerPoland", typeof(Resources))]
        Poland = 9,
        [LocalizedDescription("ServerRussia", typeof(Resources))]
        Russia = 10,
        [LocalizedDescription("ServerRomain", typeof(Resources))]
        Romain = 11,
        [LocalizedDescription("ServerUSA", typeof(Resources))]
        USA = 12,
        [LocalizedDescription("ServerFranceOuranos", typeof(Resources))]
        FranceOuranos = 13,
        [LocalizedDescription("ServerFranceGaia", typeof(Resources))]
        FranceGaia = 14,
        [LocalizedDescription("ServerSweden", typeof(Resources))]
        Sweden = 15,
    }

    public enum HorseSex { Male, Female };

    public enum Result
    {
        None = 0,
        Success = 1,
        Error = 2,
        Empty = 3,
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum ProductType
    {
        [LocalizedDescription("None", typeof(Resources))]
        None = 0,
        [LocalizedDescription("Hay", typeof(Resources))]
        Hay = 1,
        [LocalizedDescription("Oat", typeof(Resources))]
        Oat = 2,
        [LocalizedDescription("Wheat", typeof(Resources))]
        Wheat = 3,
        [LocalizedDescription("Shit", typeof(Resources))]
        Shit = 4,
        [LocalizedDescription("Leather", typeof(Resources))]
        Leather = 5,
        [LocalizedDescription("Apples", typeof(Resources))]
        Apples = 6,
        [LocalizedDescription("Carrot", typeof(Resources))]
        Carrot = 7,
        [LocalizedDescription("Wood", typeof(Resources))]
        Wood = 8,
        [LocalizedDescription("Steel", typeof(Resources))]
        Steel = 9,
        [LocalizedDescription("Sand", typeof(Resources))]
        Sand = 10,
        [LocalizedDescription("Straw", typeof(Resources))]
        Straw = 11,
        [LocalizedDescription("Flax", typeof(Resources))]
        Flax = 12,
        [LocalizedDescription("OR", typeof(Resources))]
        OR = 13,
        [LocalizedDescription("Mash", typeof(Resources))]
        Mash = 14
    };

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum Specialization
    {
        [LocalizedDescription("SpecializationClassic", typeof(Resources))]
        Classic = 0,
        [LocalizedDescription("SpecializationWestern", typeof(Resources))]
        Western = 1,
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum SchemeType
    {
        [LocalizedDescription("SchemeTypeHalfPair", typeof(Resources))]
        HalfPair = 0,
        [LocalizedDescription("SchemeTypePair", typeof(Resources))]
        Pair = 1,
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum HorsingEdge
    {
        [LocalizedDescription("HorsingEdge60NLNP", typeof(Resources))]
        NLNP = 0,
        [LocalizedDescription("HorsingEdge10Years", typeof(Resources))]
        Year = 1,
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum Limit
    {
        [LocalizedDescription("LimitOR", typeof(Resources))]
        OR = 0,
        [LocalizedDescription("LimitHorses", typeof(Resources))]
        Horses = 1,
    }

    public enum EquipmentType
    {
        SaddleClassic1 = 0,
        SaddleClassic2 = 1,
        SaddleClassic3 = 2,
        SaddleWestern1 = 3,
        SaddleWestern2 = 4,
        SaddleWestern3 = 5,
        BridleClassic1 = 6,
        BridleClassic2 = 7,
        BridleClassic3 = 8,
        BridleWestern1 = 9,
        BridleWestern2 = 10,
        BridleWestern3 = 11,
        RampClassic = 12,
        RampWestern = 13,
        Bandages = 14,
        Forehead = 15,
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum TrainType
    {
        [LocalizedDescription("TrainTypeTeam", typeof(Resources))]
        Team = 0,
        [LocalizedDescription("TrainTypeWithoutTeam", typeof(Resources))]
        WithoutTeam = 1
    }

    public enum Localization
    {
        ru = 0,
        en = 1
    }
}
