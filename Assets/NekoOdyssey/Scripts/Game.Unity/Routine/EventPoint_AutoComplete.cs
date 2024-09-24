using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(EventPointInteractive))]
public class EventPoint_AutoComplete : MonoBehaviour
{
    public UnityAction OnComplete { get; set; }

    private void Awake()
    {
        GetComponent<EventPointInteractive>().OnInteractive += () =>
        {
            Debug.Log($"EventPoint_AutoComplete");

            OnComplete?.Invoke();
        };

    }
}
