using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;
using NekoOdyssey.Scripts;

public class EndDayResult_CanvasController : MonoBehaviour
{
    public float fadeDuration = 1f;

    public Subject<Unit> OnEndDayResultStart { get; } = new();
    public Subject<Unit> OnEndDayResultFinish { get; } = new();

    CanvasGroup canvasGroup;

    [SerializeField] CanvasGroup subCanvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.LerpAlpha(0, 0);
        subCanvasGroup.LerpAlpha(0, 0);
    }


    // Start is called before the first frame update
    void Start()
    {
        OpenPanel();

        GameRunner.Instance.PlayerInputHandler.OnFireTriggerred.Subscribe(_ => CheckInput()).AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            CheckInput();
        }
    }


    void OpenPanel()
    {
        OnEndDayResultStart.OnNext(Unit.Default);
        canvasGroup.DOFade(1, fadeDuration).OnComplete(() =>
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            subCanvasGroup.LerpAlpha(1, fadeDuration);

        });
    }
    void ClosePanel()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.DOFade(0, fadeDuration).OnComplete(() =>
        {
            OnEndDayResultFinish.OnNext(Unit.Default);
        });

    }

    void CheckInput()
    {
        Debug.Log("Check input");
        if (canvasGroup.interactable)
        {
            Debug.Log("Check input pass");

            canvasGroup.DOFade(0, fadeDuration);

            ClosePanel();
        }
    }
}
