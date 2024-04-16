using NekoOdyssey.Scripts.Database.Domains;
using NekoOdyssey.Scripts.Database.Domains.Sites;
using NekoOdyssey.Scripts.Database.Domains.Sites.Entities.SiteEntity.Repo;

namespace NekoOdyssey.Scripts.Site.Core.Transition
{
    public class Transition
    {
        private readonly DbContextOptions _dbContextOptions = new() { ReadOnly = false, CopyRequired = false };

        public void Bind()
        {
        }

        public void Start()
        {
        }

        public void Unbind()
        {
        }

        public void AutoTransit()
        {
            var currentSite = Site.Site.CurrentSite;
            var nextSite = currentSite.NextSite;
            if (nextSite == null) return;
            var nextSiteName = nextSite.Name;
            TransitTo(nextSiteName);
        }

        public void TransitTo(string siteName)
        {
            using (var siteDbContext = new SitesDbContext(_dbContextOptions))
            {
                var siteRepo = new SiteRepo(siteDbContext);
                var site = siteRepo.FindByName(siteName);
                if (site == null) return;
                SiteRunner.Instance.Core.Site.SetSite(site);
            }
        }

        public void ChangeSite()
        {
            var currentSite = Site.Site.CurrentSite;
            using (var siteDbContext = new SitesDbContext(_dbContextOptions))
            {
                var siteRepo = new SiteRepo(siteDbContext);
                var site = siteRepo.FindByName(currentSite == null ? $"Prologue" : currentSite.Name);
                SiteRunner.Instance.Core.Site.SetSite(site);
            }
        }
    }
}