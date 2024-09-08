using NekoOdyssey.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventPoint_Cutscene : MonoBehaviour
{
    public string siteName = "";

    private void Awake()
    {
        GetComponent<EventPointInteractive>().OnInteractive += () =>
        {
            Debug.Log($"EventPoint_Cutscene : {siteName}");


        };

    }

}
