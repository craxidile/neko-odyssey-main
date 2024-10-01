using NekoOdyssey.Scripts;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NekoOdyssey.Scripts.Database.Domains;
using NekoOdyssey.Scripts.Database.Domains.SaveV001;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DemoCutsceneManager : MonoBehaviour
{
    public PlayableDirector directorIntro, directorGoToGame;
    public GameObject buttonGroup;
    [HideInInspector]
    public bool isDone;
    void Start()
    {
        directorIntro.gameObject.SetActive(true);
        directorGoToGame.gameObject.SetActive(false);
        isDone = false;
        
        // Debug.Log($">>timeline<< {GameObject.Find("Intro")}");
        // var timeline = GameObject.Find("Intro").GetComponent<PlayableDirector>();
        // Debug.Log($">>timeline<< {timeline}");
        // if (timeline == null) return;
        // Debug.Log($">>timeline<< play");
        // timeline.time = 0;
        // timeline.Stop();
        // timeline.Evaluate();
    }

    // Update is called once per frame
    void Update()
    {
        if (!directorIntro.playableGraph.IsValid())
        {
            buttonGroup.SetActive(true);
            directorIntro.gameObject.SetActive(false);
        }
        if (!directorGoToGame.playableGraph.IsValid() && !isDone)
        {
            CutSceneIntroIsDone();
        }
        if (Input.GetKeyUp(KeyCode.KeypadEnter))
        {
            GoToGame();
            Debug.Log($">>valid<< 03 false");
        }
    }

    public void CutSceneIntroIsDone()
    {
        // TODO: Remove this after demo
        using (new SaveV001DbContext(new() { CopyMode = DbCopyMode.ForceCopy, ReadOnly = false })) ;
        
        isDone = true;
        Debug.Log("Runsite here");
    }
    public void GoToGame()
    {
        Debug.Log($">>game<< starts");
        buttonGroup.SetActive(false);
        directorGoToGame.gameObject.SetActive(true);
    }
}
