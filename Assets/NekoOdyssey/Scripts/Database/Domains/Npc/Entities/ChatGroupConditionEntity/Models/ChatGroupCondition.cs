using System;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.ChatGroupEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.ChatGroupConditionEntity.Models
{
    [Serializable]
    public class ChatGroupCondition : EntityBase<int>, IAggregateRoot
    {
        [Indexed]
        [ForeignKey(typeof(ChatGroup))]
        public int ChatGroupId { get; set; }
        
        [NotNull]
        public string Type { get; set; }
        
        public string Code { get; set; }
        
        [NotNull]
        public string Operator { get; set; }
        
        [NotNull]
        public int Value { get; set; }
    }
}