using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventPoint : MonoBehaviour
{
    //public enum EventPointType { Npc, Area, Other }
    //public EventPointType eventType { get; set; }

    static Dictionary<string, EventPoint> AllEventPoint = new Dictionary<string, EventPoint>();


    private void Awake()
    {
        AllEventPoint.Add(name, this);
        gameObject.SetActive(false);
    }


    public static EventPoint GetEventPoint(string key)
    {
        if (AllEventPoint.ContainsKey(key))
        {
            return AllEventPoint[key];
        }
        else
        {
            Debug.LogWarning($"EventPoint : {key} Not Found");
            return null;
        }
    }
}
