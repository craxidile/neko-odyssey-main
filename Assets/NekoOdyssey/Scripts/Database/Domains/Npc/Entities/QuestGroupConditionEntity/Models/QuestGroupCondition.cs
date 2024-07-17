using System;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestGroupEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestGroupConditionEntity.Models
{
    [Serializable]
    public class QuestGroupCondition : EntityBase<int>, IAggregateRoot
    {
        [Indexed]
        [ForeignKey(typeof(QuestGroup))]
        public int QuestGroupId { get; set; }
        
        [NotNull]
        public string Type { get; set; }
        
        public string Code { get; set; }
        
        [NotNull]
        public string Operator { get; set; }
        
        [NotNull]
        public int Value { get; set; }
    }
}