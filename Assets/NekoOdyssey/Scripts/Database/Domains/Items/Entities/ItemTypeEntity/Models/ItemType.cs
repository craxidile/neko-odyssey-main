using System;
using System.Collections.Generic;
using NekoOdyssey.Scripts.Database.Commons.Models;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Sites.Entities.SiteSceneEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemTypeEntity.Models
{
    [Serializable]
    public class ItemType : EntityBase<int>, IAggregateRoot
    {
        [NotNull] [Indexed] public string Code { get; set; }

        [NotNull] public string Name { get; set; }

        public string Description { get; set; }

        [NotNull] public string NormalIcon { get; set; }

        [NotNull] public string ActiveIcon { get; set; }

        [NotNull] public bool IsAll { get; set; }

        [Ignore] public virtual ICollection<Item> Items { get; set; }

        public string NameTh { get; set; }
        public string NameEn { get; set; }
        public string NameZhCn { get; set; }
        public string NameZhTw { get; set; }
        public string NameJa { get; set; }
        
        [Ignore]
        public LocalizedText LocalizedName => new()
        {
            Original = NameEn,
            Th = NameTh,
            En = NameEn,
            ZhCn = NameZhCn,
            ZhTw = NameZhTw,
            Ja = NameJa,
        };

        public string DescriptionTh { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionZhCn { get; set; }
        public string DescriptionZhTw { get; set; }
        public string DescriptionJa { get; set; }
        
        [Ignore]
        public LocalizedText LocalizedDescription => new()
        {
            Original = DescriptionEn,
            Th = DescriptionTh,
            En = DescriptionEn,
            ZhCn = DescriptionZhCn,
            ZhTw = DescriptionZhTw,
            Ja = DescriptionJa,
        };

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