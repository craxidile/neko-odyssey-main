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

        public DialogQuestion FindByDialogId(int dialogId)
        {
            return _dbContext.Context.Table<DialogQuestion>()
                .FirstOrDefault(dc => dc.DialogId == dialogId);
        }

        public DialogQuestion FindBySubDialogId(int subDialogId)
        {
            return _dbContext.Context.Table<DialogQuestion>()
                .FirstOrDefault(dc => dc.SubDialogId == subDialogId);
        }

        public DialogQuestion FindByAnswerId(int answerId)
        {
            return _dbContext.Context.Table<DialogQuestion>()
                .FirstOrDefault(dc => dc.AnswerId == answerId);
        }

        public DialogQuestion FindByOption(int optionId)
        {
            return _dbContext.Context.Table<DialogQuestion>()
                .FirstOrDefault(dc => dc.OptionId == optionId);
        }
    }
}