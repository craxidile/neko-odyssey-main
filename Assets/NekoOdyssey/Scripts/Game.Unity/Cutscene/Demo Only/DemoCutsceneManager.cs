using NekoOdyssey.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

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
        }
    }

    public void CutSceneIntroIsDone()
    {
        isDone = true;
        Debug.Log("Runsite here");
    }
    public void GoToGame()
    {
        buttonGroup.SetActive(false);
        directorGoToGame.gameObject.SetActive(true);
    }
}
