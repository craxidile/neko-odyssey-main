using System;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestConditionEntity.Models
{
    [Serializable]
    public class QuestCondition : EntityBase<int>, IAggregateRoot
    {
        [NotNull]
        [Indexed]
        [ForeignKey(typeof(Quest))]
        public int QuestId { get; set; }

        public string QuestCode { get; set; }
        
        public string QuestItemCode { get; set; }
        
        [NotNull] public bool Inclusive { get; set; }
    }
}