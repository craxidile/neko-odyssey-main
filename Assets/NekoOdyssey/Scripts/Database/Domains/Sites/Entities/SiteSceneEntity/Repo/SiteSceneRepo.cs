using NekoOdyssey.Scripts.Database.Domains.Sites.Entities.SiteSceneEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Sites.Entities.SiteSceneEntity.Repo
{
    public class SiteSceneRepo: Repository<SiteScene, int, SQLiteConnection>
    {
        public SiteSceneRepo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }
    }
}