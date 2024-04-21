using System.Collections.Generic;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Sites.Entities.SiteSceneEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemTypeEntity.Models
{
    public class ItemType: EntityBase<int>, IAggregateRoot
    {
        [NotNull] [Indexed] public string Code { get; set; }
        
        [NotNull] public string Name { get; set; }
        
        public string Description { get; set; }
        
        [NotNull] public string NormalIcon { get; set; }
        
        [NotNull] public string ActiveIcon { get; set; }
        
        [NotNull] public bool IsAll { get; set; }
        
        [Ignore] public virtual ICollection<Item> Items { get; set; }

        public ItemType()
        {
        }

        public ItemType(string code, string normalIcon, string activeIcon)
        {
            Code = code;
            NormalIcon = normalIcon;
            ActiveIcon = activeIcon;
        }
    }
}