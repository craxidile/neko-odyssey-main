using System.Collections.Generic;
using NekoOdyssey.Scripts.Database.Domains;
using NekoOdyssey.Scripts.Database.Domains.Npc;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogAnswerEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogAnswerEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogConditionCaseEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogConditionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogConditionEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogConditionOptionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogConditionOptionEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogLineEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogQuestionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogQuestionEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.SubDialogEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.SubDialogEntity.Repo;
using UniRx;

namespace NekoOdyssey.Scripts.Game.Core.MasterData.Npc.Dialogs
{
    public class DialogsMasterData
    {
        public bool Ready { get; private set; }
        public ICollection<Dialog> Dialogs { get; private set; }

        public Subject<Unit> OnReady { get; } = new();

        public void Bind()
        {
        }

        public void Start()
        {
            ListAll();
            Ready = true;
            OnReady.OnNext(default);
        }

        public void Unbind()
        {
        }

        private void ListAll()
        {
            using (var npcDbContext = new NpcDbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var dialogRepo = new DialogRepo(npcDbContext);
                var subDialogRepo = new SubDialogRepo(npcDbContext);
                var questionRepo = new DialogQuestionRepo(npcDbContext);
                var conditionRepo = new DialogConditionRepo(npcDbContext);

                Dialogs = dialogRepo.List();
                foreach (var dialog in Dialogs)
                {
                    var subDialog = subDialogRepo.FindByDialogId(dialog.Id);
                    if (subDialog != null)
                    {
                        dialog.SubDialog = subDialog;
                        TraverseSubDialog(npcDbContext, subDialog);
                    }

                    var question = questionRepo.FindByDialogId(dialog.Id);
                    if (question != null)
                    {
                        dialog.Question = question;
                        TraverseQuestion(npcDbContext, question);
                    }

                    var condition = conditionRepo.FindByDialogId(dialog.Id);
                    if (condition != null)
                    {
                        dialog.Condition = condition;
                        TraverseCondition(npcDbContext, condition);
                    }
                }
            }
        }

        private static void TraverseSubDialog(NpcDbContext npcDbContext, SubDialog subDialog)
        {
            var lineRepo = new DialogLineRepo(npcDbContext);
            var questionRepo = new DialogQuestionRepo(npcDbContext);
            var conditionRepo = new DialogConditionRepo(npcDbContext);
            subDialog.Lines = lineRepo.ListBySubDialogId(subDialog.Id);

            var question = questionRepo.FindBySubDialogId(subDialog.Id);
            if (question != null)
            {
                subDialog.Question = question;
                TraverseQuestion(npcDbContext, question);
            }

            var condition = conditionRepo.FindBySubDialogId(subDialog.Id);
            if (condition != null)
            {
                subDialog.Condition = condition;
                TraverseCondition(npcDbContext, condition);
            }
        }

        private static void TraverseQuestion(NpcDbContext npcDbContext, DialogQuestion question)
        {
            var answerRepo = new DialogAnswerRepo(npcDbContext);

            question.Answers = answerRepo.ListByQuestionId(question.Id);
            foreach (var answer in question.Answers)
            {
                TraverseAnswer(npcDbContext, answer);
            }
        }

        private static void TraverseAnswer(NpcDbContext npcDbContext, DialogAnswer answer)
        {
            var subDialogRepo = new SubDialogRepo(npcDbContext);
            var questionRepo = new DialogQuestionRepo(npcDbContext);
            var conditionRepo = new DialogConditionRepo(npcDbContext);

            var subDialog = subDialogRepo.FindByAnswerId(answer.Id);
            if (subDialog != null)
            {
                answer.SubDialog = subDialog;
                TraverseSubDialog(npcDbContext, subDialog);
            }

            var question = questionRepo.FindByAnswerId(answer.Id);
            if (question != null)
            {
                answer.Question = question;
                TraverseQuestion(npcDbContext, question);
            }

            var condition = conditionRepo.FindByAnswerId(answer.Id);
            if (condition != null)
            {
                answer.Condition = condition;
                TraverseCondition(npcDbContext, condition);
            }
        }

        private static void TraverseCondition(NpcDbContext npcDbContext, DialogCondition condition)
        {
            var conditionCaseRepo = new DialogConditionCaseRepo(npcDbContext);
            var optionRepo = new DialogConditionOptionRepo(npcDbContext);

            condition.ConditionCases = conditionCaseRepo.ListByConditionId(condition.Id);
            condition.Options = optionRepo.ListByConditionId(condition.Id);

            foreach (var option in condition.Options)
            {
                TraverseOption(npcDbContext, option);
            }
        }

        private static void TraverseOption(NpcDbContext npcDbContext, DialogConditionOption option)
        {
            var subDialogRepo = new SubDialogRepo(npcDbContext);
            var questionRepo = new DialogQuestionRepo(npcDbContext);
            var conditionRepo = new DialogConditionRepo(npcDbContext);

            var subDialog = subDialogRepo.FindByOptionId(option.Id);
            if (subDialog != null)
            {
                option.SubDialog = subDialog;
                TraverseSubDialog(npcDbContext, subDialog);
            }

            var question = questionRepo.FindByOptionId(option.Id);
            if (question != null)
            {
                option.Question = question;
                TraverseQuestion(npcDbContext, question);
            }

            var condition = conditionRepo.FindByOptionId(option.Id);
            if (condition != null)
            {
                option.Condition = condition;
                TraverseCondition(npcDbContext, condition);
            }
        }
    }
}