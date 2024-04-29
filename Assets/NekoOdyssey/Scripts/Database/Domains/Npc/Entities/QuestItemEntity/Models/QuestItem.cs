using System;
using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.RequiredQuestItemEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestItemEntity.Models
{
    [Serializable]
    public class QuestItem : EntityBase<int>, IAggregateRoot
    {
        [NotNull] [Indexed] public string Code { get; set; }
        
        [NotNull]
        [Indexed]
        [ForeignKey(typeof(Quest))]
        public int QuestId { get; set; }

        public string QuestCode { get; set; }
        
        [Indexed]
        [ForeignKey(typeof(Dialog))]
        public int DialogId { get; set; }

        public string DialogCode { get; set; }

        public string TargetEventPoint { get; set; }

        public string RoutineDays { get; set; }

        public bool RoutineEnabled { get; set; }

        public int StartingHour { get; set; }
        public int StartingMinute { get; set; }
        public int EndingHour { get; set; }
        public int EndingMinute { get; set; }

        [Ignore] public virtual ICollection<RequiredQuestItem> RequiredQuests { get; set; }

        [Ignore]
        public ICollection<string> RequiredQuestItemCodes =>
            RequiredQuests.Select(rq => rq.RequiredQuestItemCode).ToList();
    }
}