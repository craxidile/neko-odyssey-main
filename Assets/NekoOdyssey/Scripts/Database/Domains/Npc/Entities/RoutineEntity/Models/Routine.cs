using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.RoutineEntity.Models
{
    [Serializable]
    public class Routine : EntityBase<int>, IAggregateRoot
    {
        [NotNull] [Indexed] public string Code { get; set; }

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

        [Indexed] [ForeignKey(typeof(Dialog))] public int? DialogId { get; set; }

        [Ignore]
        public virtual Dialog Dialog { get; set; }
    }
}