using NekoOdyssey.Scripts.Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[CreateAssetMenu(fileName = "TimeScriptable", menuName = "ScriptableObjects/TimeScriptable")]
public class TimeScriptable : ScriptableObject
{
    public Day currentDay;
    [Range(0, AppConstants.Time.MaxDayMinute)] public int dayMinute;

    [ReadOnlyField]
    [SerializeField]
    string currentTimeText;

    public float timeMultiplier = 1f;
    public bool _isTimer;

    [Tooltip("increase this value mean hungey decrease faster")]
    //[Range(0, 10)]
    public float hungryOverTimeMultiplier = 1;


    [Space(50)]
    public List<DayNightDataProfile_Scriptable> dayNightDataProfiles;


    public Subject<Unit> OnValidated = new();
    private void OnValidate()
    {
        //Debug.Log("ScriptableObject validate");
        OnValidated.OnNext(Unit.Default);

        currentTimeText = new TimeHrMin($"{dayMinute / 60}:{dayMinute % 60}").ToString();
    }
}
