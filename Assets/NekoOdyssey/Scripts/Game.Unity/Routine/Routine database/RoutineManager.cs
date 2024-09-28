﻿using NekoOdyssey.Scripts.Game.Unity.AssetBundles;
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
using NekoOdyssey.Scripts.Game.Unity.Player;

namespace NekoOdyssey.Scripts.Game.Core.Routine
{
    public class RoutineManger
    {
        float _enabledTime;

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
            //public Quest quest { get; set; }
            public (string Type, string Code, int Value)[] rewards;

            public DialogueTemporaryData(string eventCode, DialogType dialogType, EventPoint eventPoint)
            {
                this.eventCode = eventCode;
                this.dialogType = dialogType;
                this.eventPoint = eventPoint;
            }
        }
        DialogueTemporaryData _currentDialog;
        List<DialogueTemporaryData> _tempCompletedDialogue = new();



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

            _enabledTime = Time.time;


            if (SiteRunner.Instance.Core.Site.Ready)
            {
                UpdateWorld();
            }
            else
            {
                SiteRunner.Instance.Core.Site.OnReady.Subscribe(_ =>
                {
                    UpdateWorld();
                }).AddTo(SiteRunner.Instance);
            }
        }

        public void Start()
        {
            ChatBalloonManager.Start();
            dayNightLightingManager.Start();

            SetUpCaptureQuest();
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
        public void ForceUpdateWorld()
        {
            Debug.Log("UpdateWorld");
            EventPoint.HideAllEventPoint();
            npcDatas.Clear();
            UpdateQuestEvent();
            UpdateNpcRoutine();
        }

        void UpdateQuestEvent()
        {
            var player = GameRunner.Instance.Core.Player;
            var allQuestGroups = GameRunner.Instance.Core.MasterData.NpcMasterData.QuestGroupsMasterData.QuestGroups;
            foreach (var questGroup in allQuestGroups)
            {
                if (player.IsQuestComplete(questGroup.Code)) continue;
                if (!CheckQuestGroupCondition(questGroup.Conditions)) continue;

                foreach (var quest in questGroup.Quests)
                {
                    if (player.IsQuestComplete(quest.Code)) continue;
                    if (!CheckQuestCondition(quest.Conditions)) continue;

                    var isInQuestDay = quest.DayOfWeekExists(GameRunner.Instance.TimeRoutine.CurrentDay.ToDayOfWeek());

                    var startingTime = new TimeHrMin(quest.StartingHour, quest.StartingMinute);
                    var EndingTime = new TimeHrMin(quest.EndingHour, quest.EndingMinute);
                    var isInQuestTime = GameRunner.Instance.TimeRoutine.currentTime.inBetweenTime(startingTime, EndingTime);

                    var targetEventPoint = EventPoint.GetEventPoint(quest.TargetEventPoint);

                    if (isInQuestDay && isInQuestTime)
                    {
                        foreach (var actor in quest.TargetActorList)
                        {
                            var npcData = GetNpcData(actor);
                            npcData.isAppearedInQuest = true;
                        }

                        if (targetEventPoint != null)
                        {
                            targetEventPoint.gameObject.SetActive(true);
                            targetEventPoint.eventPointType = EventPoint.EventPointType.Quest;
                            targetEventPoint.ReferenceEventCode = quest.Code;

                            //talk to npc
                            var dialogueActors = targetEventPoint.GetComponentsInChildren<DialogueActor>();
                            var eventPointInteractive = targetEventPoint.GetComponent<EventPointInteractive>();
                            if (dialogueActors != null && dialogueActors.Length > 0 && eventPointInteractive != null)
                            {
                                eventPointInteractive.OnInteractive += () =>
                                {
                                    if (_enabledTime > Time.time) return;

                                    var dialogueTemporaryData = new DialogueTemporaryData(quest.Code, DialogType.Quest, targetEventPoint);
                                    //dialogueTemporaryData.quest = quest;
                                    dialogueTemporaryData.rewards = quest.Rewards.Select(reward => (reward.Type, reward.Code, reward.Value)).ToArray();
                                    OnBeginEventPoint.OnNext(targetEventPoint);
                                    ConversationHandle(quest.Dialog, dialogueTemporaryData);
                                };
                            }
                        }

                    }
                    else //outside event time
                    {
                        targetEventPoint?.gameObject.SetActive(false);

                        if (quest.DisableRoutine)
                        {
                            foreach (var actor in quest.TargetActorList)
                            {
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
            var lastestDialogue = _tempCompletedDialogue.FirstOrDefault(dialog => dialog.eventCode == dialogueData.eventCode);
            if (lastestDialogue != null)
            {
                Debug.Log("Repeat quest dialogue");
                _currentDialog = lastestDialogue;
                lastestDialogue.nextDialogueCallback?.Invoke();
                return;
            }

            Debug.Log("ConversationHandle 0");
            //if (_withinDialogue)
            if (_currentDialog != null)
            {
                Debug.Log($"ConversationHandle _nextDialogueCallback");
                _currentDialog.nextDialogueCallback?.Invoke();
            }
            else
            {
                Debug.Log($"ConversationHandle 1 {dialogueData.eventCode}");

                //_lastestQuest = quest;
                _currentDialog = dialogueData;

                if (dialog == null) return;
                ExecuteDialog(dialog);

            }
        }

        void ShowDialog(string dialog, string actor)
        {
            var targetEventPoint = _currentDialog.eventPoint;
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


                //line.LocalizedText.ToLocalizedString(GameRunner.Instance.Core.Settings.Locale);
                Debug.Log($">>test_npc<< >>line<< {line.Actor} {line.LocalizedText.ToLocalizedString(GameRunner.Instance.Core.Settings.Locale)} <color=purple>{line.AnimatorParam} {line.AnimatorParamValue}</color> <color=green>{line.Photo}</color>");

                ShowDialog(line.LocalizedText.ToLocalizedString(GameRunner.Instance.Core.Settings.Locale), line.Actor);


                _currentDialog.nextDialogueCallback = () =>
                {
                    ExecuteSubDialog(subDialog, lineIndex + 1);
                };
                SetAnimator(line);
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
                        _currentDialog.nextDialogueCallback = () =>
                        {
                            ExecuteSubDialog(subDialog, lineIndex - 1);
                        };
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
            var dialog = question.LocalizedText.ToLocalizedString(GameRunner.Instance.Core.Settings.Locale);
            if (!string.IsNullOrEmpty(dialog)) ShowDialog(dialog, question.Actor);

            _currentDialog.nextDialogueCallback = () =>
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

        void SetAnimator(DialogLine line)
        {
            if (string.IsNullOrEmpty(line.AnimatorParam)) return;

            var animationName = line.AnimatorParam.ToLower();

            Debug.Log($"SetAnimator {line.AnimatorParam} {line.AnimatorParamValue} {line.AnimatorDelay}");
            //Debug.Log($"SetAnimator {line.AnimatorDelay == null}");

            //Debug.Log($"Check Animator Asset : {GameRunner.Instance.AssetMap.ContainsKey(animationName)}");
            //Debug.Log($"Check Animator Asset : {GameRunner.Instance.AssetMap[animationName] is RuntimeAnimatorController}");


            RuntimeAnimatorController targetRuntimeAnimator = null;
            if (GameRunner.Instance.AssetMap.ContainsKey(animationName) && GameRunner.Instance.AssetMap[animationName] is RuntimeAnimatorController)
            {
                targetRuntimeAnimator = GameRunner.Instance.AssetMap[animationName] as RuntimeAnimatorController;
            }
            else
                return;




            Animator targetCharacterAnimator = null;
            if (line.Actor == "player")
            {
                targetCharacterAnimator = GameRunner.Instance.Core.Player.GameObject.GetComponent<PlayerController>().Animator;
            }
            else
            {
                var actors = _currentDialog.eventPoint.GetComponentsInChildren<DialogueActor>();
                foreach (var actor in actors)
                {
                    if (actor.actorId.Equals(line.Actor, System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        targetCharacterAnimator = actor.animtor;
                    }
                }
            }

            //character animator cannot be found
            if (targetCharacterAnimator == null)
            {
                Debug.Log("SetAnimator targetCharacterAnimator is Null");

                //if empty line go next line
                if (string.IsNullOrEmpty(line.Original))
                {
                    _enabledTime = Time.time;
                    _currentDialog.nextDialogueCallback?.Invoke();
                }
                return;
            }

            var previousRuntimeAnimatorController = targetCharacterAnimator.runtimeAnimatorController;

            targetCharacterAnimator.runtimeAnimatorController = targetRuntimeAnimator;

            if (line.AnimatorParamValue == "trigger")
            {
                _currentDialog.nextDialogueCallback += () =>
                {
                    targetCharacterAnimator.runtimeAnimatorController = previousRuntimeAnimatorController;
                };
            }

            _enabledTime = Time.time + 0.2f;

            //wait for animator set the new one
            DG.Tweening.DOVirtual.DelayedCall(0.1f, () =>
            {
                var animationLength = targetCharacterAnimator.GetCurrentAnimatorStateInfo(0).length;
                var delayDuration = Mathf.Max(animationLength, 0.1f);
                if (line.AnimatorDelay != null)
                {
                    delayDuration = line.AnimatorDelay.Value;
                }

                Debug.Log($"delay {delayDuration}");
                _enabledTime = Mathf.Max(_enabledTime, Time.time + delayDuration - 0.1f);

                if (string.IsNullOrEmpty(line.Original))
                {
                    DG.Tweening.DOVirtual.DelayedCall(delayDuration, () =>
                    {
                        _enabledTime = Time.time;
                        _currentDialog.nextDialogueCallback?.Invoke();

                    });
                }



            });


        }

        public void CompleteQuest()
        {
            EndDialogue();

            Debug.Log("Complete dialogue");

            if (_currentDialog.dialogType == DialogType.Quest)
            {
                //_currentDialog.eventPoint.gameObject.SetActive(false);
                GameRunner.Instance.Core.Player.AddAchievedQuest(_currentDialog.eventCode);
                _tempCompletedDialogue.Add(_currentDialog);

                if (!GameRunner.Instance.Core.Player.IsQuestComplete(_currentDialog.eventCode)) //check for one time reward
                {
                    GiveRewards();
                }
            }
            if (_currentDialog.dialogType == DialogType.Routine)
            {
                if (!_tempCompletedDialogue.Contains(_currentDialog))
                {
                    _tempCompletedDialogue.Add(_currentDialog);
                    GiveRewards();
                }
            }

            OnCompleteEventPoint.OnNext(_currentDialog.eventPoint);
            _currentDialog = null;

            //UpdateWorld();
        }
        public void CancelQuest()
        {
            EndDialogue();

            OnCancelEventPoint.OnNext(_currentDialog.eventPoint);
            _currentDialog = null;
        }

        void EndDialogue()
        {
            //complete talking
            ChatBalloonManager.HideChatBalloon();

            //restore player control
            GameRunner.Instance.Core.Player.SetMode(PlayerMode.Move);
        }

        void GiveRewards()
        {
            var rewards = _currentDialog.rewards;
            foreach (var reward in rewards)
            {
                if (reward.Type == "Quest Key")
                {
                    if (reward.Code.ToLower().Equals("next"))
                    {
                        DG.Tweening.DOVirtual.DelayedCall(0.3f, () =>
                        {
                            UpdateQuestEvent_RelatedQuestCode(_currentDialog.eventCode);
                        });
                    }
                    else
                    {
                        GameRunner.Instance.Core.Player.AddAchievedQuest(reward.Code);
                    }
                }
                else if (reward.Type == "Item")
                {
                    var masterItems = GameRunner.Instance.Core.MasterData.ItemsMasterData.Items.ToList();
                    var item = masterItems.FirstOrDefault(i => i.Code == reward.Code);
                    var itemQty = reward.Value;

                    GameRunner.Instance.Core.Player.Bag.AddBagItem(item, itemQty);
                    GameRunner.Instance.Core.Player.ItemObtainPopUp.ShowPopUp(item, itemQty);
                }
                else if (reward.Type == "Money")
                {
                    GameRunner.Instance.Core.Player.AddPocketMoney(reward.Value);
                }
                else if (reward.Type == "Stamina")
                {
                    GameRunner.Instance.Core.Player.Stamina.AddStamina(reward.Value);
                }
                else if (reward.Type == "Site")
                {
                    GameRunner.Instance.Core.Player.SetMode(PlayerMode.Stop);
                    GameRunner.Instance.Core.GameScene.CloseScene();
                    GameRunner.Instance.Core.GameScene.OnChangeSceneFinish.Subscribe(_ =>
                    {
                        SiteRunner.Instance.Core.Site.SetSite(reward.Code, true, reward.Value);
                    });
                }
            }
        }



        void UpdateNpcRoutine()
        {
            var player = GameRunner.Instance.Core.Player;
            var routines = GameRunner.Instance.Core.MasterData.NpcMasterData.RoutinesMasterData.Routines;
            foreach (var routine in routines)
            {
                Debug.Log($"Check routine : {routine.Code}");

                if (!CheckRoutineCondition(routine.Conditions))
                {
                    Debug.Log($"CheckRoutineCondition : false");
                    continue;
                }

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
                Debug.Log($"CheckRoutineEnable : {routineEnable}");
                if (!routineEnable) continue;

                //Debug.Log($"routine is enable");

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
                    targetEventPoint.eventPointType = EventPoint.EventPointType.Routine;
                    targetEventPoint.ReferenceEventCode = routine.Code;

                    //talk to npc
                    var dialogueActors = targetEventPoint.GetComponentsInChildren<DialogueActor>();
                    var eventPointInteractive = targetEventPoint.GetComponent<EventPointInteractive>();
                    if (dialogueActors != null && dialogueActors.Length > 0 && eventPointInteractive != null)
                    {
                        Debug.Log($"routine {routine.Code} ready for action");
                        eventPointInteractive.OnInteractive += () =>
                        {
                            if (_enabledTime > Time.time) return;

                            var dialogueTemporaryData = new DialogueTemporaryData(routine.Code, DialogType.Routine, targetEventPoint);
                            //dialogueTemporaryData.quest = routine;
                            dialogueTemporaryData.rewards = routine.Rewards.Select(reward => (reward.Type, reward.Code, reward.Value)).ToArray();
                            OnBeginEventPoint.OnNext(targetEventPoint);
                            ConversationHandle(routine.Dialog, dialogueTemporaryData);

                        };
                    }

                }

            }

        }

        void SetUpCaptureQuest()
        {
            //GameRunner.Instance.Core.Player.Capture.OnCaptureBegin.Subscribe(_ =>
            //{
            //    var eventPoint = GameRunner.Instance.Core.PlayerMenu.GameObject.GetComponentInParent<EventPoint>();
            //    if (eventPoint == null) return;
            //    if (eventPoint.eventPointType != EventPoint.EventPointType.Quest) return;

            //    Debug.Log("PlayerCapture begin");

            //    if (!string.IsNullOrEmpty(eventPoint.ReferenceEventCode))
            //    {
            //        Debug.Log($"PlayerCapture AddAchievedQuest {eventPoint.ReferenceEventCode}");
            //        GameRunner.Instance.Core.Player.AddAchievedQuest(eventPoint.ReferenceEventCode);
            //    }
            //});

            GameRunner.Instance.Core.Player.Capture.OnCaptureFinish.Subscribe(_ =>
            {
                var eventPoint = GameRunner.Instance.Core.PlayerMenu.GameObject.GetComponentInParent<EventPoint>();
                if (eventPoint == null) return;
                if (eventPoint.eventPointType != EventPoint.EventPointType.Quest) return;

                Debug.Log("PlayerCapture Finish");

                if (!string.IsNullOrEmpty(eventPoint.ReferenceEventCode))
                {
                    Debug.Log($"PlayerCapture AddAchievedQuest {eventPoint.ReferenceEventCode}");
                    GameRunner.Instance.Core.Player.AddAchievedQuest(eventPoint.ReferenceEventCode);

                    var allQuestGroups = GameRunner.Instance.Core.MasterData.NpcMasterData.QuestGroupsMasterData.QuestGroups;
                    foreach (var questGroup in allQuestGroups)
                    {
                        foreach (var quest in questGroup.Quests)
                        {
                            if (quest.Code.Equals(eventPoint.ReferenceEventCode, System.StringComparison.InvariantCultureIgnoreCase))
                            {
                                Debug.Log("PlayerCapture GiveRewards");
                                var dialogueTemporaryData = new DialogueTemporaryData(quest.Code, DialogType.Quest, eventPoint);
                                dialogueTemporaryData.rewards = quest.Rewards.Select(reward => (reward.Type, reward.Code, reward.Value)).ToArray();
                                GiveRewards();
                            }
                        }
                    }
                }
            });



        }

        //update quests that required questCode as condition 
        //used for immediately update quest on the same site
        public void UpdateQuestEvent_RelatedQuestCode(string questCode)
        {
            _currentDialog.eventPoint.gameObject.SetActive(false);

            Debug.Log($"UpdateQuestEvent_RelatedQuestCode {questCode}");
            var player = GameRunner.Instance.Core.Player;
            var allQuestGroups = GameRunner.Instance.Core.MasterData.NpcMasterData.QuestGroupsMasterData.QuestGroups;
            foreach (var questGroup in allQuestGroups)
            {
                if (player.IsQuestComplete(questGroup.Code)) continue;
                if (!CheckQuestGroupCondition(questGroup.Conditions)) continue;

                foreach (var quest in questGroup.Quests)
                {
                    Debug.Log($"UpdateQuestEvent_RelatedQuestCode 1");
                    bool checkThisQuest = false;
                    foreach (var condition in quest.Conditions)
                    {
                        if (condition.Type == "Quest Key" || condition.Type == "Quest Group Key")
                        {
                            if (condition.Code.Equals(questCode, System.StringComparison.InvariantCultureIgnoreCase))
                            {
                                checkThisQuest = true;
                            }
                        }
                    }
                    if (!checkThisQuest) continue; //only check quests that required this questCode as condition
                    Debug.Log($"UpdateQuestEvent_RelatedQuestCode 2");
                    if (player.IsQuestComplete(quest.Code)) continue;
                    Debug.Log($"UpdateQuestEvent_RelatedQuestCode 3");
                    if (!CheckQuestCondition(quest.Conditions)) continue;
                    Debug.Log($"UpdateQuestEvent_RelatedQuestCode 4");
                    var isInQuestDay = quest.DayOfWeekExists(GameRunner.Instance.TimeRoutine.CurrentDay.ToDayOfWeek());

                    var startingTime = new TimeHrMin(quest.StartingHour, quest.StartingMinute);
                    var EndingTime = new TimeHrMin(quest.EndingHour, quest.EndingMinute);
                    var isInQuestTime = GameRunner.Instance.TimeRoutine.currentTime.inBetweenTime(startingTime, EndingTime);

                    var targetEventPoint = EventPoint.GetEventPoint(quest.TargetEventPoint);

                    if (isInQuestDay && isInQuestTime)
                    {
                        Debug.Log($"UpdateQuestEvent_RelatedQuestCode 5");
                        foreach (var actor in quest.TargetActorList)
                        {
                            var npcData = GetNpcData(actor);
                            npcData.isAppearedInQuest = true;
                        }

                        if (targetEventPoint != null)
                        {
                            targetEventPoint.gameObject.SetActive(true);
                            targetEventPoint.eventPointType = EventPoint.EventPointType.Quest;
                            targetEventPoint.ReferenceEventCode = quest.Code;

                            //talk to npc
                            var dialogueActors = targetEventPoint.GetComponentsInChildren<DialogueActor>();
                            var eventPointInteractive = targetEventPoint.GetComponent<EventPointInteractive>();
                            if (dialogueActors != null && dialogueActors.Length > 0 && eventPointInteractive != null)
                            {
                                eventPointInteractive.OnInteractive += () =>
                                {
                                    if (_enabledTime > Time.time) return;

                                    var dialogueTemporaryData = new DialogueTemporaryData(quest.Code, DialogType.Quest, targetEventPoint);
                                    //dialogueTemporaryData.quest = quest;
                                    dialogueTemporaryData.rewards = quest.Rewards.Select(reward => (reward.Type, reward.Code, reward.Value)).ToArray();
                                    OnBeginEventPoint.OnNext(targetEventPoint);
                                    ConversationHandle(quest.Dialog, dialogueTemporaryData);
                                };
                            }
                        }

                    }
                    else //outside event time
                    {
                        //targetEventPoint?.gameObject.SetActive(false);

                        if (quest.DisableRoutine)
                        {
                            foreach (var actor in quest.TargetActorList)
                            {
                                var npcData = GetNpcData(actor);
                                npcData.isAppearedInQuest = true;
                            }
                        }

                    }

                }
            }
        }

    }
}
