using System;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.RoutineEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.RoutineConditionEntity.Models
{
    [Serializable]
    public class RoutineCondition : EntityBase<int>, IAggregateRoot
    {
        [NotNull]
        [Indexed]
        [ForeignKey(typeof(Routine))]
        public int RoutineId { get; set; }

        public string RoutineCode { get; set; }

        public string EventKey { get; set; }

        [NotNull] public bool Inclusive { get; set; }
    }
}