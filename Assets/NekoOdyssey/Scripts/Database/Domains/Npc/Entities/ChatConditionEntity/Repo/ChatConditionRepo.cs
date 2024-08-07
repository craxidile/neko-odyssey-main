using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.ChatConditionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestConditionEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.ChatConditionEntity.Repo
{
    public class ChatConditionRepo : Repository<ChatCondition, int, SQLiteConnection>
    {
        public ChatConditionRepo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public ICollection<ChatCondition> List()
        {
            return _dbContext.Context.Table<ChatCondition>()
                .OrderBy(cc => cc.Id)
                .ToList();
        }

        public ChatCondition FindById(int id)
        {
            return _dbContext.Context.Table<ChatCondition>()
                .FirstOrDefault(qc => qc.Id == id);
        }

        public ICollection<ChatCondition> ListByChatId(int chatId)
        {
            return _dbContext.Context.Table<ChatCondition>()
                .Where(cc => cc.ChatId == chatId)
                .OrderBy(cc => cc.Id)
                .ToList();
        }
    }
}