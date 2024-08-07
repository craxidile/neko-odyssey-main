using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.ChatGroupConditionEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.ChatGroupConditionEntity.Repo
{
    public class ChatGroupConditionRepo : Repository<ChatGroupCondition, int, SQLiteConnection>
    {
        public ChatGroupConditionRepo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public ICollection<ChatGroupCondition> List()
        {
            return _dbContext.Context.Table<ChatGroupCondition>()
                .OrderBy(cgc => cgc.Id)
                .ToList();
        }

        public ChatGroupCondition FindById(int id)
        {
            return _dbContext.Context.Table<ChatGroupCondition>()
                .FirstOrDefault(cgc => cgc.Id == id);
        }

        public ICollection<ChatGroupCondition> ListByChatGroupId(int chatGroupId)
        {
            return _dbContext.Context.Table<ChatGroupCondition>()
                .Where(cgc => cgc.ChatGroupId == chatGroupId)
                .OrderBy(cgc => cgc.Id)
                .ToList();
        }
    }
}