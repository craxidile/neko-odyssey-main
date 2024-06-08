using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NekoOdyssey.Scripts;
using UniRx;
using DG.Tweening;

public class PauseCanvasController : MonoBehaviour
{
    [SerializeField] float transitionDuration = 0.3f;
    CanvasGroup canvasGroup;
    [SerializeField] ButtonHover resumeButton, titleButton, settingButton;

    bool _isShowed;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    // Start is called before the first frame update
    void Start()
    {
        GameRunner.Instance.PlayerInputHandler.OnPauseGameTriggerred
                .Subscribe(_ => PauseGameTriggered())
                .AddTo(this);


        canvasGroup.LerpAlpha(0, 0);


        resumeButton.onClick.AddListener(Resume);
        titleButton.onClick.AddListener(BackToTitle);

        settingButton.onClick.AddListener(Setting);
    }

    // Update is called once per frame
    void Update()
    {

    }


    void PauseGameTriggered()
    {
        if (!_isShowed)
        {
            ShowPanel();
        }
        else
        {
            ClosePanel();
        }
    }

    void ShowPanel()
    {
        _isShowed = true;
        Debug.Log("Show pause Panel");

        canvasGroup.DOFade(1, transitionDuration).OnComplete(() =>
        {
            canvasGroup.SetAlpha(1);

        }).SetUpdate(UpdateType.Normal, true);


        GameRunner.Instance.TimeRoutine.PauseTime();
        Time.timeScale = 0;
    }

    void ClosePanel()
    {
        _isShowed = false;
        Debug.Log("Close pause Panel");

        canvasGroup.LerpAlpha(0, transitionDuration);

        GameRunner.Instance.TimeRoutine.ContinueTime();
        Time.timeScale = 1;
    }



    void Resume()
    {
        ClosePanel();
    }
    void BackToTitle()
    {

    }
    void Setting()
    {

    }
}
