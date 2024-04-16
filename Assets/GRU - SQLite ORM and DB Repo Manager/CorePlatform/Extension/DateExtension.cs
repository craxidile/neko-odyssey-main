using System;

namespace SpatiumInteractive.Libraries.Unity.Platform.Extensions
{
    public static class DateExtension
    {
        #region Public Methods

        public static bool IsEqualTo(this DateTime d1, DateTime d2)
        {
            if 
            (
                (d1 == null && d2 != null) || 
                (d1 != null && d2 == null)
            )
            {
                return false;
            }

            if (d1.Year == d2.Year &&
                d1.Month == d2.Month &&
                d1.Day == d2.Day &&
                d1.Hour == d2.Hour &&
                d1.Minute == d2.Minute &&
                d1.Second == d2.Second)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
