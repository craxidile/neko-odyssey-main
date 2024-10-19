using NekoOdyssey.Scripts.Database.Domains;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerSiteEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerSiteEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.Sites;
using NekoOdyssey.Scripts.Database.Domains.Sites.Entities.SiteEntity.Repo;
using UniRx;
using UnityEngine;
using SaveV001DbContext = NekoOdyssey.Scripts.Database.Domains.SaveV001.SaveV001DbContext;

namespace NekoOdyssey.Scripts.Site.Core.Site
{
    public class Site
    {
        private const string GameFirstSite = "GamePlayZone3_02";

        private bool _databaseInitialized;

        private static Database.Domains.Sites.Entities.SiteEntity.Models.Site _previousSite;
        private static Database.Domains.Sites.Entities.SiteEntity.Models.Site _currentSite;

        public Database.Domains.Sites.Entities.SiteEntity.Models.Site PreviousSite => _previousSite;
        public Database.Domains.Sites.Entities.SiteEntity.Models.Site CurrentSite => _currentSite;

        public bool Ready { get; private set; } = false;
        public bool GameStarted => IsGameStarted();

        public Subject<Unit> OnReady { get; } = new();
        public Subject<Database.Domains.Sites.Entities.SiteEntity.Models.Site> OnChangeSite { get; } = new();

        public void Bind()
        {
            InitializeDatabase();
            InitializeSite();
            // Ready = true;
            // OnReady.OnNext(default);
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
            using (new SitesDbContext(new() { CopyMode = DbCopyMode.ForceCopy, ReadOnly = false })) ;
            _databaseInitialized = true;
        }

        private void InitializeSite()
        {
            if (CurrentSite != null) return;
            SetSite("NekoInside01", false);
            // SetSite("QuestPhase0Scene01B", false);
            // SetSite("Intro", false);
            // SetSite("GamePlayZone4_01", false);
            // SetSite("GamePlayZone4_02", false);
            // SetSite("GamePlayZone6_01", false);
            // SetSite("GamePlayZone6_06", false);
            // SetSite("GamePlayZone3_01", false);
            // SetSite("GamePlayZone3_02", false);
            // SetSite("P0S02", false);
            // SetSite("NekoInside28BedroomFinal", false);
            // SetSite("StartTitle", false);
            // SetSite("MiniGameFishing", false);
            // SetSite("DemoTitle", false);
        }

        public void SetReady()
        {
            Ready = true;
            OnReady.OnNext(default);
        }

        private bool IsGameStarted()
        {
            var firstSiteFound = false;
            using var dbContext = new SaveV001DbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true });
            var playerSiteRepo = new PlayerSiteV001Repo(dbContext);
            firstSiteFound = playerSiteRepo.FindBySiteCode(GameFirstSite) != null;
            Debug.Log($">>main_menu<< game_started {firstSiteFound}");
            return firstSiteFound;
        }

        public void MoveToLastVisitedSite()
        {
            Debug.Log($">>main_menu<< last_visited_site db_check");
            PlayerSiteV001 lastVisitedSite = null;
            using (var dbContext = new SaveV001DbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var playerSiteRepo = new PlayerSiteV001Repo(dbContext);
                lastVisitedSite = playerSiteRepo.FindLastVisited();
            }
            Debug.Log($">>main_menu<< last_visited_site check {lastVisitedSite != null}");

            if (lastVisitedSite == null) return;
            Debug.Log($">>main_menu<< last_visited_site {lastVisitedSite.SiteCode}");
            SetSite(lastVisitedSite.SiteCode);
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

        public void SetSite(string siteName, bool reload = true, int? siteValue = null)
        {
            Debug.Log(
                $"<color=purple>>>set_site<< {siteName} {reload}</color> {(new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name}");

            if (siteValue != null && MiniGameRunner.Instance != null)
            {
                MiniGameRunner.Instance.Core.Site.SetPreviousSite(
                    siteValue.Value,
                    GameRunner.Instance.Core.Player.Position
                );
            }

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