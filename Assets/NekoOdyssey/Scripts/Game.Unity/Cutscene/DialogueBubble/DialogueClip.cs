using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class DialogueClip : PlayableAsset
{
    public string lineIndexID;
    //public bool waitPlayerSummit = true;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<DialogueBehaviour>.Create(graph);
        DialogueBehaviour dialogueBehaviour = playable.GetBehaviour();
        dialogueBehaviour.lineIndexID = lineIndexID;
        //dialogueBehaviour.waitPlayerSummit = waitPlayerSummit;
        //dialogueBehaviour.isLooped = false;

        return playable;
    }
}
