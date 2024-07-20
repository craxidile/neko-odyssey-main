using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneFriendListPresenter : MonoBehaviour
{
    public ButtonHover button;
    [SerializeField] Image avatarImage;
    [SerializeField] Text npcNameText, descriptionText, notificationText;
    [SerializeField] CanvasGroup notificationGroup;

    public string chatGroupId { get; set; }


    public void SetupAppearance(string chatGroupId)
    {
        this.chatGroupId = chatGroupId;

        var npcAsset = ScriptableHolder.Instance.GetNpcAsset(chatGroupId);
        avatarImage.sprite = npcAsset.phoneChatProfileIcon;
        npcNameText.text = npcAsset.npcId; //FIX THIS part later

        notificationGroup.alpha = 0;


    }
    public void UpdateMessage(string message)
    {
        notificationGroup.alpha = 1;

        descriptionText.text = message;
    }
}
