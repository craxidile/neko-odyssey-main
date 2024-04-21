using System.Collections.Generic;
using NekoOdyssey.Scripts.Database.Domains;
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
    public class ItemsMasterData
    {
        public bool Ready { get; private set; }
        public ICollection<ItemType> ItemTypes { get; private set; }
        public ICollection<Item> Items { get; private set; }

        public Subject<Unit> OnReady { get; } = new();

        public void Bind()
        {
            InitializeDatabase();
        }

        public void Start()
        {
            ListAll();
            LogData();
            Ready = true;
            OnReady.OnNext(default);
        }

        public void Unbind()
        {
        }

        private void InitializeDatabase()
        {
            using (new ItemsDbContext(new() { CopyMode = DbCopyMode.ForceCopy, ReadOnly = false })) ;
        }

        private void ListAll()
        {
            using (var itemDbContext = new ItemsDbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var itemTypeRepo = new ItemTypeRepo(itemDbContext);
                ItemTypes = itemTypeRepo.List();
                var itemRepo = new ItemRepo(itemDbContext);
                Items = itemRepo.List();
            }
        }

        private void LogData()
        {
            Debug.Log(">>item_types<<");
            foreach (var itemType in ItemTypes)
            {
                Debug.Log($">>item_type<< {itemType.Code} {itemType.NormalIcon} {itemType.ActiveIcon}");
            }
            Debug.Log(">>items<<");
            foreach (var item in Items)
            {
                Debug.Log($">>item<< {item.Code} {item.Type.Code}");
            }
        }
    }
}