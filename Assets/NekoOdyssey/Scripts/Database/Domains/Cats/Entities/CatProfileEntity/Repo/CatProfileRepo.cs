using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Cats.Entities.CatProfileEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Cats.Entities.CatProfileEntity.Repo
{
    public class CatProfileRepo : Repository<CatProfile, int, SQLiteConnection>
    {
        public CatProfileRepo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public ICollection<CatProfile> List()
        {
            return _dbContext.Context.Table<CatProfile>()
                .OrderBy(cp => cp.Code)
                .ToList();
        }
    }
}