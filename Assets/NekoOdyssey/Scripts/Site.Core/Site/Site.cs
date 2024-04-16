using NekoOdyssey.Scripts.Database.Domains.Sites;
using NekoOdyssey.Scripts.Database.Domains.Sites.Entities.SiteEntity.Repo;
using UniRx;

namespace NekoOdyssey.Scripts.Site.Core.Site
{
    public class Site
    {
        private bool _databaseInitialized;

        public static Database.Domains.Sites.Entities.SiteEntity.Models.Site CurrentSite { get; private set; }

        public bool Ready { get; private set; }
        
        public Subject<Unit> OnReady { get; } = new();
        public Subject<Database.Domains.Sites.Entities.SiteEntity.Models.Site> OnChangeSite { get; } = new();

        public void Bind()
        {
            InitializeDatabase();
            InitializeSite();
            Ready = true;
            OnReady.OnNext(default);
        }

        public void Start()
        {
        }

        public void Unbind()
        {
        }

        private void InitializeDatabase()
        {
            if (_databaseInitialized) return;
            using (new SitesDbContext(new() { CopyRequired = true, ReadOnly = true })) ;
            _databaseInitialized = true;
        }

        private void InitializeSite()
        {
            if (CurrentSite != null) return;
            SetSite("Prologue");
        }

        public void MoveToNextSite()
        {
            var currentSite = CurrentSite;
            var nextSite = currentSite.NextSite;
            if (nextSite == null) return;
            var nextSiteName = nextSite.Name;
            SetSite(nextSiteName);
        }

        public void SetSite(string siteName)
        {
            Database.Domains.Sites.Entities.SiteEntity.Models.Site site;
            using (var siteDbContext = new SitesDbContext(new() { CopyRequired = false, ReadOnly = true }))
            {
                var siteRepo = new SiteRepo(siteDbContext);
                site = siteRepo.FindByName(siteName);
            }

            if (site == null) return;
            CurrentSite = site;
            OnChangeSite.OnNext(site);
        }
    }
}