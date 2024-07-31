using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogConditionOptionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.SubDialogEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.SubDialogEntity.Repo
{
    public class SubDialogRepo : Repository<SubDialog, int, SQLiteConnection>
    {
        public SubDialogRepo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public SubDialog FindByDialogId(int dialogId)
        {
            return _dbContext.Context.Table<SubDialog>()
                .FirstOrDefault(sd => sd.DialogId == dialogId);
        }

        public SubDialog FindByAnswerId(int answerId)
        {
            return _dbContext.Context.Table<SubDialog>()
                .FirstOrDefault(sd => sd.AnswerId == answerId);
        }

        public SubDialog FindByOptionId(int optionId)
        {
            return _dbContext.Context.Table<SubDialog>()
                .FirstOrDefault(sd => sd.OptionId == optionId);
        }
    }
}