using System.Collections;
using System.Collections.Generic;
using UniRx;
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
    public int ToInt()
    {
        return (Hour * 60) + Minute;
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

namespace NekoOdyssey.Scripts.Game.Core.Routine
{
    public class TimeRoutine : MonoBehaviour
    {
        public const float GameHourPerMinute = 0.25f;
        public const int StartDayMinute = 500;
        public const int MaxDayMinute = 1440;


        public Day currentDay;
        [Range(0, MaxDayMinute)] public int dayMinute;
        static float _dayMinuteFloat;

        [ReadOnlyField]
        [SerializeField]
        public string currentTimeText;

        public float timeMultiplier = 1f;
        //public static TimeScriptable timeScriptable;

        public static Day day { get; set; } = 0;
        public static TimeHrMin currentTime { get; set; } = new TimeHrMin("00.00");

        [SerializeField] bool _isTimer; //only edit in unity editor
        static bool isTimeRunning = false;
        public static void PauseTime() => isTimeRunning = false;
        public static void ContinueTime() => isTimeRunning = true;

        public static Subject<int> OnTimeUpdate { get; } = new();

        // Start is called before the first frame update
        public void Start()
        {
            isTimeRunning = false;
            _dayMinuteFloat = StartDayMinute;
            dayMinute = Mathf.RoundToInt(_dayMinuteFloat);
        }

        // Update is called once per frame
        public void Update()
        {
            //if (timeScriptable == null) return;
            //day = currentDay;
            //timeHrMin.Hour = currentHours;
            //timeHrMin.Minute = currentMinute;

            _isTimer = isTimeRunning;
            currentDay = day;
            ProcessTime();



            //SetTime($"{dayMinute / 60}:{dayMinute % 60}");


            //Debug.Log($"current time : {currentTimeText}");

            GameRunner.Instance.Core.Routine.UpdateWorld();
            GameRunner.Instance.Core.Routine.dayNightLightingManager.Update();
            DayNightTimeActivator.UpdateActivator();

        }

        public static bool inBetweenDayAndTime(List<Day> checkDay, string checkTime)
        {
            if (checkDay.Contains(day))
            {
                return currentTime.inBetweenTime(checkTime);
            }
            return false;
        }

        public static void SetTime(string timeText)
        {
            var newTime = new TimeHrMin(timeText);
            _dayMinuteFloat = newTime.ToInt();

            currentTime = newTime;
        }


        public void ProcessTime()
        {
            if (isTimeRunning)
            {
                var secondPerSecond = GameHourPerMinute * timeMultiplier;
                var nextSecondValue = Time.deltaTime * secondPerSecond;


                _dayMinuteFloat += nextSecondValue;
                if (_dayMinuteFloat > MaxDayMinute)
                {
                    _dayMinuteFloat = 0;
                }


            }
            else
            {

            }


            var previosMinute = dayMinute;
            dayMinute = Mathf.RoundToInt(_dayMinuteFloat);
            currentTime = new TimeHrMin($"{dayMinute / 60}:{dayMinute % 60}");
            currentTimeText = currentTime.ToString();

            if (previosMinute != dayMinute)
            {
                OnTimeUpdate.OnNext(Mathf.Max(dayMinute - previosMinute, 0));
            }
        }



        private void OnValidate()
        {
            _dayMinuteFloat = dayMinute;
            isTimeRunning = _isTimer;
            day = currentDay;
        }

        //private void OnDrawGizmos()
        //{
        //    day = currentDay;
        //    SetTime($"{dayMinute / 60}:{dayMinute % 60}");
        //    currentTimeText = currentTime.ToString();
        //}

    }
}