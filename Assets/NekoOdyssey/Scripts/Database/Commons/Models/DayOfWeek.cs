using System;
using System.ComponentModel;
using UnityEngine;

namespace NekoOdyssey.Scripts.Database.Commons.Models
{
    public enum DayOfWeek
    {
        [Description("Su")] 
        Sunday,
        [Description("Mo")] 
        Monday,
        [Description("Tu")] 
        Tuesday,
        [Description("We")] 
        Wednesday,
        [Description("We")] 
        Thursday,
        [Description("Fr")] 
        Friday,
        [Description("Sa")] 
        Saturday,
    }


    public static class DayOfWeekExtensions
    {
        public static string GetShortName(this DayOfWeek value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field == null) return value.ToString();

            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
            {
                Debug.Log($">>test_npc<< day_of_week_desc {attribute.Description}");
                return attribute.Description;
            }

            return value.ToString();
        }
    }
}