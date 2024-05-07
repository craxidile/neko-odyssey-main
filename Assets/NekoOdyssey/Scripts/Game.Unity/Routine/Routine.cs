using NekoOdyssey.Scripts.Game.Unity.AssetBundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UniRx;

namespace NekoOdyssey.Scripts.Game.Core.Routine
{
    public class Routine : MonoBehaviour
    {
        public List<NpcData> npcDatas = new List<NpcData>();
        public List<QuestEventDetail> allQuestEvents = new List<QuestEventDetail>();
        public Dictionary<string, QuestGroup> allQuestGroups = new Dictionary<string, QuestGroup>();
        public Dictionary<string, QuestDialogueGroup> allQuestDialogueGroup = new Dictionary<string, QuestDialogueGroup>();
        //public static List<EventDetail> allNpcEvents = new List<EventDetail>();


        //public CSVHolderScriptable csvHolder;
        //public WorldRoutineManager WorldRoutineManager;

        public QuestEventDetail _lastestQuestEventDetail;

        public DayNightLightingManager dayNightLightingManager;



        public Subject<EventPoint> OnBeginEventPoint { get; } = new();
        public Subject<EventPoint> OnCompleteEventPoint { get; } = new();
        public Subject<EventPoint> OnCancelEventPoint { get; } = new();


        public Routine()
        {

        }

        public void Bind()
        {
            questEventManager = new QuestEventManager();
            npcRoutineManager = new NpcRoutineManager();
            questDialogueManager = new QuestDialogueManager();
            dayNightLightingManager = new DayNightLightingManager();
        }

        public void Start()
        {
            //AssetBundleUtils.OnReady(() =>
            //{
            //    //var a = GameRunner.Instance.AssetMap["daytime"] as ScriptableObject;
            //    //var CSVHolder = GameRunner.Instance.AssetMap["CSVHolder".ToLower()] as CSVHolderScriptable;
            //    //var currentTime = GameRunner.Instance.AssetMap["CurrentTime".ToLower()] as TimeScriptable;
            //    //Debug.Log($"test load scriptable asset bundle {currentTime.currentTimeText}");


            //    //csvHolder = CSVHolder;
            //    //TimeRoutine.timeScriptable = currentTime;

            //    //InitializedSceneEventPoint();
            //    //questEventManager.Start();
            //    //npcRoutineManager.Start();
            //    //questDialogueManager.Start();

            //    //UpdateWorld();
            //    //Invoke(nameof(UpdateWorld), 1f);
            //});

            questEventManager.Start();
            npcRoutineManager.Start();
            questDialogueManager.Start();

            UpdateWorld();

            dayNightLightingManager.Start();
        }

        public void Unbind()
        {
        }


        public QuestEventManager questEventManager { get; set; }
        public NpcRoutineManager npcRoutineManager { get; set; }
        public QuestDialogueManager questDialogueManager { get; set; }

        public static PlayerChoiceDialogueController playerChoiceDialogueController { get; set; }


        //void InitializedSceneEventPoint()
        //{
        //    questEventManager.InitializedQuestEvent();
        //    npcRoutineManager.InitializedRoutine();
        //    questDialogueManager.InitializedQuestDialogue();
        //}


        //private void Awake()
        //{
        //    questEventManager = GetComponent<QuestEventManager>();
        //    npcRoutineManager = GetComponent<NpcRoutineManager>();
        //    questDialogueManager = GetComponent<QuestDialogueManager>();
        //}


        // Start is called before the first frame update
        //void Start()
        //{
        //    InitializedSceneEventPoint();

        //    Invoke(nameof(UpdateWorld), 1f);
        //}

        //float _delayTargetTime = 0;
        // Update is called once per frame
        //void Update()
        //{
        //    //if (Time.time < _delayTargetTime)
        //    //{
        //    //    return;
        //    //}
        //    //else
        //    //{
        //    //    _delayTargetTime = Time.time + 2;
        //    //}

        //    //UpdateWorld();

        //    //foreach (var dnta in dayNightTimeActivators)
        //    //{
        //    //    dnta.CheckActivation();
        //    //}
        //}

        public QuestDialogueGroup GetDialogueGroup(string groupId)
        {
            var dialogueGroup = allQuestDialogueGroup[groupId];
            return dialogueGroup;
        }

        public void UpdateWorld()
        {
            UpdateQuestEvent();

            UpdateNpcRoutine();

        }

        void UpdateQuestEvent()
        {
            foreach (var npcData in npcDatas) //reset story state
            {
                npcData.routineEnable = true;
            }

            foreach (var questGroup in allQuestGroups.Values)
            {
                if (questGroup.questStatus == QuestGroup.QuestStatus.Completed) continue;
                if (questGroup.questStatus == QuestGroup.QuestStatus.Disable)
                {
                    if (!questEventManager.CheckQuestKeyAndItem(questGroup.questIdConditions, questGroup.questIdConditionsExclude)) continue;
                    questGroup.questStatus = QuestGroup.QuestStatus.Avaliable;
                }

                foreach (var questEventDetail in questGroup.questEventDetails)
                {
                    if (questEventManager.ownedQuestKey.Contains(questEventDetail.questId)) continue;//already complete

                    if (!questEventManager.CheckQuestKeyAndItem(questEventDetail)) continue;//check quest key condition

                    if (questEventDetail.IsInEventTime(TimeRoutine.day, TimeRoutine.currentTime))
                    {
                        questEventDetail.GetTargetEventPoint()?.gameObject.SetActive(true);

                        foreach (var relatedCharacter in questEventDetail.relatedCharacters)
                        {
                            var npcData = npcDatas.Find(npc => npc.npcName == relatedCharacter);
                            //Debug.Log($"Get related npc {npcData.npcName} ({relatedCharacter})");
                            if (npcData != null)
                            {
                                //show eventPoint
                                npcData.routineEnable = false;
                                npcData.HideAllRoutine();


                                ////dialogue
                                //var dialogueGroup = allQuestDialogueGroup[questEventDetail.questId];
                                //npcData.dialogueGroup = dialogueGroup;
                            }
                        }


                        //talk to npc
                        var dialogueActors = questEventDetail.GetTargetEventPoint()?.GetComponentsInChildren<DialogueActor>();
                        var eventPointInteractive = questEventDetail.GetTargetEventPoint()?.GetComponent<EventPointInteractive>();
                        if (dialogueActors != null && dialogueActors.Length > 0 && eventPointInteractive != null)
                        {
                            eventPointInteractive.OnInteractive = () =>
                            {
                                _lastestQuestEventDetail = questEventDetail;

                                var dialogueGroup = allQuestDialogueGroup[questEventDetail.questId];
                                var dialogueMessage = dialogueGroup.GetNextDialogue();

                                if (dialogueMessage != null)
                                {
                                    Debug.Log($"npc Talk quest id : {questEventDetail.questId} , text : {dialogueMessage.messageIndex}, {dialogueMessage.message}");

                                    if (dialogueMessage.messageIndex.Equals("choice"))
                                    {
                                        var choiceGroup = dialogueGroup.GetDialogueGroup(dialogueMessage.messageIndex);
                                        playerChoiceDialogueController.ShowChoice(choiceGroup, choice =>
                                        {
                                            var nextDialogue = dialogueGroup.GetNextDialogue(choice);

                                            //same as #else1
                                            if (nextDialogue != null)
                                            {
                                                dialogueMessage = nextDialogue;
                                                var targetActor = dialogueActors.FirstOrDefault(actor => actor.actorId == dialogueMessage.actor);
                                                if (targetActor != null)
                                                {
                                                    ChatBalloonManager.instance.ShowChatBalloon(targetActor.transform, dialogueMessage.message);
                                                    //NekoOdyssey.Scripts.GameRunner.Instance.Core.Player.SetMode(NekoOdyssey.Scripts.Game.Unity.Game.Core.PlayerMode.Conversation);
                                                }
                                                else
                                                {
                                                    Debug.Log($"npc actor {dialogueMessage.actor} cannot found ({dialogueMessage.messageIndex}/{dialogueMessage.message})");
                                                }

                                                if (dialogueMessage.choiceTarget.ToLowerInvariant().Equals("cancel"))
                                                {
                                                    dialogueGroup.isCanceled = true;
                                                }
                                            }

                                            //Debug.Log($"Check index : {dialogueGroup._currentDialogueIndex}");
                                        });

                                        ChatBalloonManager.instance.HideChatBalloon();
                                    }
                                    else
                                    {
                                        //#else1
                                        var targetActor = dialogueActors.FirstOrDefault(actor => actor.actorId == dialogueMessage.actor);
                                        if (targetActor != null)
                                        {
                                            ChatBalloonManager.instance.ShowChatBalloon(targetActor.transform, dialogueMessage.message);
                                            GameRunner.Instance.Core.PlayerMenu.GameObject = targetActor.gameObject;
                                            GameRunner.Instance.Core.Player.SetMode(PlayerMode.QuestConversation);
                                        }
                                        else
                                        {
                                            Debug.Log($"npc actor {dialogueMessage.actor} cannot found ({dialogueMessage.messageIndex}/{dialogueMessage.message})");
                                        }
                                    }

                                    if (dialogueMessage.choiceTarget.ToLowerInvariant().Equals("cancel"))
                                    {
                                        dialogueGroup.isCanceled = true;
                                    }

                                }
                                else
                                {
                                    //complete talking
                                    //restore player control
                                    ChatBalloonManager.instance.HideChatBalloon();
                                    GameRunner.Instance.Core.Player.SetMode(PlayerMode.Move);

                                    if (!dialogueGroup.isCanceled)
                                    {
                                        Debug.Log("Complete dialogue");
                                        questEventDetail.GetTargetEventPoint()?.gameObject.SetActive(false);


                                        CompleteQuestStep();
                                        //questEventManager.ownedQuestKey.Add(questEventDetail.questId);
                                        //UpdateWorld();
                                    }
                                    else
                                    {
                                        Debug.Log("Cancel dialogue");
                                        dialogueGroup.isCanceled = false;

                                        CancleQuestStep();
                                    }


                                }


                                if (questEventDetail.GetTargetEventPoint().TryGetComponent(out EventPoint_AutoComplete eventPointAutoComplete))
                                {
                                    DG.Tweening.DOVirtual.DelayedCall(eventPointAutoComplete.CompleteDelay, () =>
                                    {
                                        Debug.Log("Complete dialogue");
                                        questEventDetail.GetTargetEventPoint()?.gameObject.SetActive(false);
                                        CompleteQuestStep();
                                    });
                                }

                                //foreach (var item in dialogueGroup.questDialogues)
                                //{
                                //    Debug.Log($"npc check list : {item.actor} , {item.messageIndex} , {item.message} , {item.choiceTarget}");

                                //}

                                //Debug.Log($"Check index : {dialogueGroup._currentDialogueIndex}");
                            };
                        }

                    }
                    else //outside event time
                    {
                        questEventDetail.GetTargetEventPoint()?.gameObject.SetActive(false);
                        //if (_lastestQuestEventDetail == questEventDetail) _lastestQuestEventDetail = null;

                        foreach (var keyValue in questEventDetail.relatedCharactersRoutineDisable)
                        {
                            if (keyValue.Value == true)
                            {
                                var npcData = npcDatas.Find(npc => npc.npcName == keyValue.Key);
                                npcData.routineEnable = false;
                            }

                        }
                    }

                }

            }
        }


        void AddRoutineDialogue(EventPoint eventPoint, string dialogueGroupId)
        {
            var dialogueActors = eventPoint.GetComponentsInChildren<DialogueActor>();
            var eventPointInteractive = eventPoint.GetComponent<EventPointInteractive>();
            if (dialogueActors.Length == 0 || eventPointInteractive == null) return;

            eventPointInteractive.OnInteractive = () =>
            {
                var dialogueGroup = allQuestDialogueGroup[dialogueGroupId];
                var dialogueMessage = dialogueGroup.GetNextDialogue();

                Debug.Log($"interactive event point");

                if (dialogueMessage != null)
                {
                    Debug.Log($"npc Talk quest id : {dialogueGroupId} , text : {dialogueMessage.messageIndex}, {dialogueMessage.message}");

                    if (dialogueMessage.messageIndex.Equals("choice"))
                    {
                        var choiceGroup = dialogueGroup.GetDialogueGroup(dialogueMessage.messageIndex);
                        playerChoiceDialogueController.ShowChoice(choiceGroup, choice =>
                        {
                            var nextDialogue = dialogueGroup.GetNextDialogue(choice);

                            //same as #else1
                            if (nextDialogue != null)
                            {
                                dialogueMessage = nextDialogue;
                                var targetActor = dialogueActors.FirstOrDefault(actor => actor.actorId == dialogueMessage.actor);
                                if (targetActor != null)
                                {
                                    ChatBalloonManager.instance.ShowChatBalloon(targetActor.transform, dialogueMessage.message);
                                    //NekoOdyssey.Scripts.GameRunner.Instance.Core.Player.SetMode(NekoOdyssey.Scripts.Game.Unity.Game.Core.PlayerMode.Conversation);
                                }
                                else
                                {
                                    Debug.Log($"npc actor {dialogueMessage.actor} cannot found ({dialogueMessage.messageIndex}/{dialogueMessage.message})");
                                }

                                if (dialogueMessage.choiceTarget.ToLowerInvariant().Equals("cancel"))
                                {
                                    dialogueGroup.isCanceled = true;
                                }
                            }

                            //Debug.Log($"Check index : {dialogueGroup._currentDialogueIndex}");
                        });

                        ChatBalloonManager.instance.HideChatBalloon();
                    }
                    else
                    {
                        //#else1
                        var targetActor = dialogueActors.FirstOrDefault(actor => actor.actorId == dialogueMessage.actor);
                        if (targetActor != null)
                        {
                            ChatBalloonManager.instance.ShowChatBalloon(targetActor.transform, dialogueMessage.message);
                            GameRunner.Instance.Core.PlayerMenu.GameObject = targetActor.gameObject;
                            GameRunner.Instance.Core.Player.SetMode(PlayerMode.QuestConversation);
                        }
                        else
                        {
                            Debug.Log($"npc actor {dialogueMessage.actor} cannot found ({dialogueMessage.messageIndex}/{dialogueMessage.message})");
                        }
                    }

                    if (dialogueMessage.choiceTarget.ToLowerInvariant().Equals("cancel"))
                    {
                        dialogueGroup.isCanceled = true;
                    }
                }
                else
                {
                    //complete talking
                    //restore player control
                    ChatBalloonManager.instance.HideChatBalloon();
                    GameRunner.Instance.Core.Player.SetMode(PlayerMode.Move);

                    if (!dialogueGroup.isCanceled)
                    {
                        Debug.Log("Complete dialogue");

                        UpdateWorld();
                    }
                    else
                    {
                        Debug.Log("Cancel dialogue");
                        dialogueGroup.isCanceled = false;
                    }


                }

                //foreach (var item in dialogueGroup.questDialogues)
                //{
                //    Debug.Log($"npc check list : {item.actor} , {item.messageIndex} , {item.message} , {item.choiceTarget}");

                //}

                //Debug.Log($"Check index : {dialogueGroup._currentDialogueIndex}");
            };

        }

        void UpdateNpcRoutine()
        {
            foreach (var npcData in npcDatas) //do routine for other npcs
            {
                if (npcData.routineEnable)
                {
                    var enabledRoutine = npcData.UpdateRoutine();

                    if (enabledRoutine != null)
                    {
                        if (!enabledRoutine.dialogueKey.Equals("-"))
                        {
                            //Debug.Log($"Add dialogue key : {enabledRoutine.dialogueKey}");
                            AddRoutineDialogue(enabledRoutine.GetTargetEventPoint(), enabledRoutine.dialogueKey);
                        }
                        //Debug.Log($"add routine target = {enabledRoutine.targetEventPoint.name} , dialogue = {enabledRoutine.dialogueKey}");

                    }
                }
            }
        }


        public void CompleteQuestStep()
        {
            Debug.Log("Complete dialogue");

            questEventManager.ownedQuestKey.Add(_lastestQuestEventDetail.questId);


            _lastestQuestEventDetail = null;

            UpdateWorld();
        }
        public void CancleQuestStep()
        {
            _lastestQuestEventDetail = null;
        }



    }
}
