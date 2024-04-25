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
    public string[] lineIndexID;
    public ExposedReference<GameObject> bubbleObject;
    //public bool waitPlayerSummit = true;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<DialogueBehaviour>.Create(graph);
        var gameObject = bubbleObject.Resolve(graph.GetResolver());
        DialogueBehaviour dialogueBehaviour = playable.GetBehaviour();
        dialogueBehaviour.lineIndexID = lineIndexID;
        dialogueBehaviour.maxLength = lineIndexID.Length;
        dialogueBehaviour.isLooped = true;
        if (gameObject != null)
        {
            dialogueBehaviour.Initialize(gameObject);
        }
        //dialogueBehaviour.waitPlayerSummit = waitPlayerSummit;
        //dialogueBehaviour.isLooped = false;

        return playable;
    }
}
