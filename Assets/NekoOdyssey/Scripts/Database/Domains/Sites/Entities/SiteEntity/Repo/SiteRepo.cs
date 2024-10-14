using NekoOdyssey.Scripts.Database.Domains.Sites.Entities.SiteSceneEntity.Repo;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SpatiumInteractive.Libraries.Unity.GRU.Extensions;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Sites.Entities.SiteEntity.Repo
{
    public class SiteRepo: Repository<Models.Site, int, SQLiteConnection>
    {
        private readonly SiteSceneRepo _siteSceneRepo;
        
        public SiteRepo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
            _siteSceneRepo = new SiteSceneRepo(dbContext);
        }

        private void PopulateNextSite(Models.Site site)
        {
            site.Include(site => site.Scenes, _siteSceneRepo);
            if (site.NextSiteId != null) site.Include(site => site.NextSite, this);
        }

        public Models.Site FindByName(string name)
        {
            var site = _dbContext.Context.Table<Models.Site>()
                .Where(site => site.Name == name)
                .FirstOrDefault();
            if (site == null) return null;
            PopulateNextSite(site);
            return site;
        }
    }
}