﻿using System;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemShopEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemTypeEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemEntity.Models
{
    [Serializable]
    public class Item : EntityBase<int>, IAggregateRoot
    {
        [NotNull] [Indexed] public string Code { get; set; }

        [NotNull] public string Name { get; set; }

        public string Description { get; set; }

        [NotNull] public string NormalIcon { get; set; }

        [NotNull] public string ActiveIcon { get; set; }

        [NotNull]
        [Indexed]
        [ForeignKey(typeof(ItemType))]
        public int ItemTypeId { get; set; }

        [Ignore] public ItemType Type { get; set; }

        public string ItemTypeCode { get; set; }

        [Indexed]
        [ForeignKey(typeof(ItemShop))]
        public int ItemShopId { get; set; }

        [NotNull] public bool SingleUse { get; set; }

        [NotNull] public bool Collectible { get; set; }

        public string NameTh { get; set; }
        public string NameEn { get; set; }
        public string NameZhCn { get; set; }
        public string NameZhTw { get; set; }
        public string NameJa { get; set; }

        public string DescriptionTh { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionZhCn { get; set; }
        public string DescriptionZhTw { get; set; }
        public string DescriptionJa { get; set; }

        public Item()
        {
        }

        public Item(string code, string normalIcon, string activeIcon)
        {
            Code = code;
            NormalIcon = normalIcon;
            ActiveIcon = activeIcon;
        }
    }
}