using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemTypeEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemTypeEntity.Repo
{
    public class ItemTypeRepo : Repository<ItemType, int, SQLiteConnection>
    {
        public ItemTypeRepo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public ItemType FindById(int id)
        {
            return _dbContext.Context.Table<ItemType>()
                .FirstOrDefault(it => it.Id == id);
        }
        
        public ICollection<ItemType> List()
        {
            return _dbContext.Context.Table<ItemType>()
                .OrderBy(it => it.Id)
                .ToList();
        }
    }
}