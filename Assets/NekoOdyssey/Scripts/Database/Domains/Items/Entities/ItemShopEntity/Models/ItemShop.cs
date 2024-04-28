using System;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemShopEntity.Models
{
    [Serializable]
    public class ItemShop : EntityBase<int>, IAggregateRoot
    {
        [NotNull] [Unique] public string SiteCode { get; set; }
        
        public string Name { get; set; }
    }
}