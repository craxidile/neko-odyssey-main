using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogLineEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogLineEntity.Repo
{
    public class DialogLineRepo : Repository<DialogLine, int, SQLiteConnection>
    {
        public DialogLineRepo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public ICollection<DialogLine> List()
        {
            return _dbContext.Context.Table<DialogLine>()
                .OrderBy(dl => dl.Id)
                .ToList();
        }

        public DialogLine FindById(int id)
        {
            return _dbContext.Context.Table<DialogLine>()
                .FirstOrDefault(dl => dl.Id == id);
        }

        public ICollection<DialogLine> ListBySubDialogId(int subDialogId)
        {
            return _dbContext.Context.Table<DialogLine>()
                .Where(dl => dl.SubDialogId == subDialogId)
                .OrderBy(dl => dl.Id)
                .ToList();
        }
    }
}