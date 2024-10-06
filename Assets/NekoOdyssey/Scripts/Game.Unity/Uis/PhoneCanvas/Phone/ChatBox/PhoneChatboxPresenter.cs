using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PhoneChatboxPresenter : MonoBehaviour
{
    [SerializeField] GameObject playerObject, npcObject;
    GameObject _currentObject => playerObject.activeSelf ? playerObject : npcObject;

    [SerializeField] Image[] avatarImages;
    [SerializeField] Text[] chatTexts;
    [SerializeField] Image[] chatImages;


    RectSizeOverride rectSizeOverride;

    public void SetChatBoxSide(bool isPlayer)
    {
        if (playerObject != null) playerObject.SetActive(isPlayer);
        if (npcObject != null) npcObject.SetActive(!isPlayer);
    }

    public void SetText(string text)
    {
        if (chatTexts == null || chatTexts.Length == 0) return;
        foreach (var chatText in chatTexts)
        {
            chatText.text = text;
        }


        if (rectSizeOverride == null) rectSizeOverride = gameObject.AddComponent<RectSizeOverride>();
        rectSizeOverride.SetTargetRect(_currentObject.GetComponent<RectTransform>());
    }
    public void SetAvatar(Sprite sprite)
    {
        if (avatarImages == null || avatarImages.Length == 0) return;
        foreach (var avatarImage in avatarImages)
        {
            avatarImage.sprite = sprite;
        }
    }
    public void SetImageMessage(Sprite sprite)
    {
        if (chatImages == null || chatImages.Length == 0) return;
        foreach (var chatImage in chatImages)
        {
            chatImage.sprite = sprite;
        }
    }
}
