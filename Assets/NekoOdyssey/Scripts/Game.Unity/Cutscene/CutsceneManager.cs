using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutSceneManager : MonoBehaviour
{
    [HideInInspector]
    public PlayableDirector director;
    [HideInInspector]
    public bool isDone;
    private void Awake()
    {
        director = GetComponent<PlayableDirector>();
        isDone = false;
}
    private void Update()
    {
        if (!director.playableGraph.IsValid() && !isDone)
        {
            CutSceneIsDone();
        }
    }

    public void CutSceneIsDone()
    {
        isDone = true;
        Debug.Log("CutScene is Done");
        
    }
}
