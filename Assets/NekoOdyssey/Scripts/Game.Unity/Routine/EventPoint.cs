using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventPoint : MonoBehaviour
{
    public enum EventPointType { Quest, Routine }
    public EventPointType eventPointType { get; set; }
    public string ReferenceEventCode { get; set; } = "";

    static Dictionary<string, EventPoint> AllEventPoint = new Dictionary<string, EventPoint>();


    private void Awake()
    {
        AllEventPoint.Add(name, this);
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        AllEventPoint.Remove(name);
    }


    public static EventPoint GetEventPoint(string key)
    {

        //Debug.Log($"event Point Count {AllEventPoint.Count}");

        //foreach (var item in AllEventPoint)
        //{
        //    Debug.Log($"{item.Value.name}");

        //}


        if (AllEventPoint.ContainsKey(key))
        {
            return AllEventPoint[key];
        }
        else
        {
            //Debug.LogWarning($"EventPoint : {key} Not Found");
            return null;
        }

    }

    public static void HideAllEventPoint()
    {
        foreach (var eventPoint in AllEventPoint.Values)
        {
            eventPoint.gameObject.SetActive(false);
        }
    }

    public static List<EventPoint> GetEventPointsOfActors(List<string> actors)
    {
        List<EventPoint> result = new();
        foreach (var eventPoint in AllEventPoint.Values)
        {
            var eventPointActors = eventPoint.GetComponentsInChildren<DialogueActor>();

            if (eventPointActors.Any(actor => actors.Contains(actor.actorId)))
            {
                result.Add(eventPoint);
            }
        }
        return result;
    }
}
