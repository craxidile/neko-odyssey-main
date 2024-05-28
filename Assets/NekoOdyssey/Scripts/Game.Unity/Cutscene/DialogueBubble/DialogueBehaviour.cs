using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DialogueBehaviour : PlayableBehaviour
{
    public string[] lineIndexID;
    public GameObject bubbleObject;
    public bool enterClip;
    public int indexCount;
    public int maxLength;
    public void Initialize(GameObject aGameObject)
    {
        bubbleObject = aGameObject;
    }
}
