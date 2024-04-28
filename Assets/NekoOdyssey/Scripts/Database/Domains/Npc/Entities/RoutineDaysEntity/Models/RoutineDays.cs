using System;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.WeekDayEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.RoutineDaysEntity.Models
{
    [Serializable]
    public class RoutineDays : EntityBase<int>, IAggregateRoot
    {
        [NotNull] [Indexed] public string Name { get; set; }

        [NotNull]
        [Indexed]
        [ForeignKey(typeof(WeekDay))]
        public int WeekDayId { get; set; }

        public string WeekDayName { get; set; }
        
        [Ignore] public WeekDay WeekDay { get; set; }
    }
}