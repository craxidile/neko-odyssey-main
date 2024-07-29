using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.ChatGroupEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.ChatGroupEntity.Repo
{
    public class ChatGroupRepo : Repository<ChatGroup, int, SQLiteConnection>
    {
        public ChatGroupRepo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public ICollection<ChatGroup> List()
        {
            return _dbContext.Context.Table<ChatGroup>()
                .OrderBy(cg => cg.Id)
                .ToList();
        }

        public ChatGroup FindById(int id)
        {
            return _dbContext.Context.Table<ChatGroup>()
                .FirstOrDefault(cg => cg.Id == id);
        }
    }
}