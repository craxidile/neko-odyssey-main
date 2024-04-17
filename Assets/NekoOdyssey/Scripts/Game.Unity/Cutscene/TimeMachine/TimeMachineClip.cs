using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TimeMachineClip : PlayableAsset, ITimelineClipAsset
{
    [HideInInspector]
    public TimeMachineBehaviour template = new TimeMachineBehaviour();

    public TimeMachineBehaviour.TimeMachineAction action;
    public TimeMachineBehaviour.Condition condition;
    public string markerToJumpTo = "", markerLabel = "";
    public float timeToJumpTo = 0f;


    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<TimeMachineBehaviour>.Create(graph, template);
        TimeMachineBehaviour timeMachine = playable.GetBehaviour();
        timeMachine.markerToJumpTo = markerToJumpTo;
        timeMachine.action = action;
        timeMachine.condition = condition;
        timeMachine.markerLabel = markerLabel;
        timeMachine.timeToJumpTo = timeToJumpTo;

        return playable;
    }
}
