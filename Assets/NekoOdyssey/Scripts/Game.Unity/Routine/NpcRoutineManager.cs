using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace NekoOdyssey.Scripts.Game.Core.Routine
{
    public class NpcRoutineManager
    {
        //public TextAsset testCSV;
        //public string dayText, timeText;
        //public string eventPointKey;
        //GameObject testEventPoint;

        //EventDetail _eventDetail;



        //private void Awake()
        //{
        //    //var key = testEventPoint.name;
        //}



        //void Start()
        //{

        //    //var convertedDayText = dayText.Replace(' ', '-').Replace(':', '-').Replace(';', '-');
        //    //var dayAvaliables = convertedDayText.Split('-');
        //    //HashSet<Day> dayList = new HashSet<Day>();
        //    //foreach (var day in dayAvaliables)
        //    //{
        //    //    if (System.Enum.TryParse(day, true, out Day dayResult))
        //    //    {
        //    //        dayList.Add(dayResult);
        //    //        Debug.Log($"check : {day}");
        //    //    }
        //    //}



        //    //var convertedTimeText = timeText.Replace(' ', '-').Replace('_', '-');
        //    //var targetTimes = convertedTimeText.Split('-');
        //    //TimeHrMin[] eventTimeList = new TimeHrMin[] { new TimeHrMin(targetTimes[0]), new TimeHrMin(targetTimes[1]) };

        //    //EventTime[] testET = new EventTime[]{ new EventTime("1066") };


        //    //Debug.Log($"eventTime : {eventTimeList[0].Hour}:{eventTimeList[0].Minute} - {eventTimeList[1].Hour}:{eventTimeList[1].Minute}");


        //    //_eventDetail = new EventDetail(dayList, eventTimeList[0], eventTimeList[1]);


        //    //testEventPoint = EventPoint.GetEventPoint(eventPointKey)?.gameObject;





        //}

        public void Start()
        {
            if (GameRunner.Instance.CsvHolder == null) return;
            foreach (var csv in GameRunner.Instance.CsvHolder.routinesCSV)
            {
                LoadRoutineCSV(csv);
            }
        }

        void LoadRoutineCSV(TextAsset textAsset)
        {
            //csv loader
            var newNpc = new NpcData(textAsset.name.ToLower());

            string[] lines = textAsset.text.Split('\n');

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                //Debug.Log($"load csv line : {line}");

                string[] values = line.Trim().Split(',');

                List<string> row = new List<string>(values);

                if (string.IsNullOrEmpty(row.FirstOrDefault())) continue;

                var routineDialogueKey = row[0];
                var questKeyConditionsText = row[1];
                var dayText = row[2];
                var timeText = row[3];
                var eventPointKey = row[4];


                var convertedDayText = dayText.Replace(' ', '-').Replace(':', '-').Replace(';', '-');
                var dayAvaliables = convertedDayText.Split('-');
                HashSet<Day> dayList = new HashSet<Day>();

                foreach (var day in dayAvaliables)
                {
                    if (System.Enum.TryParse(day, true, out Day dayResult))
                    {
                        dayList.Add(dayResult);
                    }
                }

                var convertedTimeText = timeText.Replace(' ', '-').Replace('_', '-');
                var targetTimes = convertedTimeText.Split('-');
                TimeHrMin[] eventTimeList = new TimeHrMin[] { new TimeHrMin(targetTimes[0]), new TimeHrMin(targetTimes[1]) };


                //var tragetEventPoint = EventPoint.GetEventPoint(eventPointKey);


                var questKeyConditions = questKeyConditionsText.Replace(' ', '-').Replace('+', '-').Split('-').ToList();
                var keyIdConditions = new List<string>();
                var keyIdConditionsExclude = new List<string>();
                foreach (var questCondition in questKeyConditions)
                {
                    if (!string.IsNullOrEmpty(questCondition))
                    {
                        if (questCondition.StartsWith("!"))
                        {
                            var condition = questCondition.Substring(1);
                            keyIdConditionsExclude.Add(condition.ToLower());

                        }
                        else
                        {
                            keyIdConditions.Add(questCondition.ToLower());

                        }
                    }
                }

                var newEventDetail = new RoutineEventDetail(dayList, eventTimeList[0], eventTimeList[1], eventPointKey, keyIdConditions, keyIdConditionsExclude);

                if (string.IsNullOrEmpty(routineDialogueKey))
                {
                    routineDialogueKey = "-";
                }
                newEventDetail.dialogueKey = routineDialogueKey.ToLower();

                //WorldRoutineManager.allNpcEvents.Add(newEventDetail);
                newNpc.npcRoutineEvents.Add(newEventDetail);
            }

            GameRunner.Instance.Core.Routine.npcDatas.Add(newNpc);
            //Debug.Log($"Add npc {newNpc.npcName}");
        }


        //void Update()
        //{
        //    //foreach (var eventDetail in WorldRoutineManager.allNpcEvents)
        //    //{
        //    //    if (eventDetail.IsInEventTime(TimeRoutine.day, TimeRoutine.timeHrMin))
        //    //    {
        //    //        eventDetail.targetEventPoint.gameObject.SetActive(true);
        //    //    }
        //    //    else
        //    //    {
        //    //        eventDetail.targetEventPoint.gameObject.SetActive(false);
        //    //    }
        //    //}

        //    //if (_eventDetail != null && testEventPoint != null)
        //    //{
        //    //    if (_eventDetail.IsInEventTime(TimeRoutine.day, TimeRoutine.timeHrMin))
        //    //    {
        //    //        testEventPoint.SetActive(true);
        //    //    }
        //    //    else
        //    //    {
        //    //        testEventPoint.SetActive(false);
        //    //    }
        //    //}


        //}

    }


    public class EventDetail
    {
        public HashSet<Day> eventDays;
        public TimeHrMin eventTimeStart, eventTimeEnd;
        public string targetEventPointKey;
        EventPoint targetEventPoint;

        public List<string> keyIdConditions;
        public List<string> keyIdConditionsExclude;

        public EventDetail(HashSet<Day> eventDays, TimeHrMin eventTimeStart, TimeHrMin eventTimeEnd, string targetEventPointKey, List<string> keyIdConditions, List<string> keyIdConditionsExclude)
        {
            this.eventDays = eventDays;
            this.eventTimeStart = eventTimeStart;
            this.eventTimeEnd = eventTimeEnd;
            this.targetEventPointKey = targetEventPointKey;

            this.keyIdConditions = keyIdConditions;
            this.keyIdConditionsExclude = keyIdConditionsExclude;
        }

        public bool IsInEventTime(Day currentDay, TimeHrMin currentTime)
        {
            if (eventDays.Contains(currentDay))
            {
                //return eventTimeStart <= currentTime && currentTime <= eventTimeEnd;
                return currentTime.inBetweenTime(eventTimeStart, eventTimeEnd);
            }
            return false;
        }

        public EventPoint GetTargetEventPoint()
        {
            if (targetEventPoint == null)
            {
                targetEventPoint = EventPoint.GetEventPoint(targetEventPointKey);
            }

            return targetEventPoint;
        }
    }


    public class RoutineEventDetail : EventDetail
    {
        public string dialogueKey;

        public RoutineEventDetail(HashSet<Day> eventDays, TimeHrMin eventTimeStart, TimeHrMin eventTimeEnd, string targetEventPointKey, List<string> keyIdConditions, List<string> keyIdConditionsExclude)
            : base(eventDays, eventTimeStart, eventTimeEnd, targetEventPointKey, keyIdConditions, keyIdConditionsExclude)
        {
        }
    }
}