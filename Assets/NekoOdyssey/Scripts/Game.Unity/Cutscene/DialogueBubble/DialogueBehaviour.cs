using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

[CustomEditor(typeof(DialogueClip))]
[CanEditMultipleObjects]
public class DialogueBehaviour : PlayableBehaviour
{
    public string lineIndexID;
    public bool waitPlayerSummit;
    public bool isPaused;
    public bool startDialogue;
    public bool endDialogue;
    public GameObject PositionReference { get; set; }
    public void Initialize(GameObject gameObj)
    {
        PositionReference = gameObj;
    }
}
