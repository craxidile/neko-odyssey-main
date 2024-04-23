using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.BagItemEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.SaveV001.BagItemEntity.Repo
{
    public class BagItemV001Repo: Repository<BagItemV001, int, SQLiteConnection>
    {
        public BagItemV001Repo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public ICollection<BagItemV001> List()
        {
            return _dbContext.Context.Table<BagItemV001>()
                .OrderBy(i => i.Id)
                .ToList();
        }

        public void Add(BagItemV001 bagItem)
        {
            base.Add(bagItem);
        }

        public void Remove(BagItemV001 bagItem)
        {
            base.Remove(bagItem);
        }
    }
}