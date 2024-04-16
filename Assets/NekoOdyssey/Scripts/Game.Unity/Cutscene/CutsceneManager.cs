using System;
using System.Collections;
using System.Collections.Generic;
using NekoOdyssey.Scripts;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

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
        SiteRunner.Instance.Core.Site.MoveToNextSite();
    }
}
