using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcData
{
    public string npcName;

    public bool routineEnable;
    public List<EventDetail> npcRoutineEvents = new List<EventDetail>();

    EventDetail _cureentEvent;
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
    public void UpdateRoutine()
    {
        for (int i = npcRoutineEvents.Count - 1; i >= 0; i--)
        {
            var eventDetail = npcRoutineEvents[i];

            if (eventDetail.IsInEventTime(TimeRoutine.day, TimeRoutine.timeHrMin))
            {
                SwtichEvent(eventDetail);
                return;
            }
            //else
            //{
            //    eventDetail.targetEventPoint.gameObject.SetActive(false);
            //}
        }

        SwtichEvent(null);
    }

    public void SwtichEvent(EventDetail newEvent)
    {
        if (newEvent != _cureentEvent)
        {
            _cureentEvent?.targetEventPoint.gameObject.SetActive(false);
            _cureentEvent = newEvent;

            newEvent?.targetEventPoint.gameObject.SetActive(true);

        }

    }
}
