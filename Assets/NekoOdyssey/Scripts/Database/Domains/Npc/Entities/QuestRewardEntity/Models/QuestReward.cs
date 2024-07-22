using System;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestRewardEntity.Models
{
    [Serializable]
    public class QuestReward : EntityBase<int>, IAggregateRoot
    {
        [Indexed]
        [ForeignKey(typeof(Quest))]
        public int QuestId { get; set; }
        
        [NotNull]
        public string Type { get; set; }
        
        public string Code { get; set; }
        
        [NotNull]
        public int Value { get; set; }
    }
}