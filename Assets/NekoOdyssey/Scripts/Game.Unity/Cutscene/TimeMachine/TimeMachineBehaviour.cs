using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class TimeMachineBehaviour : PlayableBehaviour
{
    public TimeMachineAction action;
    public Condition condition;
    public string markerToJumpTo, markerLabel;
    public float timeToJumpTo;
    public bool waitInput;
    [HideInInspector]
    public bool clipExecuted = false; //the user shouldn't author this, the Mixer does

    public bool ConditionMet(object playerData)
    {
        var manager = playerData as DialogueManager;
        switch (condition)
        {
            case Condition.Always:
                return true;

            case Condition.WaitInput:
                return !manager.endBubble;

            case Condition.Never:
            default:
                return false;
        }
    }

    public enum TimeMachineAction
    {
        Marker,
        JumpToTime,
        JumpToMarker,
        Pause,
        ResetWaitInput,
    }

    public enum Condition
    {
        Always,
        Never,
        WaitInput,
    }
}
