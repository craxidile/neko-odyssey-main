using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace NekoOdyssey.Scripts.Game.Core.Routine
{
    public class QuestDialogueManager
    {
        //public TextAsset combineCSV;
        //public TextAsset[] multiCSV;


        //void Start()
        //{

        //}



        //void Update()
        //{

        //}


        public void Start()
        {
            //csv loader

            foreach (var dialogueCSV in GameRunner.Instance.CsvHolder.allQuestDialoguesCSV)
            {
                var questDialogueGroup = new QuestDialogueGroup(dialogueCSV.name.ToLower().Replace("_dialogue", ""));

                string[] lines = dialogueCSV.text.Split('\n');

                int indexColumn = -1, jumpColumn = -1, actorColumn = 0, firstTextColumn = 1;
                var columnHeaders = lines[0].Trim().Split(',');
                for (int i = 0; i < columnHeaders.Length; i++)
                {
                    var columnText = columnHeaders[i];
                    if (columnText.ToLowerInvariant().Equals("index")) indexColumn = i;
                    if (columnText.ToLowerInvariant().Equals("jump")) jumpColumn = i;
                    if (columnText.ToLowerInvariant().Equals("actor")) actorColumn = i;
                    if (columnText.ToLowerInvariant().Equals("th")) firstTextColumn = i;
                }


                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];
                    Debug.Log($"load csv line : {line}");

                    List<string> row = line.Trim().Split(',').ToList();

                    if (string.IsNullOrEmpty(row.FirstOrDefault())) continue;


                    //process
                    var indexText = "0";
                    if (indexColumn >= 0)
                        indexText = row[indexColumn];

                    var actor = row[actorColumn];
                    var messageText = row[firstTextColumn];

                    var choiceTargetText = "";
                    if (jumpColumn >= 0)
                        choiceTargetText = row[jumpColumn].Replace("-", "");


                    var newQuestDialogue = new QuestDialogue(indexText,
                        actor,
                        messageText,
                        choiceTargetText);

                    questDialogueGroup.questDialogues.Add(newQuestDialogue);
                }

                GameRunner.Instance.Core.Routine.allQuestDialogueGroup.Add(questDialogueGroup.questId, questDialogueGroup);
            }
        }




    }


    public class QuestDialogueGroup
    {
        public string questId { get; private set; }

        public List<QuestDialogue> questDialogues = new List<QuestDialogue>();

        //string _lastestDialogueIndex;

        public QuestDialogueGroup(string questId)
        {
            this.questId = questId;
        }


        int _currentDialogueIndex = 0;
        QuestDialogue _lastestDialogue;
        public bool isCanceled { get; set; } = false;
        public void JumpTo(QuestDialogue currentDialogue)
        {
            var listIndex = questDialogues.IndexOf(currentDialogue);
            Debug.Log($"dialogue Jump to list index {listIndex} : {currentDialogue.messageIndex}, {currentDialogue.actor}, {currentDialogue.message}");

            _currentDialogueIndex = listIndex;
        }
        public QuestDialogue GetNextDialogue(QuestDialogue previosDialogue)
        {
            var nextDialogueGroup = GetDialogueGroup(previosDialogue.choiceTarget);
            var jumpDialogue = nextDialogueGroup.FirstOrDefault();
            JumpTo(jumpDialogue);
            Debug.Log($"GetNextDialogue with condition {_currentDialogueIndex} : {previosDialogue.messageIndex}, {previosDialogue.actor}, {previosDialogue.message}");

            _lastestDialogue = jumpDialogue;
            _currentDialogueIndex++;
            return jumpDialogue;
        }
        public QuestDialogue GetNextDialogue()
        {
            if (_currentDialogueIndex < questDialogues.Count)
            {
                var nextDialogue = questDialogues[_currentDialogueIndex];

                Debug.Log($"GetNextDialogue : {_lastestDialogue?.message}, {nextDialogue?.message}");

                _currentDialogueIndex++;
                if (_lastestDialogue == null) return nextDialogue;

                if (!_lastestDialogue.choiceTarget.Equals(""))
                {
                    return GetNextDialogue(_lastestDialogue);
                }
                else
                if (_lastestDialogue.messageIndex.Equals(nextDialogue.messageIndex))
                {
                    return nextDialogue;
                }
                //_lastestDialogueIndex = targetDialogue.choiceTarget;

            }


            _currentDialogueIndex = 0;
            _lastestDialogue = null;
            return null;

        }

        public QuestDialogue[] GetDialogueGroup(string index)
        {
            Debug.Log($"GetDialogueGroup : {index}");
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
}