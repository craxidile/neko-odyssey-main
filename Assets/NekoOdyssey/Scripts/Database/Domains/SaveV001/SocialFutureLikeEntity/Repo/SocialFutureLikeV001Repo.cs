using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.SocialFutureLikeEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.SaveV001.SocialFutureLikeEntity.Repo
{
    public class SocialFutureLikeV001Repo : Repository<SocialFutureLikeV001, int, SQLiteConnection>
    {
        public SocialFutureLikeV001Repo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public ICollection<SocialFutureLikeV001> List()
        {
            return _dbContext.Context.Table<SocialFutureLikeV001>()
                .Where(sfl => sfl.LikeCount > 0)
                .ToList();
        }

        public SocialFutureLikeV001 FindBySocialPostId(int socialPostId)
        {
            return _dbContext.Context.Table<SocialFutureLikeV001>()
                .FirstOrDefault(sfl => sfl.SocialPostId == socialPostId);
        }
    }
}