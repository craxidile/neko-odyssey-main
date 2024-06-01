using NekoOdyssey.Scripts.Constants;
using System.Collections;
using System.Collections.Generic;
using NekoOdyssey.Scripts.Game.Unity.CustomAttribute;
using UnityEngine;
using UniRx;

[CreateAssetMenu(fileName = "TimeScriptable", menuName = "ScriptableObjects/TimeScriptable")]
[System.Serializable]
public class TimeScriptable : ScriptableObject
{
    //public Day currentDay;
    [Tooltip("1 is first day (monday)")] public int currentDayCount = 1;
    [Range(0, AppConstants.Time.MaxDayMinute)] public int dayMinute;

    [ReadOnlyField]
    [SerializeField]
    string currentTimeText;

    public float timeMultiplier = 1f;
    public bool _isTimer;

    [Tooltip("increase this value mean hungry decrease faster")]
    //[Range(0, 10)]
    public float hungryOverTimeMultiplier = 1;


    [Space(50)]
    public List<DayNightDataProfile_Scriptable> dayNightDataProfiles;


    public Subject<Unit> OnValidated = new();
    private void OnValidate()
    {
        Debug.Log("ScriptableObject validate");
        currentTimeText = new TimeHrMin($"{dayMinute / 60}:{dayMinute % 60}").ToString();

        OnValidated.OnNext(Unit.Default);
    }
}
