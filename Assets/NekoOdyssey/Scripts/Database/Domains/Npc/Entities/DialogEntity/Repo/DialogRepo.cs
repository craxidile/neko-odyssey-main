using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogEntity.Repo
{
    public class DialogRepo : Repository<Dialog, int, SQLiteConnection>
    {
        public DialogRepo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public ICollection<Dialog> List()
        {
            return _dbContext.Context.Table<Dialog>()
                .OrderBy(d => d.Id)
                .ToList();
        }

        public Dialog FindById(int id)
        {
            return _dbContext.Context.Table<Dialog>()
                .FirstOrDefault(c => c.Id == id);
        }

        public Dialog FindByCode(string code)
        {
            return _dbContext.Context.Table<Dialog>()
                .FirstOrDefault(c => c.Code == code);
        }
    }
}