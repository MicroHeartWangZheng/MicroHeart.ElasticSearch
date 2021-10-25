using System;
using System.ComponentModel;

namespace ElasticSearch.Repository.Extensions
{
    public static class EnumExtensions
    {
        public static string ToDescription(this System.Enum @enum)
        {
            if (@enum == null)
                return string.Empty;
            var type = @enum.GetType();
            var f = type.GetField(@enum.ToString());
            if (f == null)
                return @enum.ToString();
            var da = (DescriptionAttribute)Attribute.GetCustomAttribute(f, typeof(DescriptionAttribute));
            return da?.Description ?? @enum.ToString();
        }

        public static int ToInt(this System.Enum e)
        {
            return e.GetHashCode();
        }
    }
}
