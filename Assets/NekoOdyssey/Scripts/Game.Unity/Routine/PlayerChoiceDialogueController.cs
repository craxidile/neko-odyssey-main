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
    [SerializeField] Transform uiTail;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Transform playerDialogueActor;

    List<Button> buttons = new List<Button>();

    //RectTransform _rect;
    //Vector3 _localScale;

    private void Awake()
    {
        Routine.playerChoiceDialogueController = this;
        RoutineManger.playerChoiceDialogueController = this;

        buttons = new List<Button>()
        {
            uiChoicePrefab.GetComponent<Button>()
        };

        //_rect = GetComponent<RectTransform>();
        //_localScale = _rect.localScale;
    }
    private void Start()
    {
        SetVisible(false);
    }


    public void ShowChoice(QuestDialogue[] questDialogues, Action<QuestDialogue> callback) //call by Routine (CSV version)
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
    public void ShowChoice(ICollection<DialogAnswer> choices, Action<DialogAnswer> callback) //call by RoutineManager (CMS version)
    {
        var currentChatBalloon = NekoOdyssey.Scripts.GameRunner.Instance.Core.RoutineManger.ChatBalloonManager.CurrentBalloon;
        var player = NekoOdyssey.Scripts.GameRunner.Instance.Core.Player.Position;
        bool isMirror = Camera.main.WorldToScreenPoint(currentChatBalloon.parent.transform.position).x > Camera.main.WorldToScreenPoint(player).x;

        int isMirrorMuliplier = isMirror ? -1 : 1;
        var newPosition = transform.localPosition;
        newPosition.x = Mathf.Abs(newPosition.x) * isMirrorMuliplier;
        transform.localPosition = newPosition;

        var tailScale = uiTail.localScale;
        tailScale.x = Mathf.Abs(tailScale.x) * isMirrorMuliplier;
        uiTail.localScale = tailScale;

        var npcBalloonScale = currentChatBalloon.parent.transform.localScale;
        playerDialogueActor.localScale = npcBalloonScale;

        //LayoutRebuilder.ForceRebuildLayoutImmediate(_rect);


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
        //foreach (var button in buttons)
        //{
        //    button.gameObject.SetActive(trigger);
        //}

        if (trigger)
        {
            canvasGroup.LerpAlpha(1, 0.3f);

        }
        else
        {
            canvasGroup.LerpAlpha(0, 0.3f);
        }
    }
}
