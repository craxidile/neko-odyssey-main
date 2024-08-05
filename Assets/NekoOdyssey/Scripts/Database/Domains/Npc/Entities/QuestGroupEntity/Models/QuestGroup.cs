using System;
using System.Collections.Generic;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestGroupConditionEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestGroupEntity.Models
{
    [Serializable]
    public class QuestGroup : EntityBase<int>, IAggregateRoot
    {
        [NotNull]
        [Indexed]
        public string Code { get; set; }
        
        public string TitleOriginal { get; set; }
        public string TitleTh { get; set; }
        public string TitleEn { get; set; }
        public string TitleZhCn { get; set; }
        public string TitleZhTw { get; set; }
        public string TitleJa { get; set; }
        
        public string DescriptionOriginal { get; set; }
        public string DescriptionTh { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionZhCn { get; set; }
        public string DescriptionZhTw { get; set; }
        public string DescriptionJa { get; set; }
        
        [Ignore]
        public virtual ICollection<Quest> Quests { get; set; }
        
        [Ignore]
        public virtual ICollection<QuestGroupCondition> Conditions { get; set; }
    }
}