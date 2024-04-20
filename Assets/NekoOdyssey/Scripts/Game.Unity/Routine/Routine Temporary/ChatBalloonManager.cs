using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class ChatBalloonManager : MonoBehaviour
{
    public GameObject balloonPrefab;
    public float balloonScaleTime = 0.3f;

    public static ChatBalloonManager instance { get; private set; }


    class ChatBalloonData
    {
        public Transform parent;
        public GameObject chatBalloonObject;
        public NekoOdyssey.Scripts.Game.Unity.Uis.DialogCanvas.DialogCanvasController chatBalloon;
        public CanvasGroup canvasGroup;
        public string message;
        public float startTime;
    }
    List<ChatBalloonData> chatBalloonDatas = new List<ChatBalloonData>();
    ChatBalloonData _lastestBalloon;


    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //foreach (var balloon in chatBalloonDatas)
        //{
        //    balloon.dialogCanvas.messageBox.text = balloon.message;
        //}
    }

    public void ShowChatBalloon(Transform parent, string message)
    {
        var newBalloonData = chatBalloonDatas.Find(balloonData => balloonData.parent == parent);
        if (newBalloonData != null)
        {
            //newBalloonData.chatBalloon.messageBox.text = message;
            newBalloonData.chatBalloon.SetText(message);
            newBalloonData.message = message;
            newBalloonData.startTime = Time.time;
        }
        else
        {
            var newBalloon = Instantiate(balloonPrefab, parent);
            //newBalloon.transform.localPosition = Vector3.up * 0.5f;

            var chatBalloon = newBalloon.GetComponent<NekoOdyssey.Scripts.Game.Unity.Uis.DialogCanvas.DialogCanvasController>();
            chatBalloon.SetText(message);

            var canvasGroup = newBalloon.GetComponentInChildren<CanvasGroup>();

            newBalloonData = new ChatBalloonData()
            {
                parent = parent,
                chatBalloonObject = newBalloon,
                chatBalloon = chatBalloon,
                canvasGroup = canvasGroup,
                message = message,
                startTime = Time.time
            };

            chatBalloonDatas.Add(newBalloonData);


            //var originalScale = newBalloon.transform.localScale;
            //newBalloon.transform.localScale = Vector3.zero;
            //newBalloon.transform.DOScale(originalScale, balloonScaleTime).SetEase(Ease.OutExpo);
            //chatBalloon.animator.SetTrigger("OpenTrigger");

        }

        _lastestBalloon = newBalloonData;


        //flash balloon
        //newBalloonData.canvasGroup.DOFade(0, 1f).OnComplete(() =>
        //{
        //    newBalloonData.canvasGroup.DOFade(1, 1f);
        //});



        //force update chat balloon frame
        //Canvas.ForceUpdateCanvases();
        //var layoutGroup = newBalloonData.chatBalloonObject.GetComponentInChildren<UnityEngine.UI.LayoutGroup>();
        //layoutGroup.enabled = false;
        //layoutGroup.enabled = true;
    }

    public void HideChatBalloon()
    {
        chatBalloonDatas.Remove(_lastestBalloon);

        var targetObject = _lastestBalloon;
        //targetObject.chatBalloonObject.transform.DOScale(0, balloonScaleTime).SetEase(Ease.OutExpo).OnComplete(() =>
        //{
        //    Destroy(targetObject.chatBalloon.gameObject);
        //});
        //targetObject.chatBalloon.animator.SetTrigger("OpenTrigger");
        targetObject.chatBalloon.SetOpened(false);

        //DOVirtual.DelayedCall(1, () => { Destroy(targetObject.chatBalloon.gameObject); });
        DOVirtual.DelayedCall(0, () => { Destroy(targetObject.chatBalloon.gameObject); });

    }
}
