using System;
using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Npc.Commons;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogAnswerEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogConditionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogQuestionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.SubDialogEntity.Models;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Core
{
    public class TestDialog
    {
        public void Test()
        {
            IDialogNextEntity next;

            var dialog = new Dialog();
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
            var a = new Animator();
            foreach (var line in subDialog.Lines)
            {
                if (line.AnimatorParamValue == "true")
                {
                    a.SetBool(line.AnimatorParam, true);
                }
                else if (line.AnimatorParamValue == "false")
                {
                    a.SetBool(line.AnimatorParam, false);
                }
                else if (line.AnimatorParamValue == "trigger")
                {
                    a.SetTrigger(line.AnimatorParam);
                }
                
                Debug.Log($">>line<< {line.Actor} {line.TextTh}");
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
            Debug.Log($">>question<< {question.Original}");
            foreach (var answer in question.Answers)
            {
                Debug.Log($">>answer<< {answer.Original}");
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
                    $">>condition<< {conditionCase.Type} {conditionCase.Code} {conditionCase.Operator} {conditionCase.Value}");
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
            Debug.Log($">>end<<");
        }

        private void CancelDialog()
        {
            Debug.Log($">>cancel<<");
        }
    }
}