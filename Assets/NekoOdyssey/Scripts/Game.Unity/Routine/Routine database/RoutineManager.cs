using NekoOdyssey.Scripts.Game.Unity.AssetBundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using System.Linq;

using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UniRx;

using NekoOdyssey.Scripts.Database.Domains.Npc.Commons;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.ChatGroupEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogAnswerEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogConditionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogQuestionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestGroupEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.SubDialogEntity.Models;
using DayOfWeek = NekoOdyssey.Scripts.Database.Commons.Models.DayOfWeek;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestGroupConditionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestConditionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogConditionCaseEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogLineEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.RoutineConditionEntity.Models;

namespace NekoOdyssey.Scripts.Game.Core.Routine
{
    public class RoutineManger
    {
        //public List<NpcData> npcDatas = new List<NpcData>();

        class NpcData
        {
            public string npcCode;

            public bool isAppearedInQuest, isAppearedInRoutine;
            public NpcData(string npcCode = "newNpc")
            {
                this.npcCode = npcCode;
                isAppearedInQuest = false;
                isAppearedInRoutine = false;
            }
        }
        List<NpcData> npcDatas = new();
        NpcData GetNpcData(string npcCode)
        {
            foreach (var npc in npcDatas)
            {
                if (npc.npcCode.Equals(npcCode, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    return npc;
                }
            }

            var newNpc = new NpcData(npcCode);
            npcDatas.Add(newNpc);

            return newNpc;
        }


        enum DialogType { Quest, Routine }
        class DialogueTemporaryData
        {
            public string eventCode;
            public DialogType dialogType;
            public EventPoint eventPoint;
            public UnityAction nextDialogueCallback;

            public DialogueTemporaryData(string eventCode, DialogType dialogType, EventPoint eventPoint)
            {
                this.eventCode = eventCode;
                this.dialogType = dialogType;
                this.eventPoint = eventPoint;
            }
        }
        DialogueTemporaryData currentDialog;


        //public QuestEventDetail _lastestQuestEventDetail;
        //Quest _lastestQuest;

        //UnityAction _nextDialogueCallback;
        //bool _withinDialogue = false;


        public DayNightLightingManager dayNightLightingManager;
        public ChatBalloonManager ChatBalloonManager { get; set; }
        public static PlayerChoiceDialogueController playerChoiceDialogueController { get; set; }


        public Subject<EventPoint> OnBeginEventPoint { get; } = new();
        public Subject<EventPoint> OnCompleteEventPoint { get; } = new();
        public Subject<EventPoint> OnCancelEventPoint { get; } = new();



        public void Bind()
        {
            dayNightLightingManager = new DayNightLightingManager();
            ChatBalloonManager = new ChatBalloonManager();

            AssetBundleUtils.OnReady(UpdateWorld);
        }

        public void Start()
        {
            ChatBalloonManager.Start();
            dayNightLightingManager.Start();
        }

        public void Unbind()
        {
        }




        public void UpdateWorld()
        {
            Debug.Log("UpdateWorld");
            UpdateQuestEvent();
            UpdateNpcRoutine();
        }

        void UpdateQuestEvent()
        {
            //foreach (var npcData in npcDatas) //reset story state
            //{
            //    npcData.routineEnable = true;
            //}

            var player = GameRunner.Instance.Core.Player;
            var allQuestGroups = GameRunner.Instance.Core.MasterData.NpcMasterData.QuestGroupsMasterData.QuestGroups;
            foreach (var questGroup in allQuestGroups)
            {
                //if (questGroup.questStatus == QuestGroup.QuestStatus.Completed) continue;
                if (player.IsQuestComplete(questGroup.Code)) continue;

                //if (questGroup.questStatus == QuestGroup.QuestStatus.Disable)
                //{
                //    if (!questEventManager.CheckQuestKeyAndItem(questGroup.questIdConditions, questGroup.questIdConditionsExclude)) continue;
                //    questGroup.questStatus = QuestGroup.QuestStatus.Avaliable;
                //}
                if (!CheckQuestGroupCondition(questGroup.Conditions)) continue;

                foreach (var quest in questGroup.Quests)
                {
                    //if (questEventManager.ownedQuestKey.Contains(quest.questId)) continue;//already complete
                    if (player.IsQuestComplete(quest.Code)) continue;

                    //if (!questEventManager.CheckQuestKeyAndItem(quest)) continue;//check quest key condition
                    if (!CheckQuestCondition(quest.Conditions)) continue;

                    var isInQuestDay = quest.DayOfWeekExists(GameRunner.Instance.TimeRoutine.CurrentDay.ToDayOfWeek());

                    //if (quest.IsInEventTime(GameRunner.Instance.TimeRoutine.CurrentDay, GameRunner.Instance.TimeRoutine.currentTime))
                    var startingTime = new TimeHrMin(quest.StartingHour, quest.StartingMinute);
                    var EndingTime = new TimeHrMin(quest.EndingHour, quest.EndingMinute);
                    var isInQuestTime = GameRunner.Instance.TimeRoutine.currentTime.inBetweenTime(startingTime, EndingTime);

                    var targetEventPoint = EventPoint.GetEventPoint(quest.TargetEventPoint);

                    if (isInQuestDay && isInQuestTime)
                    {
                        //quest.GetTargetEventPoint()?.gameObject.SetActive(true);
                        targetEventPoint?.gameObject.SetActive(true);


                        foreach (var actor in quest.TargetActorList)
                        {
                            //var npcData = npcDatas.Find(npc => npc.npcName == actor);
                            ////Debug.Log($"Get related npc {npcData.npcName} ({relatedCharacter})");
                            //if (npcData != null)
                            //{
                            //    //show eventPoint
                            //    npcData.routineEnable = false;
                            //    npcData.HideAllRoutine();


                            //    ////dialogue
                            //    //var dialogueGroup = allQuestDialogueGroup[questEventDetail.questId];
                            //    //npcData.dialogueGroup = dialogueGroup;
                            //}

                            var npcData = GetNpcData(actor);
                            npcData.isAppearedInQuest = true;
                        }


                        //talk to npc
                        //var dialogueActors = quest.GetTargetEventPoint()?.GetComponentsInChildren<DialogueActor>();
                        var dialogueActors = targetEventPoint?.GetComponentsInChildren<DialogueActor>();
                        //var eventPointInteractive = quest.GetTargetEventPoint()?.GetComponent<EventPointInteractive>();
                        var eventPointInteractive = targetEventPoint?.GetComponent<EventPointInteractive>();
                        if (dialogueActors != null && dialogueActors.Length > 0 && eventPointInteractive != null)
                        {
                            eventPointInteractive.OnInteractive = () =>
                            {
                                var dialogueTemporaryData = new DialogueTemporaryData(quest.Code, DialogType.Quest, targetEventPoint);
                                ConversationHandle(quest.Dialog, dialogueTemporaryData);
                            };
                        }

                    }
                    else //outside event time
                    {
                        //quest.GetTargetEventPoint()?.gameObject.SetActive(false);
                        targetEventPoint?.gameObject.SetActive(false);

                        //foreach (var keyValue in quest.relatedCharactersRoutineDisable)
                        //{
                        //    if (keyValue.Value == true)
                        //    {
                        //        var npcData = npcDatas.Find(npc => npc.npcName == keyValue.Key);
                        //        npcData.routineEnable = false;
                        //    }

                        //}
                        if (quest.DisableRoutine)
                        {
                            foreach (var actor in quest.TargetActorList)
                            {
                                //var npcData = npcDatas.Find(npc => npc.npcName == actor);
                                //npcData.routineEnable = false;

                                var npcData = GetNpcData(actor);
                                npcData.isAppearedInQuest = true;
                            }
                        }

                    }

                }
            }
        }

        bool CheckQuestGroupCondition(ICollection<QuestGroupCondition> conditionList)
        {
            foreach (var condition in conditionList)
            {
                if (!CheckCondition(condition.Type, condition.Code, condition.Operator, condition.Value)) return false;
            }
            return true;
        }
        bool CheckQuestCondition(ICollection<QuestCondition> conditionList)
        {
            foreach (var condition in conditionList)
            {
                if (!CheckCondition(condition.Type, condition.Code, condition.Operator, condition.Value)) return false;
            }
            return true;
        }
        bool CheckDialogueCondition(ICollection<DialogConditionCase> conditionList)
        {
            foreach (var condition in conditionList)
            {
                if (!CheckCondition(condition.Type, condition.Code, condition.Operator, condition.Value)) return false;
            }
            return true;
        }
        bool CheckRoutineCondition(ICollection<RoutineCondition> conditionList)
        {
            foreach (var condition in conditionList)
            {
                if (!CheckCondition(condition.Type, condition.Code, condition.Operator, condition.Value)) return false;
            }
            return true;
        }

        bool CheckCondition(string type, string code, string operation, int value)
        {
            if (type == "Quest Key" || type == "Quest Group Key")
            {
                if (!GameRunner.Instance.Core.Player.IsQuestComplete(code)) return false;
            }
            if (type == "Exclude Quest Key" || type == "Exclude Quest Group Key")
            {
                if (GameRunner.Instance.Core.Player.IsQuestComplete(code)) return false;
            }

            if (type == "Item Count")
            {
                var itemCount = GameRunner.Instance.Core.Player.Bag.CheckBagItem(code);
                if (!ExtecuteOperation(itemCount, value, operation)) return false;
            }
            if (type == "Stamina")
            {
                var stamina = GameRunner.Instance.Core.Player.Stamina.Stamina;
                if (!ExtecuteOperation(stamina, value, operation)) return false;
            }
            if (type == "Followers")
            {
                var follower = GameRunner.Instance.Core.Player.FollowerCount;
                if (!ExtecuteOperation(follower, value, operation)) return false;
            }
            if (type == "Likes")
            {
                var like = GameRunner.Instance.Core.Player.LikeCount;
                if (!ExtecuteOperation(like, value, operation)) return false;
            }
            if (type == "Money")
            {
                var money = GameRunner.Instance.Core.Player.PocketMoney;
                if (!ExtecuteOperation(money, value, operation)) return false;
            }

            return true;
        }
        bool ExtecuteOperation(int a, int b, string opeator)
        {
            bool result = false;
            switch (opeator)
            {
                case ">=":
                    result = a >= b;
                    break;
                case "<=":
                    result = a <= b;
                    break;
                case ">":
                    result = a > b;
                    break;
                case "<":
                    result = a < b;
                    break;
                case "==":
                    result = a == b;
                    break;
                case "!=":
                    result = a != b;
                    break;
                default:
                    break;
            }
            Debug.Log($"{a} {opeator} {b} = {result}");
            return result;
        }



        //public void ConversationHandle(QuestEventDetail questEventDetail)
        void ConversationHandle(Dialog dialog, DialogueTemporaryData dialogueData)
        {
            Debug.Log("ConversationHandle 0");
            //if (_withinDialogue)
            if (currentDialog != null)
            {
                Debug.Log($"ConversationHandle _nextDialogueCallback");
                currentDialog.nextDialogueCallback?.Invoke();
            }
            else
            {
                Debug.Log($"ConversationHandle 1 {currentDialog.eventCode}");
                //var targetEventPoint = EventPoint.GetEventPoint(quest.TargetEventPoint);

                //var dialogueActors = targetEventPoint?.GetComponentsInChildren<DialogueActor>();
                //var eventPointInteractive = targetEventPoint?.GetComponent<EventPointInteractive>();


                //_lastestQuest = quest;
                currentDialog = dialogueData;

                //var dialogueGroup = allQuestDialogueGroup[quest.Code];
                //var dialogue = quest.Dialog;
                //var dialogueMessage = dialogue.GetNextDialogue();
                if (dialog == null) return;
                ExecuteDialog(dialog);

            }
        }

        void ShowDialog(string dialog, string actor)
        {
            //var targetEventPoint = EventPoint.GetEventPoint(_lastestQuest.TargetEventPoint);
            var targetEventPoint = currentDialog.eventPoint;
            var dialogueActors = targetEventPoint?.GetComponentsInChildren<DialogueActor>();
            var targetActor = dialogueActors.FirstOrDefault(dialogActor => dialogActor.actorId.Equals(actor, System.StringComparison.InvariantCultureIgnoreCase));
            if (targetActor != null)
            {
                GameRunner.Instance.Core.PlayerMenu.GameObject = targetActor.gameObject;
                GameRunner.Instance.Core.Player.SetMode(PlayerMode.QuestConversation);

                if (!string.IsNullOrEmpty(dialog))
                {
                    ChatBalloonManager.ShowChatBalloon(targetActor.transform, dialog);
                }
                else
                {
                    ChatBalloonManager.HideChatBalloon();
                }
            }
            else
            {
                ChatBalloonManager.HideChatBalloon();
                Debug.Log($"npc actor {actor} cannot found ({dialog})");
            }
        }

        void ExecuteDialog(Dialog dialog)
        {
            Debug.Log($"ConversationHandle 2");
            Debug.Log($">>test_npc<< >>dialog<< {dialog.Code}");

            IDialogNextEntity next;
            next = dialog.NextEntity;
            if (next is SubDialog)
            {
                ExecuteSubDialog(next as SubDialog);
            }
            else if (next is DialogQuestion)
            {
                ExecuteQuestion(next as DialogQuestion);
            }
            else if (next is DialogCondition)
            {
                ExecuteCondition(next as DialogCondition);
            }
        }

        void ExecuteSubDialog(SubDialog subDialog, int lineIndex = 0)
        {
            Debug.Log($">>sub_dialog<< {subDialog.Id} / lineIndex : {lineIndex}");

            if (subDialog.Lines.Count > lineIndex)
            {
                // var a = new Animator();
                var line = subDialog.Lines.ElementAt(lineIndex);

                // if (line.AnimatorParamValue == "true")
                // {
                //     a.SetBool(line.AnimatorParam, true);
                // }
                // else if (line.AnimatorParamValue == "false")
                // {
                //     a.SetBool(line.AnimatorParam, false);
                // }
                // else if (line.AnimatorParamValue == "trigger")
                // {
                //     a.SetTrigger(line.AnimatorParam);
                // }

                Debug.Log($">>test_npc<< >>line<< {line.Actor} {line.GetLocalisedText()} <color=purple>{line.AnimatorParam} {line.AnimatorParamValue}</color> <color=green>{line.Photo}</color>");

                //var targetEventPoint = EventPoint.GetEventPoint(_lastestQuest.TargetEventPoint);
                //var dialogueActors = targetEventPoint?.GetComponentsInChildren<DialogueActor>();
                //var targetActor = dialogueActors.FirstOrDefault(actor => actor.actorId.Equals(line.Actor, System.StringComparison.InvariantCultureIgnoreCase));
                //if (targetActor != null)
                //{
                //    GameRunner.Instance.Core.PlayerMenu.GameObject = targetActor.gameObject;
                //    GameRunner.Instance.Core.Player.SetMode(PlayerMode.QuestConversation);

                //    if (!string.IsNullOrEmpty(line.Original))
                //    {
                //        ChatBalloonManager.ShowChatBalloon(targetActor.transform, line.GetLocalisedText());
                //    }
                //    else
                //    {
                //        ChatBalloonManager.HideChatBalloon();
                //    }
                //}
                //else
                //{
                //    ChatBalloonManager.HideChatBalloon();
                //    Debug.Log($"npc actor {line.Actor} cannot found ({line.GetLocalisedText()})");
                //}

                ShowDialog(line.GetLocalisedText(), line.Actor);


                currentDialog.nextDialogueCallback = () =>
                {
                    ExecuteSubDialog(subDialog, lineIndex + 1);
                };
            }
            else
            {
                var childFlag = subDialog.DialogChildFlag;
                switch (childFlag)
                {
                    case DialogChildFlag.HasQuestion:
                        ExecuteQuestion(subDialog.Question);
                        break;
                    case DialogChildFlag.HasCondition:
                        ExecuteCondition(subDialog.Condition);
                        break;
                    case DialogChildFlag.End:
                        CompleteQuest();
                        break;
                    case DialogChildFlag.Cancel:
                        CancelQuest();
                        break;
                    default:
                        return;
                }
            }

        }

        private void ExecuteQuestion(DialogQuestion question)
        {
            //Debug.Log($">>test_npc<< >>question<< {question.Original}");
            //foreach (var answer in question.Answers)
            //{
            //    Debug.Log($">>test_npc<< >>answer<< {answer.Original}");
            //}

            //Debug.Log($"ExecuteQuestion");
            //Debug.Log($"{question.Answers.Count}");
            //foreach (var item in question.Answers)
            //{
            //    Debug.Log($"{item.ChildFlag.ToString()}");

            //}

            var dialog = question.GetLocalisedText();
            if (!string.IsNullOrEmpty(dialog)) ShowDialog(dialog, question.Actor);

            currentDialog.nextDialogueCallback = () =>
            {
                playerChoiceDialogueController.ShowChoice(question.Answers, answer =>
                {
                    //Debug.Log($"ExecuteQuestion got answer");
                    var childFlag = answer?.DialogChildFlag;
                    //Debug.Log($"ExecuteQuestion {childFlag}");
                    switch (childFlag)
                    {
                        case DialogChildFlag.HasSubDialog:
                            ExecuteSubDialog(answer.SubDialog);
                            break;
                        case DialogChildFlag.HasQuestion:
                            ExecuteQuestion(answer.Question);
                            break;
                        case DialogChildFlag.HasCondition:
                            ExecuteCondition(answer.Condition);
                            break;
                        case DialogChildFlag.End:
                            CompleteQuest();
                            break;
                        case DialogChildFlag.Cancel:
                            CancelQuest();
                            break;
                        default:
                            break;
                    }

                });

                //ChatBalloonManager.HideChatBalloon();
                ChatBalloonManager.GrayBalloon();
            };



        }

        private void ExecuteCondition(DialogCondition condition)
        {
            var valid = CheckDialogueCondition(condition.ConditionCases);

            foreach (var conditionCase in condition.ConditionCases)
            {
                Debug.Log(
                    $">>test_npc<< >>condition<< {conditionCase.Type} {conditionCase.Code} {conditionCase.Operator} {conditionCase.Value}");
            }

            var options = condition.Options;
            var selectedOption = options.FirstOrDefault(o => o.Valid == valid);

            var childFlag = selectedOption?.DialogChildFlag;
            switch (childFlag)
            {
                case DialogChildFlag.HasSubDialog:
                    ExecuteSubDialog(selectedOption.SubDialog);
                    break;
                case DialogChildFlag.HasQuestion:
                    ExecuteQuestion(selectedOption.Question);
                    break;
                case DialogChildFlag.HasCondition:
                    ExecuteCondition(selectedOption.Condition);
                    break;
                case DialogChildFlag.End:
                    CompleteQuest();
                    break;
                case DialogChildFlag.Cancel:
                    CancelQuest();
                    break;
                default:
                    return;
            }
        }

        public void CompleteQuest()
        {
            EndDialogue();

            //var targetEventPoint = EventPoint.GetEventPoint(_lastestQuest.TargetEventPoint);
            currentDialog.eventPoint.gameObject.SetActive(false);

            Debug.Log("Complete dialogue");

            //questEventManager.ownedQuestKey.Add(_lastestQuest.Code);
            if (currentDialog.dialogType == DialogType.Quest)
            {
                GameRunner.Instance.Core.Player.AddAchievedQuest(currentDialog.eventCode);
            }

            //_withinDialogue = false;
            currentDialog = null;

            //UpdateWorld();
        }
        public void CancelQuest()
        {
            EndDialogue();

            //_withinDialogue = false;
            currentDialog = null;
        }

        void EndDialogue()
        {
            //complete talking
            ChatBalloonManager.HideChatBalloon();

            //restore player control
            GameRunner.Instance.Core.Player.SetMode(PlayerMode.Move);
        }





        void UpdateNpcRoutine()
        {
            //foreach (var npcData in npcDatas) //do routine for other npcs
            //{
            //    if (!npcData.isAppearedInQuest)
            //    {
            //        var enabledRoutine = npcData.UpdateRoutine();

            //        if (enabledRoutine != null)
            //        {
            //            if (!enabledRoutine.dialogueKey.Equals("-"))
            //            {
            //                //Debug.Log($"Add dialogue key : {enabledRoutine.dialogueKey}");
            //                AddRoutineDialogue(enabledRoutine.GetTargetEventPoint(), enabledRoutine.dialogueKey);
            //            }
            //            //Debug.Log($"add routine target = {enabledRoutine.targetEventPoint.name} , dialogue = {enabledRoutine.dialogueKey}");

            //        }
            //    }
            //}

            var player = GameRunner.Instance.Core.Player;
            var routines = GameRunner.Instance.Core.MasterData.NpcMasterData.RoutinesMasterData.Routines;
            foreach (var routine in routines)
            {
                Debug.Log($"Check routine : {routine.Code}");

                //if (player.IsQuestComplete(questGroup.Code)) continue;

                if (!CheckRoutineCondition(routine.Conditions)) continue;

                bool routineEnable = true;
                foreach (var actor in routine.TargetActorList)
                {
                    var npcData = GetNpcData(actor);
                    if (npcData.isAppearedInQuest || npcData.isAppearedInRoutine)
                    {
                        routineEnable = false;
                        break;
                    }
                }
                if (!routineEnable) continue;

                Debug.Log($"routine is enable");

                var isInEventDay = routine.DayOfWeekExists(GameRunner.Instance.TimeRoutine.CurrentDay.ToDayOfWeek());

                var startingTime = new TimeHrMin(routine.StartingHour, routine.StartingMinute);
                var EndingTime = new TimeHrMin(routine.EndingHour, routine.EndingMinute);
                var isInQuestTime = GameRunner.Instance.TimeRoutine.currentTime.inBetweenTime(startingTime, EndingTime);

                var targetEventPoint = EventPoint.GetEventPoint(routine.TargetEventPoint);

                if (isInEventDay && isInQuestTime)
                {
                    Debug.Log($"routine is in event time");

                    foreach (var actor in routine.TargetActorList)
                    {
                        var npcData = GetNpcData(actor);
                        npcData.isAppearedInRoutine = true;
                    }

                    if (targetEventPoint == null) continue;
                    Debug.Log($"routine {routine.Code} got eventPoint");
                    targetEventPoint.gameObject.SetActive(true);

                    //talk to npc
                    var dialogueActors = targetEventPoint.GetComponentsInChildren<DialogueActor>();
                    var eventPointInteractive = targetEventPoint.GetComponent<EventPointInteractive>();
                    if (dialogueActors != null && dialogueActors.Length > 0 && eventPointInteractive != null)
                    {
                        Debug.Log($"routine {routine.Code} ready for action");
                        eventPointInteractive.OnInteractive = () =>
                        {
                            var dialogueTemporaryData = new DialogueTemporaryData(routine.Code, DialogType.Routine, targetEventPoint);
                            ConversationHandle(routine.Dialog, dialogueTemporaryData);

                        };
                    }

                }

            }
            //void AddRoutineDialogue(EventPoint eventPoint, string dialogueGroupId)
            //{
            //    var dialogueActors = eventPoint.GetComponentsInChildren<DialogueActor>();
            //    var eventPointInteractive = eventPoint.GetComponent<EventPointInteractive>();
            //    if (dialogueActors.Length == 0 || eventPointInteractive == null) return;

            //    eventPointInteractive.OnInteractive = () =>
            //    {
            //        //var dialogueGroup = allQuestDialogueGroup[dialogueGroupId];
            //        QuestDialogueGroup dialogueGroup = null;
            //        var dialogueMessage = dialogueGroup.GetNextDialogue();

            //        Debug.Log($"interactive event point");

            //        if (dialogueMessage != null)
            //        {
            //            Debug.Log($"npc Talk quest id : {dialogueGroupId} , text : {dialogueMessage.messageIndex}, {dialogueMessage.message}");

            //            if (dialogueMessage.messageIndex.Equals("choice"))
            //            {
            //                var choiceGroup = dialogueGroup.GetDialogueGroup(dialogueMessage.messageIndex);
            //                playerChoiceDialogueController.ShowChoice(choiceGroup, choice =>
            //                {
            //                    var nextDialogue = dialogueGroup.GetNextDialogue(choice);

            //                    //same as #else1
            //                    if (nextDialogue != null)
            //                    {
            //                        dialogueMessage = nextDialogue;
            //                        var targetActor = dialogueActors.FirstOrDefault(actor => actor.actorId == dialogueMessage.actor);
            //                        if (targetActor != null)
            //                        {
            //                            ChatBalloonManager.ShowChatBalloon(targetActor.transform, dialogueMessage.message);
            //                            //NekoOdyssey.Scripts.GameRunner.Instance.Core.Player.SetMode(NekoOdyssey.Scripts.Game.Unity.Game.Core.PlayerMode.Conversation);
            //                        }
            //                        else
            //                        {
            //                            Debug.Log($"npc actor {dialogueMessage.actor} cannot found ({dialogueMessage.messageIndex}/{dialogueMessage.message})");
            //                        }

            //                        if (dialogueMessage.choiceTarget.ToLowerInvariant().Equals("cancel"))
            //                        {
            //                            dialogueGroup.isCanceled = true;
            //                        }
            //                    }

            //                    //Debug.Log($"Check index : {dialogueGroup._currentDialogueIndex}");
            //                });

            //                ChatBalloonManager.HideChatBalloon();
            //            }
            //            else
            //            {
            //                //#else1
            //                var targetActor = dialogueActors.FirstOrDefault(actor => actor.actorId == dialogueMessage.actor);
            //                if (targetActor != null)
            //                {
            //                    ChatBalloonManager.ShowChatBalloon(targetActor.transform, dialogueMessage.message);
            //                    GameRunner.Instance.Core.PlayerMenu.GameObject = targetActor.gameObject;
            //                    GameRunner.Instance.Core.Player.SetMode(PlayerMode.QuestConversation);
            //                }
            //                else
            //                {
            //                    Debug.Log($"npc actor {dialogueMessage.actor} cannot found ({dialogueMessage.messageIndex}/{dialogueMessage.message})");
            //                }
            //            }

            //            if (dialogueMessage.choiceTarget.ToLowerInvariant().Equals("cancel"))
            //            {
            //                dialogueGroup.isCanceled = true;
            //            }
            //        }
            //        else
            //        {
            //            //complete talking
            //            //restore player control
            //            ChatBalloonManager.HideChatBalloon();
            //            GameRunner.Instance.Core.Player.SetMode(PlayerMode.Move);

            //            if (!dialogueGroup.isCanceled)
            //            {
            //                Debug.Log("Complete dialogue");

            //                //UpdateWorld();
            //            }
            //            else
            //            {
            //                Debug.Log("Cancel dialogue");
            //                dialogueGroup.isCanceled = false;
            //            }


            //        }

            //    };

        }

    }
}
