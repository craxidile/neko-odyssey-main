using System;
using DayOfWeek = NekoOdyssey.Scripts.Database.Commons.Models.DayOfWeek;

public static class DayOfWeekIntegration
{
    public static Day ToRoutineDay(this DayOfWeek value)
    {
        return value switch
        {
            DayOfWeek.Sunday => Day.Sun,
            DayOfWeek.Monday => Day.Mon,
            DayOfWeek.Tuesday => Day.Tue,
            DayOfWeek.Wednesday => Day.Wed,
            DayOfWeek.Thursday => Day.Thu,
            DayOfWeek.Friday => Day.Fri,
            DayOfWeek.Saturday => Day.Sat,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
    public static DayOfWeek ToDayOfWeek(this Day value)
    {
        return value switch
        {
            Day.Sun => DayOfWeek.Sunday,
            Day.Mon => DayOfWeek.Monday,
            Day.Tue => DayOfWeek.Tuesday,
            Day.Wed => DayOfWeek.Wednesday,
            Day.Thu => DayOfWeek.Thursday,
            Day.Fri => DayOfWeek.Friday,
            Day.Sat => DayOfWeek.Saturday,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}