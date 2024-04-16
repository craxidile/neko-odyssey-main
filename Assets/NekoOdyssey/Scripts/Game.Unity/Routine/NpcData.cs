using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcData
{
    public string npcName;

    public bool routineEnable;
    public List<RoutineEventDetail> npcRoutineEvents = new List<RoutineEventDetail>();

    RoutineEventDetail _cureentEvent;
    public QuestDialogueGroup dialogueGroup { get; set; }

    public NpcData(string npcName = "npc")
    {
        this.npcName = npcName;
    }


    //public EventDetail GetEvent()
    //{
    //    foreach (var eventDetail in npcRoutineEvents)
    //    {
    //        if (eventDetail.IsInEventTime(TimeRoutine.day, TimeRoutine.timeHrMin))
    //        {
    //            return eventDetail;
    //        }
    //    }

    //    return null;
    //}

    public void HideAllRoutine()
    {
        SwtichEvent(null);
        foreach (var eventDetail in npcRoutineEvents)
        {
            var targetEventObject = eventDetail.targetEventPoint.gameObject;
            if (targetEventObject.activeSelf)
            {
                targetEventObject.SetActive(false);
            }
        }

    }
    public RoutineEventDetail UpdateRoutine()
    {
        for (int i = npcRoutineEvents.Count - 1; i >= 0; i--)
        {
            var eventDetail = npcRoutineEvents[i];
            Debug.Log("UpdateRoutine 0");
            if (!WorldRoutineManager.Instance.questEventManager.CheckQuestKeyAndItem(eventDetail.keyIdConditions, eventDetail.keyIdConditionsExclude)) continue;
            Debug.Log("UpdateRoutine 1");


            if (eventDetail.IsInEventTime(TimeRoutine.day, TimeRoutine.currentTime))
            {
                SwtichEvent(eventDetail);
                return eventDetail;
            }
            //else
            //{
            //    eventDetail.targetEventPoint.gameObject.SetActive(false);
            //}
        }

        SwtichEvent(null);
        return null;
    }

    public void SwtichEvent(RoutineEventDetail newEvent)
    {
        if (newEvent != _cureentEvent)
        {
            _cureentEvent?.targetEventPoint.gameObject.SetActive(false);
            _cureentEvent = newEvent;

            newEvent?.targetEventPoint.gameObject.SetActive(true);

        }

    }
}
