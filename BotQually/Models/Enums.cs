using System.ComponentModel;

namespace BotQually
{
    public enum WorkType
    {
        [Description("Очередь с разными настройками")]
        SingleOrder = 0,
        [Description("Очередь с одинаковыми настройками")]
        GlobalOrder = 1,
        [Description("Параллельно с разными настройками")]
        SingleParallel = 2,
        [Description("Параллельно с одинаковыми настройками")]
        GlobalParallel = 3
    }

    public enum AccountType { Normal = 0, Co = 1 }

    public enum Server
    {
        [Description("Австралия")]
        Australia = 0,
        [Description("Англия")]
        England = 1,
        [Description("Арабский")]
        Arabic = 2,
        [Description("Болгария")]
        Bulgaria = 3,
        [Description("Интернациональный")]
        International = 4,
        [Description("Испания")]
        Spain = 5,
        [Description("Канада")]
        Canada = 6,
        [Description("Немецкий")]
        Germany = 7,
        [Description("Норвегия")]
        Norway = 8,
        [Description("Польша")]
        Poland = 9,
        [Description("Россия")]
        Russia = 10,
        [Description("Румыния")]
        Romain = 11,
        [Description("США")]
        USA = 12,
        [Description("Франция Ouranos")]
        FranceOuranos = 13,
        [Description("Франция Gaia")]
        FranceGaia = 14,
        [Description("Швеция")]
        Sweden = 15,
    }

    public enum HorseSex { Male, Female };

    public enum ClientType
    {
        [Description("Новый")]
        New = 0,
        [Description("Старый")]
        Old = 1,
    }

    public enum Result
    {
        None = 0,
        Success = 1,
        Error = 2,
        Empty = 3,
    }

    public enum ProductType
    {
        [Description("Не продавать")]
        None = 0,
        [Description("Фураж")]
        Hay = 1,
        [Description("Овес")]
        Oat = 2,
        [Description("Пшеница")]
        Wheat = 3,
        [Description("Говно")]
        Shit = 4,
        [Description("Кожа")]
        Leather = 5,
        [Description("Яблоки")]
        Apples = 6,
        [Description("Морковь")]
        Carrot = 7,
        [Description("Дерево")]
        Wood = 8,
        [Description("Железо")]
        Steel = 9,
        [Description("Песок")]
        Sand = 10,
        [Description("Солома")]
        Straw = 11,
        [Description("Лен")]
        Flax = 12,
        [Description("ОР")]
        OR = 13
    };
}
