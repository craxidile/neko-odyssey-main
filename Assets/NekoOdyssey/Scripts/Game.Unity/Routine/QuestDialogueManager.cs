using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestDialogueManager : MonoBehaviour
{
    public TextAsset combineCSV;
    public TextAsset[] multiCSV;


    void Start()
    {

    }



    void Update()
    {

    }


    public void InitializedQuestDialogue()
    {
        //csv loader

        foreach (var dialogueCSV in multiCSV)
        {
            var questDialogueGroup = new QuestDialogueGroup(dialogueCSV.name.ToLower().Replace("_dialogue", ""));

            string[] lines = dialogueCSV.text.Split('\n');

            int indexColumn = 0, jumpColumn = -1, actorColumn = 2, firstTextColumn = 3;
            var columnHeaders = lines[0].Trim().Split(',');
            for (int i = 0; i < columnHeaders.Length; i++)
            {
                var columnText = columnHeaders[i];
                if (columnText.Equals("Index")) indexColumn = i;
                if (columnText.Equals("Jump")) jumpColumn = i;
                if (columnText.Equals("Actor")) actorColumn = i;
                if (columnText.Equals("Th")) firstTextColumn = i;
            }


            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                Debug.Log($"load csv line : {line}");

                string[] values = line.Trim().Split(',');

                List<string> row = new List<string>(values);

                if (string.IsNullOrEmpty(row.FirstOrDefault())) continue;


                //process
                var indexText = row[indexColumn];
                var actor = row[actorColumn];
                var messageText = row[firstTextColumn];

                var choiceTargetText = "";
                if (jumpColumn > 0)
                    choiceTargetText = row[jumpColumn];


                var newQuestDialogue = new QuestDialogue(indexText,
                    actor,
                    messageText,
                    choiceTargetText);

                questDialogueGroup.questDialogues.Add(newQuestDialogue);
            }

            WorldRoutineManager.allQuestDialogueGroup.Add(questDialogueGroup.questId, questDialogueGroup);
        }
    }




}


public class QuestDialogueGroup
{
    public string questId { get; private set; }

    public List<QuestDialogue> questDialogues = new List<QuestDialogue>();

    string _lastestDialogueIndex;

    public QuestDialogueGroup(string questId)
    {
        this.questId = questId;
    }


    int _currentDialogueIndex = 0;
    public QuestDialogue GetNextDialogue()
    {
        if (_currentDialogueIndex + 1 < questDialogues.Count)
        {
            _currentDialogueIndex++;

            var targetDialogue = questDialogues[_currentDialogueIndex];
            _lastestDialogueIndex = targetDialogue.choiceTarget;

            return targetDialogue;
        }
        else
        {
            _currentDialogueIndex = 0;
            return null;
        }
    }

    public QuestDialogue[] GetDialogueGroup(string index)
    {
        return questDialogues.Where(dialogue => dialogue.messageIndex == index).ToArray();
    }
}

public class QuestDialogue
{
    public string messageIndex { get; private set; }
    public string actor { get; private set; }
    public string message { get; private set; }

    public string choiceTarget { get; private set; }

    public QuestDialogue(string index, string actor, string message, string choiceTarget)
    {
        this.messageIndex = index;
        this.actor = actor;
        this.message = message;
        this.choiceTarget = choiceTarget;
    }
}