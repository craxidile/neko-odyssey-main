using System;

namespace SpatiumInteractive.Libraries.Unity.GRU.Tests.Extension
{
    public static class DateTimeExtension
    {
        public static bool IsSelectedInDb(this DateTime dateTime)
        {
            if (dateTime == null)
            {
                return false;
            }
            else
            {
                if 
                (
                    (
                        dateTime.Day == 1 && 
                        dateTime.Month == 1 && 
                        dateTime.Year == 1
                    ) || 
                        dateTime == default)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsNotSelectedInDb(this DateTime dateTime)
        {
            return !IsSelectedInDb(dateTime);
        }

        public static bool IsSelectedInDb(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
            {
                return false;
            }
            else
            {
                return IsSelectedInDb(dateTime.Value);
            }
        }

        public static bool IsNotSelectedInDb(this DateTime? dateTime)
        {
            return !IsSelectedInDb(dateTime);
        }
    }
}
