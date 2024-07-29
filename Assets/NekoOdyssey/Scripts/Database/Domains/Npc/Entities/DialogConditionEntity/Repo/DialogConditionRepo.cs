﻿using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogAnswerEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogConditionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogConditionOptionEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogConditionEntity.Repo
{
    public class DialogConditionRepo : Repository<DialogCondition, int, SQLiteConnection>
    {
        public DialogConditionRepo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public DialogCondition FindByDialogId(int dialogId)
        {
            return _dbContext.Context.Table<DialogCondition>()
                .FirstOrDefault(dc => dc.DialogId == dialogId);
        }

        public DialogCondition FindBySubDialogId(int subDialogId)
        {
            return _dbContext.Context.Table<DialogCondition>()
                .FirstOrDefault(dc => dc.SubDialogId == subDialogId);
        }

        public DialogCondition FindByAnswerId(int answerId)
        {
            return _dbContext.Context.Table<DialogCondition>()
                .FirstOrDefault(dc => dc.AnswerId == answerId);
        }

        public DialogCondition FindByOption(int optionId)
        {
            return _dbContext.Context.Table<DialogCondition>()
                .FirstOrDefault(dc => dc.OptionId == optionId);
        }
    }
}