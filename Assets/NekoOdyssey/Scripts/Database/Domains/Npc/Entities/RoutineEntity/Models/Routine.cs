using System;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.RoutineEntity.Models
{
    [Serializable]
    public class Routine : EntityBase<int>, IAggregateRoot
    {
        [Indexed] public string TargetEventPoint { get; set; }
        
        [Indexed] [ForeignKey(typeof(Dialog))] public int DialogId { get; set; }

        public string DialogCode { get; set; }

        public string RoutineDays { get; set; }

        public int StartingHour { get; set; }
        public int StartingMinute { get; set; }
        public int EndingHour { get; set; }
        public int EndingMinute { get; set; }
    }
}