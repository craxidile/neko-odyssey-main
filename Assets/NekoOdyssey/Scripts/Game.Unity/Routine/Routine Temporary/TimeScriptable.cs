using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TimeScriptable", menuName = "ScriptableObjects/TimeScriptable")]
public class TimeScriptable : ScriptableObject
{
    [Header("Test")]
    public Day currentDay;
    [Range(0, 1440)] public int dayMinute;

    [ReadOnlyField]
    [SerializeField]
    public string currentTimeText;

    public float timeSecondPerGameHour = 240;
}
