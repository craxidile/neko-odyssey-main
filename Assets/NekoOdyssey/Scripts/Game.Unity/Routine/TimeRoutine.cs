using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using NekoOdyssey.Scripts.Constants;

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
        TimeScriptable timeScriptable;
        //public Day currentDay;
        //[Range(0, AppConstants.Time.MaxDayMinute)] public int dayMinute;
        float dayMinuteFloat;

        //[ReadOnlyField]
        //[SerializeField]
        //public string currentTimeText;

        //public float timeMultiplier = 1f;
        //public static TimeScriptable timeScriptable;

        //public static Day day { get; set; } = 0;
        //public TimeHrMin currentTime { get; set; } = new TimeHrMin("00.00");

        //[SerializeField] bool _isTimer; //only edit in unity editor
        //static bool s_isTimeRunning;
        public Day currentDay => timeScriptable.currentDay;
        public TimeHrMin currentTime;
        public void PauseTime() => timeScriptable._isTimer = false;
        public void ContinueTime() => timeScriptable._isTimer = true;


        public int GameDayTotal { get; set; } = 1;
        public int GameMounth => ((GameDayTotal - 1) / 30) + 1;
        public int DayInMounth => ((GameDayTotal - 1) % 30) + 1;


        //[Tooltip("increase this value mean hungey decrease faster")]
        ////[Range(0, 10)]
        //public float hungryOverTimeMultiplier = 1;
        public float hungryOverTimeMultiplier => timeScriptable.hungryOverTimeMultiplier;


        public static Subject<int> OnTimeUpdate { get; } = new();
        public static Subject<int> OnChangeDay { get; } = new();


        private void Awake()
        {
            timeScriptable = GameRunner.Instance.CsvHolder.timeProfile;
            timeScriptable.OnValidated.Subscribe(OnTimeProfileValidate).AddTo(this);
            currentTime = new TimeHrMin($"{timeScriptable.dayMinute / 60}:{timeScriptable.dayMinute % 60}");

            //dayMinuteFloat = new TimeHrMin(AppConstants.Time.StartDayTime).ToInt();
        }

        // Start is called before the first frame update
        public void Start()
        {
            


            //s_isTimeRunning = true;
            
            //dayMinute = Mathf.RoundToInt(s_dayMinuteFloat);
        }

        // Update is called once per frame
        public void Update()
        {
            //if (timeScriptable == null) return;
            //day = currentDay;
            //timeHrMin.Hour = currentHours;
            //timeHrMin.Minute = currentMinute;

            //_isTimer = s_isTimeRunning;
            //currentDay = day;
            ProcessTime();



            //SetTime($"{dayMinute / 60}:{dayMinute % 60}");


            //Debug.Log($"current time : {currentTimeText}");

            GameRunner.Instance.Core.Routine.UpdateWorld();
            GameRunner.Instance.Core.Routine.dayNightLightingManager.Update();
            DayNightTimeActivator.UpdateActivator();

        }

        public bool inBetweenDayAndTime(List<Day> checkDay, string checkTime)
        {
            if (checkDay.Contains(currentDay))
            {
                return currentTime.inBetweenTime(checkTime);
            }
            return false;
        }

        public void SetTime(string timeText)
        {
            var newTime = new TimeHrMin(timeText);
            dayMinuteFloat = newTime.ToInt();
            timeScriptable.dayMinute = newTime.ToInt();

            //currentTime = newTime;
        }

        public void NextDay()
        {
            GameDayTotal += 1;
            OnChangeDay.OnNext(GameDayTotal);
        }


        public void ProcessTime()
        {
            if (timeScriptable._isTimer)
            {
                var secondPerSecond = AppConstants.Time.GameHourPerMinute * timeScriptable.timeMultiplier;
                var nextSecondValue = Time.deltaTime * secondPerSecond;


                dayMinuteFloat += nextSecondValue;
                if (dayMinuteFloat > AppConstants.Time.MaxDayMinute)
                {
                    dayMinuteFloat = 0;
                }
                

            }
            else
            {

            }


            var previosMinute = timeScriptable.dayMinute;
            timeScriptable.dayMinute = Mathf.RoundToInt(dayMinuteFloat);
            currentTime = new TimeHrMin($"{timeScriptable.dayMinute / 60}:{timeScriptable.dayMinute % 60}");

            if (previosMinute != timeScriptable.dayMinute)
            {
                OnTimeUpdate.OnNext(Mathf.Max(timeScriptable.dayMinute - previosMinute, 0));
                Debug.Log($"OnTimeUpdate = {(timeScriptable.dayMinute - previosMinute)}");
            }
        }



        private void OnTimeProfileValidate(Unit _)
        {
            if (dayMinuteFloat !=timeScriptable.dayMinute)
            {
                OnTimeUpdate.OnNext(Mathf.RoundToInt((float)timeScriptable.dayMinute - dayMinuteFloat));
            }

            dayMinuteFloat = timeScriptable.dayMinute;
            //day = currentDay;

            //s_hungryOverTimeMultiplier = hungryOverTimeMultiplier;
        }

        //private void OnDrawGizmos()
        //{
        //    day = currentDay;
        //    SetTime($"{dayMinute / 60}:{dayMinute % 60}");
        //    currentTimeText = currentTime.ToString();
        //}

    }
}