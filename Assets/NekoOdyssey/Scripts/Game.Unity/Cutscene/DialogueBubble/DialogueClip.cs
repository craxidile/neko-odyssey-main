using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

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
        dialogueBehaviour.indexCount = 0;
        dialogueBehaviour.enterClip = false;
        dialogueBehaviour.maxLength = lineIndexID.Length;
        if (gameObject != null)
        {
            dialogueBehaviour.Initialize(gameObject);
        }
        //dialogueBehaviour.waitPlayerSummit = waitPlayerSummit;
        //dialogueBehaviour.isLooped = false;

        return playable;
    }
}
