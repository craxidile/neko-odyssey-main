using System;

namespace NekoOdyssey.Scripts.Extensions
{
    public static class EnumExtensions
    {
        public static string ToText(this Enum eff)
        {
            return Enum.GetName(eff.GetType(), eff);
        }

        public static T ToEnum<T>(this string enumValue)  
        {
            return (T) Enum.Parse(typeof(T), enumValue);
        }
    }
}