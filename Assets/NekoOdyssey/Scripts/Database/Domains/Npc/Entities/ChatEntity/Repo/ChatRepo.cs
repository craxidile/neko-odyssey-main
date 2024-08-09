using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.ChatEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.ChatEntity.Repo
{
    public class ChatRepo : Repository<Chat, int, SQLiteConnection>
    {
        public ChatRepo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public ICollection<Chat> List()
        {
            return _dbContext.Context.Table<Chat>()
                .OrderBy(c => c.Id)
                .ToList();
        }

        public Chat FindById(int id)
        {
            return _dbContext.Context.Table<Chat>()
                .FirstOrDefault(c => c.Id == id);
        }

        public Chat FindByCode(string code)
        {
            return _dbContext.Context.Table<Chat>()
                .FirstOrDefault(c => c.Code == code);
        }

        public ICollection<Chat> ListByChatGroupId(int chatGroupId)
        {
            return _dbContext.Context.Table<Chat>()
                .Where(c => c.ChatGroupId == chatGroupId)
                .OrderBy(c => c.Id)
                .ToList();
        }
    }
}