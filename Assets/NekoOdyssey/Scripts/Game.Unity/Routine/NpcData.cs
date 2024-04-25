using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Core.Routine
{
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
                //Debug.Log("UpdateRoutine 0");
                if (!GameRunner.Instance.Core.Routine.questEventManager.CheckQuestKeyAndItem(eventDetail.keyIdConditions, eventDetail.keyIdConditionsExclude)) continue;
                //Debug.Log("UpdateRoutine 1");


                if (eventDetail.IsInEventTime(TimeRoutine.day, TimeRoutine.currentTime))
                {
                    if (SwtichEvent(eventDetail))
                    {
                        return eventDetail;
                    }
                }
                //else
                //{
                //    eventDetail.targetEventPoint.gameObject.SetActive(false);
                //}
            }

            SwtichEvent(null);
            return null;
        }

        public bool SwtichEvent(RoutineEventDetail newEvent)
        {
            if (newEvent != _cureentEvent)
            {
                if (_cureentEvent == null || _cureentEvent.targetEventPoint == null) return false;
                if (newEvent == null || newEvent.targetEventPoint == null) return false;


                _cureentEvent?.targetEventPoint.gameObject.SetActive(false);
                _cureentEvent = newEvent;

                newEvent?.targetEventPoint.gameObject.SetActive(true);

                return true;
            }

            return false;

        }
    }
}