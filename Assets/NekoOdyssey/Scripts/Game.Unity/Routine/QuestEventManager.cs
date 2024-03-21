using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestEventManager : MonoBehaviour
{
    public TextAsset testCSV;

    [Header("Test Quest Key")]
    public List<string> ownedQuestKey = new List<string>();



    void Start()
    {

    }



    void Update()
    {

    }


    public void InitializedQuestEvent()
    {
        #region CSV Loader        
        //csv loader

        string[] lines = testCSV.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i];
            Debug.Log($"load csv line : {line}");

            string[] values = line.Trim().Split(',');

            List<string> row = new List<string>(values);

            if (string.IsNullOrEmpty(row.FirstOrDefault())) continue;


            //process
            var questKey = row[0]; //column 0
            var questKeyConditionsText = row[1]; //column 1
            var dayText = row[2];
            var timeText = row[3];
            var eventPointKey = row[4];
            var releatedCharactersText = row[5];


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



            var convertedReleatedCharactersText = releatedCharactersText.Replace(' ', '-').Replace(':', '-').Replace(';', '-');
            var releatedCharacters = convertedReleatedCharactersText.ToLower().Split('-').ToList();

            QuestEventDetail newQuestEventDetail = new QuestEventDetail(dayList, eventTimeList[0], eventTimeList[1], tragetEventPoint, releatedCharacters);
            newQuestEventDetail.questId = questKey.ToLower();
            var questKeyConditions = questKeyConditionsText.Replace(' ', '-').Replace('+', '-').Split('-').ToList();
            newQuestEventDetail.questIdConditions = new List<string>();
            newQuestEventDetail.questIdConditionsExclude = new List<string>();
            foreach (var questCondition in questKeyConditions)
            {
                if (!string.IsNullOrEmpty(questCondition))
                {
                    if (questCondition.StartsWith("!"))
                    {
                        var condition = questCondition.Substring(1);
                        newQuestEventDetail.questIdConditionsExclude.Add(condition.ToLower());

                    }
                    else
                    {
                        newQuestEventDetail.questIdConditions.Add(questCondition.ToLower());

                    }
                }
            }

            WorldRoutineManager.allQuestEvents.Add(newQuestEventDetail);
        }

        #endregion


        #region QuestData
        //foreach (var questData in testQuestData)
        //{
        //    //process
        //    var questKey = questData.questId;
        //    var questKeyConditions = questData.questIdRequired;
        //    var dayText = questData.dayText;
        //    var timeText = questData.timeText;
        //    var eventPointKey = questData.targetEventPoint;
        //    var releatedCharactersText = questData.relatedCharacters;


        //    var convertedDayText = dayText.Replace(' ', '-').Replace(':', '-').Replace(';', '-');
        //    var dayAvaliables = convertedDayText.Split('-');
        //    HashSet<Day> dayList = new HashSet<Day>();

        //    foreach (var day in dayAvaliables)
        //    {
        //        if (System.Enum.TryParse(day, true, out Day dayResult))
        //        {
        //            dayList.Add(dayResult);
        //        }
        //    }

        //    var convertedTimeText = timeText.Replace(' ', '-').Replace('_', '-');
        //    var targetTimes = convertedTimeText.Split('-');
        //    TimeHrMin[] eventTimeList = new TimeHrMin[] { new TimeHrMin(targetTimes[0]), new TimeHrMin(targetTimes[1]) };


        //    var tragetEventPoint = EventPoint.GetEventPoint(eventPointKey);



        //    var convertedReleatedCharactersText = releatedCharactersText.Replace(' ', '-').Replace(':', '-').Replace(';', '-');
        //    var releatedCharacters = convertedReleatedCharactersText.ToLower().Split('-').ToList();

        //    QuestEventDetail newQuestEventDetail = new QuestEventDetail(dayList, eventTimeList[0], eventTimeList[1], tragetEventPoint, releatedCharacters);
        //    newQuestEventDetail.questKey = questKey;
        //    newQuestEventDetail.questKeyConditions = questKeyConditions.Replace(' ', '-').Replace('_', '-').Split('-').ToList();

        //    WorldRoutineManager.allQuestEvents.Add(newQuestEventDetail);

        //}
        #endregion
    }


    public bool CheckQuestCondition(QuestEventDetail questDetail)
    {
        if (questDetail.questIdConditions.Count > 0)
        {
            if (questDetail.questIdConditions.Any(condition => !ownedQuestKey.Contains(condition))) //check player quest owned quest id
            {
                return false;
            }

            foreach (var key in questDetail.questIdConditions)//check for player inventory item
            {
                //if (!playerInventory.contains(key))
                //{
                //return false;
                //}
            }

        }




        if (questDetail.questIdConditionsExclude.Count > 0 && questDetail.questIdConditionsExclude.Any(condition => ownedQuestKey.Contains(condition)))
        {
            return false;
        }


        return true;
    }
}


public class QuestEventDetailGroup
{
    public List<QuestEventDetail> questEventDetails = new List<QuestEventDetail>();
}
public class QuestEventDetail : EventDetail
{
    //public HashSet<Day> eventDays;
    //public TimeHrMin eventTimeStart, eventTimeEnd;
    //public EventPoint targetEventPoint;

    public string questId;
    public List<string> questIdConditions;
    public List<string> questIdConditionsExclude;

    public List<string> relatedCharacters;

    public QuestEventDetail(HashSet<Day> eventDays, TimeHrMin eventTimeStart, TimeHrMin eventTimeEnd, EventPoint targetEventPoint, List<string> relatedCharacters) : base(eventDays, eventTimeStart, eventTimeEnd, targetEventPoint)
    {
        this.eventDays = eventDays;
        this.eventTimeStart = eventTimeStart;
        this.eventTimeEnd = eventTimeEnd;
        this.targetEventPoint = targetEventPoint;

        this.relatedCharacters = relatedCharacters;
    }

    //public bool IsInEventTime(Day currentDay, TimeHrMin currentTime)
    //{
    //    if (eventDays.Contains(currentDay))
    //    {
    //        return eventTimeStart <= currentTime && currentTime <= eventTimeEnd;
    //    }
    //    return false;
    //}
}
