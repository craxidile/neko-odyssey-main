using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

using NekoOdyssey.Scripts.Game.Core.Routine;
using DialogAnswer = NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogAnswerEntity.Models.DialogAnswer;

public class PlayerChoiceDialogueController : MonoBehaviour
{
    [SerializeField] GameObject uiChoicePrefab;
    List<Button> buttons = new List<Button>();
    RectTransform rect;
    private void Awake()
    {
        rect = GetComponent<RectTransform>();

        Routine.playerChoiceDialogueController = this;
        RoutineManger.playerChoiceDialogueController = this;

        buttons = new List<Button>()
        {
            uiChoicePrefab.GetComponent<Button>()
        };

        SetVisible(false);
    }


    public void ShowChoice(QuestDialogue[] questDialogues, Action<QuestDialogue> callback)
    {
        SetVisible(true);
        for (int i = 0; i < questDialogues.Length; i++)
        {
            var choice = questDialogues[i];
            var button = buttons.Count <= i ? CreateNewChoiceButton() : buttons[i];


            var choiceText = button.GetComponentInChildren<Text>();
            choiceText.text = choice.message;


            //*** need to change this part layer ***//
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                callback?.Invoke(choice);

                SetVisible(false);
            });
        }
    }
    public void ShowChoice(ICollection<DialogAnswer> choices, Action<DialogAnswer> callback)
    {
        //Debug.Log($"ShowChoice");

        var canvasPosition = rect.anchoredPosition;

        var currentChatBalloonPosiiton = NekoOdyssey.Scripts.GameRunner.Instance.Core.RoutineManger.ChatBalloonManager.CurrentBalloon.parent.transform.position;
        //var player = NekoOdyssey.Scripts.GameRunner.Instance.Core.Player.Position;
        //bool isMirror = Camera.main.WorldToScreenPoint(currentChatBalloonPosiiton).x > Camera.main.WorldToScreenPoint(player).x;
        //rect.anchoredPosition = new Vector2(Mathf.Abs(canvasPosition.x) * (isMirror ? -1 : 1), canvasPosition.y);

        rect.position = Camera.main.WorldToScreenPoint(currentChatBalloonPosiiton);
        //rect.position = new Vector3(Screen.width/2 , rect.position.y , rect.position.z);


        SetVisible(true);
        for (int i = 0; i < choices.Count; i++)
        {
            var choice = choices.ElementAt(i);
            var button = buttons.Count <= i ? CreateNewChoiceButton() : buttons[i];


            var choiceText = button.GetComponentInChildren<Text>();
            choiceText.text = choice.Original;


            //*** need to change this part layer ***//
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                callback?.Invoke(choice);

                SetVisible(false);
            });
        }
    }

    public Button CreateNewChoiceButton()
    {
        var newButton = Instantiate(uiChoicePrefab, uiChoicePrefab.transform.parent).GetComponent<Button>();
        buttons.Add(newButton);

        return newButton;
    }


    void SetVisible(bool trigger)
    {
        foreach (var button in buttons)
        {
            button.gameObject.SetActive(trigger);
        }
    }
}
