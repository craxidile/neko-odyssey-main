using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

public class DialogueClip : PlayableAsset
{
    //public ExposedReference<GameObject> posReference;
    public string lineIndexID;
    public bool waitPlayerSummit = true;
    //public bool startDialogue;
    //public bool endDialogue;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<DialogueBehaviour>.Create(graph);
        DialogueBehaviour dialogueBehaviour = playable.GetBehaviour();
        dialogueBehaviour.lineIndexID = lineIndexID;
        dialogueBehaviour.waitPlayerSummit = waitPlayerSummit;
        dialogueBehaviour.isPaused = false;
        //dialogueBehaviour.startDialogue = startDialogue;
        //dialogueBehaviour.endDialogue = endDialogue;
        //var gameObj = posReference.Resolve(graph.GetResolver()).gameObject;
        //if (gameObj != null)
        //{
        //    dialogueBehaviour.Initialize(gameObj);
        //}

        return playable;
    }
}
