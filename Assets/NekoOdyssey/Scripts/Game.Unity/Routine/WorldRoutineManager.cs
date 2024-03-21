using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorldRoutineManager : MonoBehaviour
{
    public static List<NpcData> npcDatas = new List<NpcData>();
    public static List<QuestEventDetail> allQuestEvents = new List<QuestEventDetail>();
    public static Dictionary<string, QuestDialogueGroup> allQuestDialogueGroup = new Dictionary<string, QuestDialogueGroup>();
    //public static List<EventDetail> allNpcEvents = new List<EventDetail>();





    [SerializeField] QuestEventManager questEventManager { get; set; }
    [SerializeField] NpcRoutineManager npcRoutineManager { get; set; }

    [SerializeField] QuestDialogueManager questDialogueManager { get; set; }

    public static PlayerChoiceDialogueController playerChoiceDialogueController { get; set; }

    public static WorldRoutineManager Instance { get; private set; } //move this part to central later //or maybe move this whole class

    private void Awake()
    {
        questEventManager = GetComponent<QuestEventManager>();
        npcRoutineManager = GetComponent<NpcRoutineManager>();
        questDialogueManager = GetComponent<QuestDialogueManager>();
        Instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        InitializedSceneEventPoint();
    }

    float _delayTargetTime = 0;
    // Update is called once per frame
    void Update()
    {
        if (Time.time < _delayTargetTime)
        {
            return;
        }
        else
        {
            _delayTargetTime = Time.time + 2;
        }

        foreach (var npcData in npcDatas) //reset story state
        {
            npcData.routineEnable = true;
        }

        //for (int i = allQuestEvents.Count - 1; i >= 0; i--)
        //{
        //    var questEventDetail = allQuestEvents[i];
        //}
        foreach (var questEventDetail in allQuestEvents) //enable story event
        {
            if (questEventManager.ownedQuestKey.Contains(questEventDetail.questId))
            {
                continue;
            }

            bool conditionMatch = questEventManager.CheckQuestCondition(questEventDetail);

            //bool conditionMatch = true;
            //if (questEventDetail.questIdConditions.Count > 0 && questEventDetail.questIdConditions.Any(condition => !questEventManager.ownedQuestKey.Contains(condition)))
            //{
            //    conditionMatch = false;
            //}
            //if (questEventDetail.questIdConditionsExclude.Count > 0 && questEventDetail.questIdConditionsExclude.Any(condition => questEventManager.ownedQuestKey.Contains(condition)))
            //{
            //    conditionMatch = false;
            //}

            if (conditionMatch)
            {
                if (questEventDetail.IsInEventTime(TimeRoutine.day, TimeRoutine.timeHrMin))
                {
                    questEventDetail.targetEventPoint?.gameObject.SetActive(true);

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
                    var dialogueActors = questEventDetail.targetEventPoint?.GetComponentsInChildren<DialogueActor>();
                    var eventPointInteractive = questEventDetail.targetEventPoint?.GetComponent<EventPointInteractive>();
                    if (dialogueActors.Length > 0 && eventPointInteractive != null)
                    {
                        eventPointInteractive.OnInteractive = () =>
                        {
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
                                }
                                else
                                {
                                    //#else1
                                    var targetActor = dialogueActors.FirstOrDefault(actor => actor.actorId == dialogueMessage.actor);
                                    if (targetActor != null)
                                    {
                                        ChatBalloonManager.instance.ShowChatBalloon(targetActor.transform, dialogueMessage.message);
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
                                if (!dialogueGroup.isCanceled)
                                {
                                    Debug.Log("Complete dialogue");

                                    questEventManager.ownedQuestKey.Add(questEventDetail.questId);

                                    questEventDetail.targetEventPoint?.gameObject.SetActive(false);
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

                }
                else
                {
                    questEventDetail.targetEventPoint?.gameObject.SetActive(false);
                }

                //break;
            }
            //else
            //{
            //    questEventDetail.targetEventPoint?.gameObject.SetActive(false);
            //}
        }

        foreach (var npcData in npcDatas) //do routine for other npcs
        {
            if (npcData.routineEnable)
            {
                npcData.UpdateRoutine();

            }
        }
    }


    void InitializedSceneEventPoint()
    {
        questEventManager.InitializedQuestEvent();
        npcRoutineManager.InitializedRoutine();
        questDialogueManager.InitializedQuestDialogue();
    }
}
