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
    private void Awake()
    {
        Routine.playerChoiceDialogueController = this;

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
