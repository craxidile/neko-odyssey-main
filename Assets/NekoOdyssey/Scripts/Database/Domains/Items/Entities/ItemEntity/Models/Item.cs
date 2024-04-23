using System;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemTypeEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemEntity.Models
{
    [Serializable]
    public class Item: EntityBase<int>, IAggregateRoot, ICloneable, IEquatable<Item>
    {
        [Ignore] public string Uuid { get; } = Guid.NewGuid().ToString();
        [Ignore] public int BagItemId { get; set; }
        
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


        public Item()
        {
        }

        public Item(string code, string normalIcon, string activeIcon)
        {
            Code = code;
            NormalIcon = normalIcon;
            ActiveIcon = activeIcon;
        }

        public object Clone()
        {
            return new Item()
            {
                Code = Code,
                Name = Name,
                Description = Description,
                NormalIcon = NormalIcon,
                ActiveIcon = ActiveIcon,
                ItemTypeId = ItemTypeId,
                Type = Type,
                ItemTypeCode = ItemTypeCode,
            };
        }

        public bool Equals(Item other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Uuid == other.Uuid;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Item)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Uuid);
        }
    }
}