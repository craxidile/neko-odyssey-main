using System;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Social.Entities.FriendEntity.Models
{
    [Serializable]
    public class Friend : EntityBase<int>, IAggregateRoot
    {
        [NotNull] [Unique] public string Code { get; set; }

        public string ProfilePhoto { get; set; }
        
        public string NameTh { get; set; }
        public string NameEn { get; set; }
        public string NameZhCn { get; set; }
        public string NameZhTw { get; set; }
        public string NameJa { get; set; }
    }
}