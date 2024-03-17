using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class ChatBalloonManager : MonoBehaviour
{
    public GameObject balloonPrefab;

    public static ChatBalloonManager instance { get; private set; }
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

    }

    public void ShowChatBalloon(Transform parent, string message)
    {
        var newBalloon = Instantiate(balloonPrefab, parent);
        newBalloon.transform.localPosition = Vector3.up * 0.5f;

        var chatBalloon = newBalloon.GetComponent<NekoOdyssey.Scripts.Game.Unity.Uis.DialogCanvas.DialogCanvasController>();
        chatBalloon.messageBox.text = message;

        DOVirtual.DelayedCall(2, () =>
        {
            Destroy(newBalloon);
        });
    }
}
