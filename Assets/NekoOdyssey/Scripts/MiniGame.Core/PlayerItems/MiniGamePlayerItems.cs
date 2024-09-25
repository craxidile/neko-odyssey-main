using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains;
using NekoOdyssey.Scripts.Database.Domains.Items;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemTypeEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemTypeEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.SaveV001;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.BagItemEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.BagItemEntity.Repo;
// using UnityEditor.ShaderGraph.Internal;

namespace NekoOdyssey.Scripts.MiniGame.Core.PlayerItems
{
    public class MiniGamePlayerItems
    {
        public ICollection<ItemType> ItemTypes { get; private set; }
        public ICollection<Item> Items { get; private set; }
        
        public void Bind()
        {
            InitializeDatabase();
        }

        public void Start()
        {
            ListAllItems();
        }

        public void Unbind()
        {
        }

        private void InitializeDatabase()
        {
            using (new ItemsDbContext(new() { CopyMode = DbCopyMode.ForceCopy, ReadOnly = false })) ;
            using (new SaveV001DbContext(new() { CopyMode = DbCopyMode.CopyIfNotExists, ReadOnly = false })) ;
        }

        private void ListAllItems()
        {
            using (var itemDbContext = new ItemsDbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var itemTypeRepo = new ItemTypeRepo(itemDbContext);
                ItemTypes = itemTypeRepo.List();
                var itemRepo = new ItemRepo(itemDbContext);
                Items = itemRepo.List();
            }
        }

        public void Consume(string itemCode, int count)
        {
            var item = Items.FirstOrDefault(i => i.Code == itemCode);
            if (item == null || item.SingleUse) return;
            using (var dbContext = new SaveV001DbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = false }))
            {
                var bagItemRepo = new BagItemV001Repo(dbContext);
                var bagItems = bagItemRepo.List();
                for (var i = 0; i < count; i++)
                {
                    if (bagItems.All(bi => bi.ItemCode != itemCode)) return;
                    bagItemRepo.Remove(new BagItemV001(item));
                }
            }
        }

        public void Collect(string itemCode, int count)
        {
            var item = Items.FirstOrDefault(i => i.Code == itemCode);
            if (item == null) return;
            using (var dbContext = new SaveV001DbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = false }))
            {
                var bagItemRepo = new BagItemV001Repo(dbContext);
                for (var i = 0; i < count; i++)
                {
                    bagItemRepo.Add(new BagItemV001(item));
                }
            }
        }
    }
}