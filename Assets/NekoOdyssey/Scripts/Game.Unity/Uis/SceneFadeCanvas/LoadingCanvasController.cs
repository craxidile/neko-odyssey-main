using NekoOdyssey.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using NekoOdyssey.Scripts.Game.Core.GameScene;

public class LoadingCanvasController : MonoBehaviour
{
    [SerializeField] Text loadingText;
    string _loadingStarterText;
    int _loadingAnimatePhase = 0;
    [SerializeField] float loadingTextAnimationDelay = 0.3f;

    public static LoadingCanvasController instance { get; set; }
    List<LoadingCanvasController> remainingLoading = new List<LoadingCanvasController>();

    private void Awake()
    {
        if (instance != null)
        {
            remainingLoading.Add(instance);
        }

        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        _loadingStarterText = loadingText.text;

        GameRunner.Instance.Core.GameScene.OnChangeSceneMode
                .Subscribe(OnChangeScene)
                .AddTo(this);
        GameRunner.Instance.Core.GameScene.OnChangeSceneFinish
               .Subscribe(OnChangeSceneFinish)
               .AddTo(this);

        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }


    IEnumerator AnimatingLoadingText()
    {

        _loadingAnimatePhase = (_loadingAnimatePhase + 1) % 3;

        string newloadingText = _loadingStarterText;
        for (int i = 0; i < _loadingAnimatePhase + 1; i++)
        {
            newloadingText += ".";
        }

        loadingText.text = newloadingText;

        yield return new WaitForSeconds(loadingTextAnimationDelay);
        StartCoroutine(AnimatingLoadingText());
    }


    public void Show()
    {
        gameObject.SetActive(true);

        transform.parent = null;
        DontDestroyOnLoad(gameObject);

        StartCoroutine(AnimatingLoadingText());
    }
    public void Hide()
    {
        //gameObject.SetActive(false);

        foreach (var item in remainingLoading)
        {
            Destroy(item.gameObject);
        }

        remainingLoading.Clear();
    }

    void OnChangeScene(GameSceneMode gameSceneMode)
    {
        Debug.Log($"OnChangeScene {gameSceneMode}");
        if (gameSceneMode == GameSceneMode.Opening)
        {
            Hide();
        }
    }
    void OnChangeSceneFinish(GameSceneMode gameSceneMode)
    {
        Debug.Log($"OnChangeSceneFinish {gameSceneMode}");
        if (gameSceneMode == GameSceneMode.Closing)
        {
            Show();
        }
    }
}
