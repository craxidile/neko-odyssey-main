using System;
using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Npc.Commons;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.ChatGroupEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogAnswerEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogConditionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogQuestionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestGroupEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.SubDialogEntity.Models;
using UniRx;
using UnityEngine;
using DayOfWeek = NekoOdyssey.Scripts.Database.Commons.Models.DayOfWeek;

namespace NekoOdyssey.Scripts.Game.Core
{
    public class TestDialog
    {
        public void Test()
        {
            const string questGroupCode = "Q005";
            const string chatGroupCode = "npc41";
            const string routineCode = "Npc01DrinkTea";
            var questGroupsMasterData = GameRunner.Instance.Core.MasterData.NpcMasterData.QuestGroupsMasterData;
            var chatGroupsMasterData = GameRunner.Instance.Core.MasterData.NpcMasterData.ChatGroupsMasterData;
            var routinesMasterData = GameRunner.Instance.Core.MasterData.NpcMasterData.RoutinesMasterData;
            if (questGroupsMasterData.Ready)
            {
                ExecuteQuestGroup(questGroupsMasterData.QuestGroups.FirstOrDefault(qg => qg.Code == questGroupCode));
                ExecuteChatGroup(chatGroupsMasterData.ChatGroups.FirstOrDefault(cg => cg.Code == chatGroupCode));
                ExecuteRoutine(routinesMasterData.Routines.FirstOrDefault(r => r.Code == routineCode));
            }
            else
            {
                questGroupsMasterData.OnReady
                    .Subscribe(_ =>
                    {
                        ExecuteQuestGroup(
                            questGroupsMasterData.QuestGroups.FirstOrDefault(qg => qg.Code == questGroupCode)
                        );
                        ExecuteChatGroup(
                            chatGroupsMasterData.ChatGroups.FirstOrDefault(cg => cg.Code == chatGroupCode)
                        );
                        ExecuteRoutine(routinesMasterData.Routines.FirstOrDefault(r => r.Code == routineCode));
                    })
                    .AddTo(GameRunner.Instance);
            }
        }

        private void ExecuteQuestGroup(QuestGroup questGroup)
        {
            Debug.Log($">>test_npc<<<color=red>========== Quest Group ==========</color>");
            Debug.Log($">>test_npc<< >>quest_group<< {questGroup.Code}");
            foreach (var condition in questGroup.Conditions)
            {
                Debug.Log(
                    $">>test_npc<< >>quest_group<< condition {condition.Type} {condition.Code} {condition.Operator} {condition.Value}");
            }

            foreach (var quest in questGroup.Quests)
            {
                Debug.Log($">>test_npc<< >>quest<< {quest.Code} {string.Join(',', quest.TargetActorList)} {quest.TargetActorExists("player")} {string.Join(',', quest.ActiveDayOfWeekList)} {quest.DayOfWeekExists(DayOfWeek.Monday)}");
                foreach (var condition in quest.Conditions)
                {
                    Debug.Log(
                        $">>test_npc<< >>quest<< condition {condition.Type} {condition.Code} {condition.Operator} {condition.Value}");
                }

                Debug.Log($">>test_npc<< >>quest<< dialog_exists {quest.Dialog != null}");
                if (quest.Dialog == null) continue;
                ExecuteDialog(quest.Dialog);
            }
        }

        private void ExecuteChatGroup(ChatGroup chatGroup)
        {
            Debug.Log($">>test_npc<<<color=red>========== Chat Group ==========</color>");
            Debug.Log($">>test_npc<< >>chat_group<< {chatGroup.Code}");
            foreach (var condition in chatGroup.Conditions)
            {
                Debug.Log(
                    $">>test_npc<< >>chat_group<< condition {condition.Type} {condition.Code} {condition.Operator} {condition.Value}");
            }

            foreach (var chat in chatGroup.Chats)
            {
                Debug.Log($">>test_npc<< >>chat<< {chat.Code}");
                foreach (var condition in chat.Conditions)
                {
                    Debug.Log(
                        $">>test_npc<< >>quest<< condition {condition.Type} {condition.Code} {condition.Operator} {condition.Value}");
                }

                Debug.Log($">>test_npc<< >>quest<< dialog_exists {chat.Dialog != null}");
                if (chat.Dialog == null) continue;
                ExecuteDialog(chat.Dialog);
            }
        }

        private void ExecuteRoutine(Database.Domains.Npc.Entities.RoutineEntity.Models.Routine routine)
        {
            Debug.Log($">>test_npc<<<color=red>========== Routine ==========</color>");
            Debug.Log($">>test_npc<< >>routine<< {routine.Code} {string.Join(',', routine.TargetActorList)} {routine.TargetActorExists("player")} {string.Join(',', routine.ActiveDayOfWeekList)} {routine.DayOfWeekExists(DayOfWeek.Monday)}");
            if (routine.Dialog == null) return;
            Debug.Log($">>test_npc<< >>routine<< dialog_exists {routine.Dialog != null}");
            ExecuteDialog(routine.Dialog);
        }

        private void ExecuteDialog(Dialog dialog)
        {
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

        private void ExecuteSubDialog(SubDialog subDialog)
        {
            Debug.Log($">>sub_dialog<< {subDialog.Id}");

            // var a = new Animator();
            foreach (var line in subDialog.Lines)
            {
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

                Debug.Log(
                    $">>test_npc<< >>line<< {line.Actor} {line.Original} <color=purple>{line.AnimatorParam} {line.AnimatorParamValue}</color> <color=green>{line.Photo}</color>");
            }

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
                    EndDialog();
                    break;
                case DialogChildFlag.Cancel:
                    CancelDialog();
                    break;
                default:
                    return;
            }
        }

        private void ExecuteQuestion(DialogQuestion question)
        {
            Debug.Log($">>test_npc<< >>question<< {question.Original}");
            foreach (var answer in question.Answers)
            {
                Debug.Log($">>test_npc<< >>answer<< {answer.Original}");
            }

            var selectedAnswer = (question.Answers as List<DialogAnswer>)?[0];

            var childFlag = selectedAnswer?.DialogChildFlag;
            switch (childFlag)
            {
                case DialogChildFlag.HasSubDialog:
                    ExecuteSubDialog(selectedAnswer.SubDialog);
                    break;
                case DialogChildFlag.HasQuestion:
                    ExecuteQuestion(selectedAnswer.Question);
                    break;
                case DialogChildFlag.HasCondition:
                    ExecuteCondition(selectedAnswer.Condition);
                    break;
                case DialogChildFlag.End:
                    EndDialog();
                    break;
                case DialogChildFlag.Cancel:
                    CancelDialog();
                    break;
                default:
                    break;
            }
        }

        private void ExecuteCondition(DialogCondition condition)
        {
            var valid = true;
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
                    EndDialog();
                    break;
                case DialogChildFlag.Cancel:
                    CancelDialog();
                    break;
                default:
                    return;
            }
        }

        private void EndDialog()
        {
            Debug.Log($">>test_npc<< >>end<<");
        }

        private void CancelDialog()
        {
            Debug.Log($">>test_npc<< >>cancel<<");
        }
    }
}