using System;
using System.Collections;
using System.Collections.Generic;
using NekoOdyssey.Scripts;
using NekoOdyssey.Scripts.Game.Unity.MainMenu;
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
        Debug.Log($">>main_menu<< cutscene done");
        isDone = true;
        var gameStarter = GetComponent<GameStarter>();
        if (gameStarter && SiteRunner.Instance.Core.Site.GameStarted)
        {
            Debug.Log($">>main_menu<< move_to_last_visited");
            SiteRunner.Instance.Core.Site.MoveToLastVisitedSite();
        }
        else
        {
            Debug.Log($">>main_menu<< move_to_intro");
            SiteRunner.Instance.Core.Site.MoveToNextSite();
        }
    }
}
