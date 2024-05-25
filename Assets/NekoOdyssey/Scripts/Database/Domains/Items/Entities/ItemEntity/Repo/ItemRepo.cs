using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemTypeEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemTypeEntity.Repo;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SpatiumInteractive.Libraries.Unity.GRU.Extensions;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemEntity.Repo
{
    public class ItemRepo : Repository<ItemType, int, SQLiteConnection>
    {
        private ItemTypeRepo _itemTypeRepo;
        
        public ItemRepo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
            _itemTypeRepo = new ItemTypeRepo(dbContext);
        }

        public ICollection<Item> List()
        {
            var items = _dbContext.Context.Table<Item>()
                .OrderBy(i => i.Id)
                .ToList();
            foreach (var item in items)
            {
                item.Type = _itemTypeRepo.FindById(item.ItemTypeId);
            }

            return items;
        }
    }
}