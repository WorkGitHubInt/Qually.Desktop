using System.ComponentModel;

namespace QuallyFlash
{
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
        OR = 13,
        [Description("Смесь")]
        Mash = 14,
    };

    public enum Specialization
    {
        [Description("Классика")]
        Classic = 0,
        [Description("Вестерн")]
        Western = 1,
    }

    public enum SchemeType
    {
        [Description("Полупара")]
        HalfPair = 0,
        [Description("Пара")]
        Pair = 1,
    }

    public enum HorsingEdge
    {
        [Description("60 НЛНП")]
        NLNP = 0,
        [Description("10 лет")]
        Year = 1,
    }

    public enum Limit
    {
        [Description("Кол-во ОР")]
        OR = 0,
        [Description("Кол-во лошадей")]
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

    public enum TrainType
    {
        [Description("В команде")]
        Team = 0,
        [Description("Без команды")]
        WithoutTeam = 1
    }
}
