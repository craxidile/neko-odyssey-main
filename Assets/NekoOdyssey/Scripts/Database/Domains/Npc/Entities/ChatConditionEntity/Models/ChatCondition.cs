using System;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.ChatEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.ChatConditionEntity.Models
{
    [Serializable]
    public class ChatCondition : EntityBase<int>, IAggregateRoot
    {
        [Indexed]
        [ForeignKey(typeof(Chat))]
        public int ChatId { get; set; }
        
        [NotNull]
        public string Type { get; set; }
        
        public string Code { get; set; }
        
        [NotNull]
        public string Operator { get; set; }
        
        [NotNull]
        public int Value { get; set; }
    }
}