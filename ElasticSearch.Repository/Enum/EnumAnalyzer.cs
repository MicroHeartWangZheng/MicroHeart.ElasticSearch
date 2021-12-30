using System.ComponentModel;

namespace ElasticSearch.Repository.Enum
{
    public enum EnumAnalyzer
    {
        [Description("standard")]
        Standard,

        [Description("keyword")]
        Keyword,

        [Description("whitespace")]
        WhiteSpace,

        [Description("snowball")]
        SnowBall,

        [Description("ik_max_word")]
        IkMaxWord,

        [Description("ik_smart")]
        IkSmart
    }
}
