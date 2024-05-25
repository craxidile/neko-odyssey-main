using System.Collections.Generic;
using NekoOdyssey.Scripts.Database.Domains;
using NekoOdyssey.Scripts.Database.Domains.Cats;
using NekoOdyssey.Scripts.Database.Domains.Cats.Entities.CatProfileEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Cats.Entities.CatProfileEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.Items;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemTypeEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemTypeEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.Sites.Entities.SiteEntity.Repo;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace NekoOdyssey.Scripts.Game.Core.MasterData.Items
{
    public class CatsMasterData
    {
        public bool Ready { get; private set; }
        public ICollection<CatProfile> CatProfiles { get; private set; }

        public Subject<Unit> OnReady { get; } = new();

        public void Bind()
        {
            InitializeDatabase();
        }

        public void Start()
        {
            ListAll();
            Ready = true;
            OnReady.OnNext(default);
        }

        public void Unbind()
        {
        }

        private void InitializeDatabase()
        {
            using (new CatsDbContext(new() { CopyMode = DbCopyMode.ForceCopy, ReadOnly = false })) ;
        }

        private void ListAll()
        {
            using (var catsDbContext = new CatsDbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var catProfileRepo = new CatProfileRepo(catsDbContext);
                CatProfiles = catProfileRepo.List();
            }
        }
        
    }
}