using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TimeScriptable", menuName = "ScriptableObjects/TimeScriptable")]
public class TimeScriptable : ScriptableObject
{
    public List<DayNightDataProfile_Scriptable> dayNightDataProfiles;
}
