using DG.Tweening;
using NekoOdyssey.Scripts;
using UniRx;
using UnityEngine;

public class EndDayResult_CanvasController : MonoBehaviour
{
    public float fadeDuration = 1f;

    private CanvasGroup _canvasGroup;
    
    [SerializeField] public CanvasGroup subCanvasGroup;
    
    public Subject<Unit> OnEndDayResultStart { get; } = new();
    public Subject<Unit> OnEndDayResultFinish { get; } = new();


    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        _canvasGroup.LerpAlpha(0, 0);
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
        _canvasGroup.DOFade(1, fadeDuration).OnComplete(() =>
        {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;

            subCanvasGroup.LerpAlpha(1, fadeDuration);

        });
    }
    void ClosePanel()
    {
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.DOFade(0, fadeDuration).OnComplete(() =>
        {
            OnEndDayResultFinish.OnNext(Unit.Default);
        });

    }

    void CheckInput()
    {
        Debug.Log("Check input");
        if (_canvasGroup.interactable)
        {
            Debug.Log("Check input pass");

            _canvasGroup.DOFade(0, fadeDuration);

            ClosePanel();
        }
    }
}
