using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[CreateAssetMenu(fileName = "DayNightDataProfile", menuName = "ScriptableObjects/DayNightDataProfile")]
public class DayNightDataProfile_Scriptable : ScriptableObject
{
    [Header("Enable time")]
    [Tooltip("Example : 09.35-17.55")]
    public string enableTime;

    [Header("data detail")]
    public string sceneName;
    public PostProcessProfile postProcessProfile;

}