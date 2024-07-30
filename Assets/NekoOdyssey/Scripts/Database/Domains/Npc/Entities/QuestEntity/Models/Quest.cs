using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestConditionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestGroupEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestEntity.Models
{
    [Serializable]
    public class Quest : EntityBase<int>, IAggregateRoot
    {
        [Indexed]
        [ForeignKey(typeof(QuestGroup))]
        public int QuestGroupId { get; set; }

        [NotNull] [Indexed] public string Code { get; set; }

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

        [NotNull] public string TargetEventPoint { get; set; }
        [NotNull] public string TargetActors { get; set; }

        [Ignore]
        public ICollection<string> TargetActorList
        {
            get
            {
                if (string.IsNullOrEmpty(TargetActors)) return Array.Empty<string>();
                var actorsText = Regex.Replace(TargetActors, "^|", "");
                actorsText = Regex.Replace(actorsText, "|$", "");
                return actorsText.Split('|').ToList();
            }
        }

        [NotNull] public string ActiveDaysOfWeek { get; set; }
        [NotNull] public int StartingHour { get; set; }
        [NotNull] public int StartingMinute { get; set; }
        [NotNull] public int EndingHour { get; set; }
        [NotNull] public int EndingMinute { get; set; }

        [Ignore]
        public ICollection<string> ActiveDayOfWeekList
        {
            get
            {
                if (string.IsNullOrEmpty(ActiveDaysOfWeek)) return Array.Empty<string>();
                var daysText = Regex.Replace(ActiveDaysOfWeek, "^|", "");
                daysText = Regex.Replace(daysText, "|$", "");
                return daysText.Split('|').ToList();
            }
        }

        [NotNull] public bool DisableRoutine { get; set; }

        [Indexed] [ForeignKey(typeof(Dialog))] public int? DialogId { get; set; }

        [Ignore] public virtual Dialog Dialog { get; set; }

        [Ignore] public virtual QuestGroup QuestGroup { get; set; }

        [Ignore] public virtual ICollection<QuestCondition> Conditions { get; set; }
    }
}