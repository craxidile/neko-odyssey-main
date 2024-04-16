using NekoOdyssey.Scripts.Database.Domains.Sites;
using UniRx;

namespace NekoOdyssey.Scripts.Site.Core.Site
{
    public class Site
    {
        private bool _databaseInitialized;
        
        public static Database.Domains.Sites.Entities.SiteEntity.Models.Site CurrentSite { get; private set; }

        public Subject<Database.Domains.Sites.Entities.SiteEntity.Models.Site> OnChangeSite { get; } = new();

        public void Bind()
        {
            InitializeDatabase();
        }

        public void Start()
        {
        }

        public void Unbind()
        {
        }

        public void InitializeDatabase()
        {
            if (_databaseInitialized) return;
            using (new SitesDbContext(new() { CopyRequired = true, ReadOnly = true }));
            _databaseInitialized = true;
        }

        public void SetSite(Database.Domains.Sites.Entities.SiteEntity.Models.Site site)
        {
            CurrentSite = site;
            OnChangeSite.OnNext(site);
        }
    }
}