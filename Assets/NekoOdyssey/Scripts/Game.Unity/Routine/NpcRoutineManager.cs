using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NpcRoutineManager : MonoBehaviour
{
    public TextAsset testCSV;
    //public string dayText, timeText;
    //public string eventPointKey;
    //GameObject testEventPoint;

    //EventDetail _eventDetail;
    List<EventDetail> allEvents = new List<EventDetail>();


    private void Awake()
    {
        //var key = testEventPoint.name;
    }



    void Start()
    {

        //var convertedDayText = dayText.Replace(' ', '-').Replace(':', '-').Replace(';', '-');
        //var dayAvaliables = convertedDayText.Split('-');
        //HashSet<Day> dayList = new HashSet<Day>();
        //foreach (var day in dayAvaliables)
        //{
        //    if (System.Enum.TryParse(day, true, out Day dayResult))
        //    {
        //        dayList.Add(dayResult);
        //        Debug.Log($"check : {day}");
        //    }
        //}



        //var convertedTimeText = timeText.Replace(' ', '-').Replace('_', '-');
        //var targetTimes = convertedTimeText.Split('-');
        //TimeHrMin[] eventTimeList = new TimeHrMin[] { new TimeHrMin(targetTimes[0]), new TimeHrMin(targetTimes[1]) };

        //EventTime[] testET = new EventTime[]{ new EventTime("1066") };


        //Debug.Log($"eventTime : {eventTimeList[0].Hour}:{eventTimeList[0].Minute} - {eventTimeList[1].Hour}:{eventTimeList[1].Minute}");


        //_eventDetail = new EventDetail(dayList, eventTimeList[0], eventTimeList[1]);


        //testEventPoint = EventPoint.GetEventPoint(eventPointKey)?.gameObject;




        //csv loader

        string[] lines = testCSV.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i];
            Debug.Log($"load csv line : {line}");

            string[] values = line.Trim().Split(',');

            List<string> row = new List<string>(values);

            if (string.IsNullOrEmpty(row.FirstOrDefault())) continue;

            var dayText = row[1]; //column 1
            var timeText = row[2]; //column 2
            var eventPointKey = row[3];


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


            var tragetEventPoint = EventPoint.GetEventPoint(eventPointKey);


            EventDetail newEventDetail = new EventDetail(dayList, eventTimeList[0], eventTimeList[1], tragetEventPoint);

            allEvents.Add(newEventDetail);
        }


    }


    void Update()
    {
        foreach (var eventDetail in allEvents)
        {
            if (eventDetail.IsInEventTime(TimeRoutine.day, TimeRoutine.timeHrMin))
            {
                eventDetail.targetEventPoint.gameObject.SetActive(true);
            }
            else
            {
                eventDetail.targetEventPoint.gameObject.SetActive(false);
            }
        }
        //if (_eventDetail != null && testEventPoint != null)
        //{
        //    if (_eventDetail.IsInEventTime(TimeRoutine.day, TimeRoutine.timeHrMin))
        //    {
        //        testEventPoint.SetActive(true);
        //    }
        //    else
        //    {
        //        testEventPoint.SetActive(false);
        //    }
        //}


    }
}


public class EventDetail
{
    public HashSet<Day> eventDays;
    public TimeHrMin eventTimeStart, eventTimeEnd;
    public EventPoint targetEventPoint;

    public EventDetail(HashSet<Day> eventDays, TimeHrMin eventTimeStart, TimeHrMin eventTimeEnd, EventPoint targetEventPoint)
    {
        this.eventDays = eventDays;
        this.eventTimeStart = eventTimeStart;
        this.eventTimeEnd = eventTimeEnd;
        this.targetEventPoint = targetEventPoint;
    }

    public bool IsInEventTime(Day currentDay, TimeHrMin currentTime)
    {
        if (eventDays.Contains(currentDay))
        {
            return eventTimeStart <= currentTime && currentTime <= eventTimeEnd;
        }
        return false;
    }
}