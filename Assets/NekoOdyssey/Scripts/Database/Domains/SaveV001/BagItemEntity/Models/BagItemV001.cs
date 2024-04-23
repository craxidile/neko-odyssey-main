using System;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.SaveV001.BagItemEntity.Models
{
    [Serializable]
    public class BagItemV001: EntityBase<int>, IAggregateRoot
    {
        [NotNull] public string ItemCode { get; set; }

        public BagItemV001()
        {
        }

        public BagItemV001(string itemCode)
        {
            ItemCode = itemCode;
        }
    }
}