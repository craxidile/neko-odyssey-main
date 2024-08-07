using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using NekoOdyssey.Scripts.Constants;
using NekoOdyssey.Scripts.Game.Core.GameScene;
using DayOfWeek = NekoOdyssey.Scripts.Database.Commons.Models.DayOfWeek;

public enum Day
{
    Mon, Tue, Wed, Thu, Fri, Sat, Sun
}
public class TimeHrMin
{
    public int Hour, Minute;

    //public TimeHrMin(int timeText)
    //{
    //    new TimeHrMin(timeText.ToString());
    //}
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
    public TimeHrMin(int hour, int minute)
    {
        Hour = hour;
        Minute = minute;
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
        bool _isOverTime; //time have pass 1 hour

        //[ReadOnlyField]
        //[SerializeField]
        //public string currentTimeText;

        //public float timeMultiplier = 1f;
        //public static TimeScriptable timeScriptable;

        //public static Day day { get; set; } = 0;
        //public TimeHrMin currentTime { get; set; } = new TimeHrMin("00.00");

        //[SerializeField] bool _isTimer; //only edit in unity editor
        //static bool s_isTimeRunning;
        //public Day CurrentDay => timeScriptable.currentDay;
        public TimeHrMin currentTime;
        //public TimeHrMin currentTime => new TimeHrMin(GameRunner.Instance.Core.Player.Time.Item1, GameRunner.Instance.Core.Player.Time.Item2);
        public void PauseTime() => timeScriptable._isTimer = false;
        public void ContinueTime() => timeScriptable._isTimer = true;


        public int GameDayTotal { get; set; } = 1;
        public int GameMounth => ((GameDayTotal - 1) / 30) + 1;
        public int DayInMounth => ((GameDayTotal - 1) % 30) + 1;
        public Day CurrentDay
        {
            get
            {
                //int daysInWeek = System.Enum.GetNames(typeof(Day)).Length;
                int daysInWeek = 7;
                int dayOfWeekIndex = (GameDayTotal - 1) % daysInWeek;
                return (Day)dayOfWeekIndex;
            }
        }


        //[Tooltip("increase this value mean hungey decrease faster")]
        ////[Range(0, 10)]
        //public float hungryOverTimeMultiplier = 1;
        public float hungryOverTimeMultiplier => timeScriptable.hungryOverTimeMultiplier;


        public Subject<int> OnTimeUpdate { get; } = new();
        public Subject<int> OnChangeDay { get; } = new();

        float _timeSaveDelay;

        private void Awake()
        {
            currentTime = new TimeHrMin(GameRunner.Instance.Core.Player.Time.Item1, GameRunner.Instance.Core.Player.Time.Item2);
            GameDayTotal = Mathf.Max(GameRunner.Instance.Core.Player.DayCount, 1);

            var startDayTime = new TimeHrMin(AppConstants.Time.StartDayTime);
            if (currentTime < startDayTime)
            {
                currentTime = startDayTime;
                SaveTimeData(true);
            }

            if (GameRunner.Instance.CsvHolder == null) return;

            timeScriptable = GameRunner.Instance.CsvHolder.timeProfile;
            timeScriptable.OnValidated.Subscribe(OnTimeProfileValidate).AddTo(this);

            timeScriptable.dayMinute = currentTime.ToInt();
            timeScriptable.currentDayCount = GameDayTotal;
            //currentTime = new TimeHrMin($"{timeScriptable.dayMinute / 60}:{timeScriptable.dayMinute % 60}");

            //dayMinuteFloat = new TimeHrMin(AppConstants.Time.StartDayTime).ToInt();
            dayMinuteFloat = timeScriptable.dayMinute;

            Debug.Log($"initialize time : {timeScriptable.dayMinute}");

            _timeSaveDelay = Time.time;

            GameRunner.Instance.Core.GameScene.OnChangeSceneMode
                .Subscribe(OnChangeSceneMode)
                .AddTo(this);
            GameRunner.Instance.Core.GameScene.OnChangeSceneFinish
                .Subscribe(OnChangeSceneFinish)
                .AddTo(this);
        }

        // Start is called before the first frame update
        public void Start()
        {
            GameRunner.Instance.Core.RoutineManger.UpdateWorld();


            //s_isTimeRunning = true;

            //dayMinute = Mathf.RoundToInt(s_dayMinuteFloat);
        }

        private void OnDestroy()
        {
            if (_isOverTime)
            {
                GameRunner.Instance.Core.Player.UpdateTime(currentTime.Hour + 1, 0);
            }
        }

        // Update is called once per frame
        public void Update()
        {
            if (_isOverTime) return;


            //if (timeScriptable == null) return;
            //day = currentDay;
            //timeHrMin.Hour = currentHours;
            //timeHrMin.Minute = currentMinute;

            //_isTimer = s_isTimeRunning;
            //currentDay = day;
            ProcessTime();

            if (_isOverTime) return;
            //SetTime($"{dayMinute / 60}:{dayMinute % 60}");


            //Debug.Log($"current time : {currentTimeText}");

            //GameRunner.Instance.Core.Routine.UpdateWorld();
            //GameRunner.Instance.Core.Routine.dayNightLightingManager.Update();

            //GameRunner.Instance.Core.RoutineManger.UpdateWorld();
            GameRunner.Instance.Core.RoutineManger.dayNightLightingManager.Update();

            DayNightTimeActivator.UpdateActivator();

        }

        public bool inBetweenDayAndTime(List<Day> checkDay, string checkTime)
        {
            if (checkDay.Contains(CurrentDay))
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

            currentTime = new TimeHrMin(newTime.Hour, newTime.Minute);
            SaveTimeData(true);
        }

        public void NextDay()
        {
            GameRunner.Instance.Core.Player.UpdateDayCount(GameDayTotal + 1);

            GameDayTotal += 1;

            //int dayOfWeekIndex = (GameDayTotal - 1) % 7;
            //timeScriptable.currentDay = (Day)dayOfWeekIndex;

            OnChangeDay.OnNext(GameDayTotal);
        }


        public void ProcessTime()
        {
            if (timeScriptable == null) return;
            if (GameRunner.Instance.Core.Player.Mode != Unity.Game.Core.PlayerMode.Move) return;

            if (timeScriptable._isTimer)
            {
                var secondPerSecond = AppConstants.Time.GameHourPerMinute * timeScriptable.timeMultiplier;
                var nextSecondValue = Time.deltaTime * secondPerSecond;


                dayMinuteFloat += nextSecondValue;
                if (dayMinuteFloat > AppConstants.Time.MaxDayMinute)
                {
                    dayMinuteFloat = 0;
                }


                if (Mathf.RoundToInt(dayMinuteFloat) % 60 == 59 && !_isOverTime)
                {
                    _isOverTime = true;
                }
            }
            else
            {

            }


            var previosMinute = timeScriptable.dayMinute;
            timeScriptable.dayMinute = Mathf.RoundToInt(dayMinuteFloat);

            //currentTime = new TimeHrMin($"{timeScriptable.dayMinute / 60}:{timeScriptable.dayMinute % 60}");

            if (previosMinute != timeScriptable.dayMinute)
            {
                var newTime = new TimeHrMin(timeScriptable.dayMinute / 60, timeScriptable.dayMinute % 60);
                currentTime = newTime;


                OnTimeUpdate.OnNext(Mathf.Max(timeScriptable.dayMinute - previosMinute, 0));
                //Debug.Log($"OnTimeUpdate = {(timeScriptable.dayMinute - previosMinute)}");


                SaveTimeData();
            }
        }

        public string GetUiTimeText()
        {
            var currentTimeText = currentTime.ToString();
            if (currentTimeText.StartsWith("0")) currentTimeText = currentTimeText.Substring(1);
            string timeAffixText = "AM";
            var midDayTime = new TimeHrMin("12:00");
            if (currentTime > midDayTime)
                timeAffixText = "PM";


            return $"{currentTimeText}";
            // return $"{currentTimeText} {timeAffixText}";
        }



        private void OnTimeProfileValidate(Unit _)
        {
            var previousTime = dayMinuteFloat;
            if (dayMinuteFloat != timeScriptable.dayMinute)
            {
                dayMinuteFloat = timeScriptable.dayMinute;
                OnTimeUpdate.OnNext(Mathf.RoundToInt(dayMinuteFloat - previousTime));
            }

            dayMinuteFloat = timeScriptable.dayMinute;

            GameDayTotal = timeScriptable.currentDayCount;

            //day = currentDay;

            //s_hungryOverTimeMultiplier = hungryOverTimeMultiplier;
        }

        //private void OnDrawGizmos()
        //{
        //    day = currentDay;
        //    SetTime($"{dayMinute / 60}:{dayMinute % 60}");
        //    currentTimeText = currentTime.ToString();
        //}


        public void SaveTimeData(bool forceSave = false)
        {
            if (!forceSave)
            {
                if (Time.time < _timeSaveDelay) return;
            }

            _timeSaveDelay = Time.time + 10;
            GameRunner.Instance.Core.Player.UpdateTime(currentTime.Hour, currentTime.Minute);
        }

        void OnChangeSceneMode(GameSceneMode gameSceneMode)
        {
            if (gameSceneMode.Equals(GameSceneMode.Closing))
            {
                PauseTime();
                SaveTimeData(true);
            }
        }
        void OnChangeSceneFinish(GameSceneMode gameSceneMode)
        {
            if (gameSceneMode.Equals(GameSceneMode.Opening))
            {
                ContinueTime();
            }
        }
    }
}