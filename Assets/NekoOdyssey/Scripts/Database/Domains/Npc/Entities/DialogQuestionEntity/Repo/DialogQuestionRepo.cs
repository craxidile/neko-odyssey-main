using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogQuestionEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogQuestionEntity.Repo
{
    public class DialogQuestionRepo : Repository<DialogQuestion, int, SQLiteConnection>
    {
        public DialogQuestionRepo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public ICollection<DialogQuestion> List()
        {
            return _dbContext.Context.Table<DialogQuestion>()
                .OrderBy(dq => dq.Id)
                .ToList();
        }

        public DialogQuestion FindById(int id)
        {
            return _dbContext.Context.Table<DialogQuestion>()
                .FirstOrDefault(dq => dq.Id == id);
        }

        public DialogQuestion FindByDialogId(int dialogId)
        {
            return _dbContext.Context.Table<DialogQuestion>()
                .FirstOrDefault(dq => dq.DialogId == dialogId);
        }

        public DialogQuestion FindBySubDialogId(int subDialogId)
        {
            return _dbContext.Context.Table<DialogQuestion>()
                .FirstOrDefault(dq => dq.SubDialogId == subDialogId);
        }

        public DialogQuestion FindByAnswerId(int answerId)
        {
            return _dbContext.Context.Table<DialogQuestion>()
                .FirstOrDefault(dq => dq.AnswerId == answerId);
        }

        public DialogQuestion FindByOptionId(int optionId)
        {
            return _dbContext.Context.Table<DialogQuestion>()
                .FirstOrDefault(dq => dq.OptionId == optionId);
        }
    }
}