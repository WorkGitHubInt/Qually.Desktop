using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountHolder
{
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

    public enum Result
    {
        None = 0,
        Success = 1,
        Error = 2,
        Empty = 3,
    }
}
