using UniRx;

namespace NekoOdyssey.Scripts.Site.Core.Site
{
    public class Site
    {
        public static Database.Domains.Sites.Entities.SiteEntity.Models.Site CurrentSite { get; private set; }

        public Subject<Database.Domains.Sites.Entities.SiteEntity.Models.Site> OnChangeSite { get; } = new();

        public void Bind()
        {
        }

        public void Start()
        {
        }

        public void Unbind()
        {
        }

        public void SetSite(Database.Domains.Sites.Entities.SiteEntity.Models.Site site)
        {
            CurrentSite = site;
            OnChangeSite.OnNext(site);
        }
        
    }
}