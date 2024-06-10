using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace NekoOdyssey.Scripts.Game.Core.Routine
{
    public class QuestEventManager
    {
        //public TextAsset[] testQuestCSV;
        //public TextAsset testQuestRelationship;

        //[Header("Test Quest Key")]
        public List<string> ownedQuestKey { get; set; } = new List<string>();



        public void Start()
        {
            if (GameRunner.Instance.CsvHolder == null) return;
            foreach (var csv in GameRunner.Instance.CsvHolder.allQuestsCSV)
            {
                LoadQuestCSV(csv);
            }
            LoadQuestRelationshipCSV(GameRunner.Instance.CsvHolder.questRelationshipCSV);
        }

        public void LoadQuestCSV(TextAsset textAsset)
        {
            QuestGroup newQuestGroup = new QuestGroup(textAsset.name);

            string[] lines = textAsset.text.Split('\n');

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                Debug.Log($"load csv line : {line}");

                string[] values = line.Trim().Split(',');

                List<string> row = new List<string>(values);

                if (string.IsNullOrEmpty(row.FirstOrDefault())) continue;


                //process
                var questKey = row[0]; //column 0
                var questKeyConditionsText = row[1]; //column 1
                var dayText = row[2];
                var timeText = row[3];
                var eventPointKey = row[4];
                var releatedCharactersText = row[5];
                var releatedCharactersDisableRoutine = row[6];
                var rewardText = row[7];


                var convertedDayText = dayText.Replace(' ', '-').Replace(':', '-').Replace(';', '-');
                var dayAvaliables = convertedDayText.Split('-');
                HashSet<Day> dayList = new HashSet<Day>();

                foreach (var day in dayAvaliables)
                {
                    if (System.Enum.TryParse(day, true, out Day dayResult))
                    {
                        dayList.Add(dayResult);
                    }
                }

                var convertedTimeText = timeText.Replace(' ', '-').Replace('_', '-');
                var targetTimes = convertedTimeText.Split('-');
                TimeHrMin[] eventTimeList = new TimeHrMin[] { new TimeHrMin(targetTimes[0]), new TimeHrMin(targetTimes[1]) };


                //var tragetEventPoint = EventPoint.GetEventPoint(eventPointKey);



                var convertedReleatedCharactersText = releatedCharactersText.Replace(' ', '-').Replace(':', '-').Replace(';', '-');
                var releatedCharacters = convertedReleatedCharactersText.ToLower().Split('-').ToList();

                var isRoutineDisable = bool.Parse(releatedCharactersDisableRoutine);

                var rewards = rewardText.Replace(' ', '+').Replace('-', '+').Replace(':', '-').Replace(';', '-')
                                            .Split('+').ToList();

                var questKeyConditions = questKeyConditionsText.Replace(' ', '-').Replace('+', '-').Split('-').ToList();
                var keyIdConditions = new List<string>();
                var keyIdConditionsExclude = new List<string>();
                foreach (var questCondition in questKeyConditions)
                {
                    if (!string.IsNullOrEmpty(questCondition))
                    {
                        if (questCondition.StartsWith("!"))
                        {
                            var condition = questCondition.Substring(1);
                            keyIdConditionsExclude.Add(condition.ToLower());

                        }
                        else
                        {
                            keyIdConditions.Add(questCondition.ToLower());

                        }
                    }
                }


                QuestEventDetail newQuestEventDetail = new QuestEventDetail(dayList, eventTimeList[0], eventTimeList[1], eventPointKey, keyIdConditions, keyIdConditionsExclude);
                newQuestEventDetail.questId = questKey.ToLower();

                foreach (var relatedCharacter in releatedCharacters)
                {
                    newQuestEventDetail.relatedCharacters.Add(relatedCharacter);
                    newQuestEventDetail.relatedCharactersRoutineDisable.Add(relatedCharacter, isRoutineDisable);
                }
                foreach (var reward in rewards)
                {
                    if (string.IsNullOrEmpty(reward)) continue;

                    var rewardNameAndQuantity = reward.Split('*');
                    var itemName = rewardNameAndQuantity[0];
                    int itemQuantity = 0;
                    if (rewardNameAndQuantity.Length > 1)
                    {
                        itemQuantity = int.Parse(rewardNameAndQuantity[1]);
                    }

                    newQuestEventDetail.stepRewards.Add(itemName, itemQuantity);

                    Debug.Log($"Quest step {newQuestEventDetail.questId} reward is : {itemName} x {itemQuantity}");
                }

                GameRunner.Instance.Core.Routine.allQuestEvents.Add(newQuestEventDetail);

                newQuestGroup.questEventDetails.Add(newQuestEventDetail);
            }

            GameRunner.Instance.Core.Routine.allQuestGroups.Add(newQuestGroup.questGroupId, newQuestGroup);
        }

        public void LoadQuestRelationshipCSV(TextAsset textAsset)
        {
            string[] lines = textAsset.text.Split('\n');

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                Debug.Log($"load csv line : {line}");

                string[] values = line.Trim().Split(',');

                List<string> row = new List<string>(values);

                if (string.IsNullOrEmpty(row.FirstOrDefault())) continue;


                var questKey = row[0]; //column 0
                var questKeyConditionsText = row[1]; //column 1

                GameRunner.Instance.Core.Routine.allQuestGroups.TryGetValue(questKey, out var targetQuestDetail);

                if (targetQuestDetail == null)
                {
                    Debug.LogWarning($"Original quest data of {questKey} cannot be found");
                    continue;
                }

                var questKeyConditions = questKeyConditionsText.Replace(' ', '-').Replace('+', '-').Split('-').ToList();
                targetQuestDetail.questIdConditions = new List<string>();
                targetQuestDetail.questIdConditionsExclude = new List<string>();
                foreach (var questCondition in questKeyConditions)
                {
                    if (!string.IsNullOrEmpty(questCondition))
                    {
                        if (questCondition.StartsWith("!"))
                        {
                            var condition = questCondition.Substring(1);
                            targetQuestDetail.questIdConditionsExclude.Add(condition.ToLower());

                        }
                        else
                        {
                            targetQuestDetail.questIdConditions.Add(questCondition.ToLower());

                        }
                    }
                }


                ConvertConditionText(questKeyConditionsText, out List<string> conditionText, out List<string> conditionExcludeText);

                Debug.Log($"ConvertConditionText : {conditionText.FirstOrDefault()} , {conditionExcludeText.FirstOrDefault()}");
            }
        }

        public void ConvertConditionText(string originalText, out List<string> conditionText, out List<string> conditionExcludeText)
        {
            conditionText = new List<string>();
            conditionExcludeText = new List<string>();

            var questKeyConditions = originalText.Replace(' ', '-').Replace('+', '-').Split('-').ToList();
            foreach (var questCondition in questKeyConditions)
            {
                if (!string.IsNullOrEmpty(questCondition))
                {
                    if (questCondition.StartsWith("!"))
                    {
                        var condition = questCondition.Substring(1);
                        conditionExcludeText.Add(condition.ToLower());

                    }
                    else
                    {
                        conditionText.Add(questCondition.ToLower());

                    }
                }
            }
        }


        public bool CheckQuestKeyAndItem(QuestEventDetail questDetail) => CheckQuestKeyAndItem(questDetail.keyIdConditions, questDetail.keyIdConditionsExclude);
        //{
        //foreach (var condition in questDetail.questIdConditions)
        //{
        //    Debug.Log($"CheckQuestCondition {condition}");

        //}
        //if (questDetail.questIdConditions.Count > 0)
        //{
        //    if (questDetail.questIdConditions.Any(condition => !ownedQuestKey.Contains(condition))) //check player quest owned quest id
        //    {
        //        return false;
        //    }

        //    foreach (var key in questDetail.questIdConditions)//check for player inventory item
        //    {
        //        //if (!playerInventory.contains(key))
        //        //{
        //        //return false;
        //        //}
        //    }

        //}




        //if (questDetail.questIdConditionsExclude.Count > 0 && questDetail.questIdConditionsExclude.Any(condition => ownedQuestKey.Contains(condition)))
        //{
        //    return false;
        //}


        //return true;
        //}
        public bool CheckQuestKeyAndItem(List<string> conditionList, List<string> conditionExcludeList)
        {
            if (conditionList.Count > 0)
            {
                foreach (var condition in conditionList)
                {
                    if (!ownedQuestKey.Contains(condition))
                    {
                        //if (!playerInventory.Contains(key))
                        //{


                        return false;

                        //}
                        //else
                        //{
                        //return true;
                        //}
                    }

                }
            }


            if (conditionExcludeList.Count > 0)
            {
                foreach (var condition in conditionExcludeList)
                {
                    if (ownedQuestKey.Contains(condition))
                    {
                        return false;
                    }
                }
            }


            return true;
        }
    }


    public class QuestGroup
    {
        public string questGroupId;
        public enum QuestStatus { Disable, Avaliable, InProgress, Completed }
        public QuestStatus questStatus;
        public List<QuestEventDetail> questEventDetails;

        public List<string> questIdConditions;
        public List<string> questIdConditionsExclude;

        public QuestGroup(string questGroupId)
        {
            this.questGroupId = questGroupId;

            questStatus = QuestStatus.Disable;
            questEventDetails = new List<QuestEventDetail>();
            questIdConditions = new List<string>();
            questIdConditionsExclude = new List<string>();
        }
    }
    public class QuestEventDetail : EventDetail
    {
        //public HashSet<Day> eventDays;
        //public TimeHrMin eventTimeStart, eventTimeEnd;
        //public EventPoint targetEventPoint;

        public string questId;

        public List<string> relatedCharacters;
        public Dictionary<string, bool> relatedCharactersRoutineDisable;

        public Dictionary<string, int> stepRewards;

        public QuestEventDetail(HashSet<Day> eventDays, TimeHrMin eventTimeStart, TimeHrMin eventTimeEnd, string targetEventPoint, List<string> keyIdConditions, List<string> keyIdConditionsExclude)
            : base(eventDays, eventTimeStart, eventTimeEnd, targetEventPoint, keyIdConditions, keyIdConditionsExclude)
        {
            //this.eventDays = eventDays;
            //this.eventTimeStart = eventTimeStart;
            //this.eventTimeEnd = eventTimeEnd;
            //this.targetEventPoint = targetEventPoint;

            this.relatedCharacters = new List<string>();
            this.relatedCharactersRoutineDisable = new Dictionary<string, bool>();
            this.stepRewards = new Dictionary<string, int>();
        }

        //public bool IsInEventTime(Day currentDay, TimeHrMin currentTime)
        //{
        //    if (eventDays.Contains(currentDay))
        //    {
        //        return eventTimeStart <= currentTime && currentTime <= eventTimeEnd;
        //    }
        //    return false;
        //}
    }
}