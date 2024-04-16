using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Day
{
    Mon, Tue, Wed, Thu, Fri, Sat, Sun
}
public class TimeHrMin
{
    public int Hour, Minute;

    public TimeHrMin(int timeText)
    {
        new TimeHrMin(timeText.ToString());
    }
    public TimeHrMin(string timeText)
    {
        //Debug.Log($"create timeHrMin with text : {timeText}");
        timeText = timeText.Replace(':', '.').Replace(';', '.');
        if (!timeText.Contains('.'))
        {
            timeText = timeText.PadLeft(4, '0');
            timeText = timeText.Insert(2, ".");
        }

        var timeSplit = timeText.Split('.');
        timeSplit[0] = timeSplit[0].PadLeft(2, '0');
        timeSplit[1] = timeSplit[1].PadLeft(2, '0');

        Hour = int.Parse(timeSplit[0]);
        Minute = int.Parse(timeSplit[1]);
        Minute = Mathf.Clamp(Minute, 0, 59);
    }

    public static bool operator >(TimeHrMin timeA, TimeHrMin timeB)
    {
        int totalMinutesA = timeA.Hour * 60 + timeA.Minute;
        int totalMinutesB = timeB.Hour * 60 + timeB.Minute;
        return totalMinutesA > totalMinutesB;
    }

    public static bool operator <(TimeHrMin timeA, TimeHrMin timeB)
    {
        return !(timeA >= timeB);
    }

    public static bool operator >=(TimeHrMin timeA, TimeHrMin timeB)
    {
        int totalMinutesA = timeA.Hour * 60 + timeA.Minute;
        int totalMinutesB = timeB.Hour * 60 + timeB.Minute;
        return totalMinutesA >= totalMinutesB;
    }

    public static bool operator <=(TimeHrMin timeA, TimeHrMin timeB)
    {
        return !(timeA > timeB);
    }

    public override string ToString()
    {
        return $"{Hour.ToString().PadLeft(2, '0')}:{Minute.ToString().PadLeft(2, '0')}";
    }

    public bool inBetweenTime(TimeHrMin startTime, TimeHrMin endTime)
    {
        return startTime <= this && this <= endTime;
    }
    public bool inBetweenTime(string timeText)
    {
        var convertedTimeText = timeText.Replace(' ', '-').Replace('_', '-');
        var targetTimes = convertedTimeText.Split('-');
        if (targetTimes.Length < 2)
        {
            Debug.Log($"check time failed : {timeText}");
            return false;
        }
        TimeHrMin[] eventTimeList = new TimeHrMin[] { new TimeHrMin(targetTimes[0]), new TimeHrMin(targetTimes[1]) };
        var startTime = eventTimeList[0];
        var endTime = eventTimeList[1];
        return inBetweenTime(startTime, endTime);
    }
}

public class TimeRoutine : MonoBehaviour
{
    [Header("Test")]
    public TimeScriptable timeScriptable;
    //public Day currentDay;
    //[Range(0, 24)] public int currentHours;
    //[Range(0, 60)] public int currentMinute;
    //[Range(0, 1440)] public int dayMinute;

    //[ReadOnlyField]
    //[SerializeField]
    //string currentTimeText;

    public static Day day { get; set; } = 0;
    public static TimeHrMin currentTime { get; set; } = new TimeHrMin("00.00");

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //day = currentDay;
        //timeHrMin.Hour = currentHours;
        //timeHrMin.Minute = currentMinute;




        day = timeScriptable.currentDay;
        SetTime($"{timeScriptable.dayMinute / 60}:{timeScriptable.dayMinute % 60}");
        timeScriptable.currentTimeText = currentTime.ToString();
    }


    public static void SetTime(string timeText)
    {
        currentTime = new TimeHrMin(timeText);
    }
    public static void PauseTime()
    {

    }

    private void OnValidate()
    {
        //day = currentDay;
        //SetTime($"{dayMinute / 60}:{dayMinute % 60}");
        //currentTimeText = timeHrMin.ToString();
    }

    private void OnDrawGizmos()
    {
        Update();
    }
}
