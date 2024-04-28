using System;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestItemEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.RequiredQuestItemEntity.Models
{
    [Serializable]
    public class RequiredQuestItem : EntityBase<int>, IAggregateRoot
    {
        [NotNull]
        [Indexed]
        [ForeignKey(typeof(QuestItem))]
        public string QuestItemId { get; set; }

        public string QuestItemCode { get; set; }

        public string RequiredQuestItemCode { get; set; }
    }
}