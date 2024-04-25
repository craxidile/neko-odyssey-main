using NekoOdyssey.Scripts.Database.Domains;
using NekoOdyssey.Scripts.Database.Domains.Sites;
using NekoOdyssey.Scripts.Database.Domains.Sites.Entities.SiteEntity.Repo;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Site.Core.Site
{
    public class Site
    {
        private bool _databaseInitialized;

        private static Database.Domains.Sites.Entities.SiteEntity.Models.Site _previousSite;
        private static Database.Domains.Sites.Entities.SiteEntity.Models.Site _currentSite;

        public Database.Domains.Sites.Entities.SiteEntity.Models.Site PreviousSite => _previousSite;
        public Database.Domains.Sites.Entities.SiteEntity.Models.Site CurrentSite => _currentSite;

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
            using (new SitesDbContext(new() { CopyMode = DbCopyMode.ForceCopy, ReadOnly = false }));
            _databaseInitialized = true;
        }

        private void InitializeSite()
        {
            if (CurrentSite != null) return;
             //SetSite("Intro", false);
            // SetSite("GamePlayZone4_01", false);
            // SetSite("GamePlayZone4_02", false);
            // SetSite("GamePlayZone4_03", false);
            // SetSite("GamePlayZone5_01", false);
            // SetSite("GamePlayZone6_01", false);
             SetSite("GamePlayZone3_01", false);
            //SetSite("GamePlayZone9_02", false);
        }

        public void MoveToNextSite()
        {
            var currentSite = CurrentSite;
            var nextSite = currentSite.NextSite;
            if (nextSite == null) return;
            var nextSiteName = nextSite.Name;
            SetSite(nextSiteName);
        }

        public void MoveToPreviousSite(Vector3? previousPosition)
        {
            _currentSite = PreviousSite;
            if (previousPosition != null)
            {
                CurrentSite.PlayerX = previousPosition.Value.x;
                CurrentSite.PlayerY = previousPosition.Value.y;
                CurrentSite.PlayerZ = previousPosition.Value.z;
            }

            _previousSite = null;
            OnChangeSite.OnNext(CurrentSite);
        }

        public void SetSite(string siteName, bool reload = true)
        {
            _previousSite = CurrentSite;
            Database.Domains.Sites.Entities.SiteEntity.Models.Site site;
            using (var siteDbContext = new SitesDbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var siteRepo = new SiteRepo(siteDbContext);
                site = siteRepo.FindByName(siteName);
            }

            if (site == null) return;
            _currentSite = site;
            
            if (!reload) return;
            OnChangeSite.OnNext(site);
        }
    }
}