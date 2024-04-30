using System;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.SaveV001.BagItemEntity.Models
{
    [Serializable]
    public class BagItemV001 : EntityBase<int>, IAggregateRoot, IEquatable<Item>, ICloneable
    {
        [Ignore] public string Uuid { get; } = Guid.NewGuid().ToString();

        [NotNull] public string ItemCode { get; set; }
        
        [Ignore] public Item Item { get; set; }
        
        public DateTime ReceivedAt { get; set; }

        public BagItemV001()
        {
        }

        public BagItemV001(string itemCode)
        {
            ItemCode = itemCode;
        }

        public BagItemV001(Item item)
        {
            ItemCode = item.Code;
            Item = item;
        }

        public bool Equals(BagItemV001 other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Uuid == other.Uuid;
        }

        public bool Equals(Item other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BagItemV001)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Uuid);
        }

        public object Clone()
        {
            return new BagItemV001()
            {
                ItemCode = ItemCode,
                Item = Item
            };
        }
    }
}