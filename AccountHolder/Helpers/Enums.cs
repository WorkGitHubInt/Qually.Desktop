using AccountHolder.Properties;
using System.ComponentModel;

namespace AccountHolder
{
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

    public enum Result
    {
        None = 0,
        Success = 1,
        Error = 2,
        Empty = 3,
    }
}
