using System;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.RoutineEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.RoutineRewardEntity.Models
{
    [Serializable]
    public class RoutineReward : EntityBase<int>, IAggregateRoot
    {
        [Indexed]
        [ForeignKey(typeof(Routine))]
        public int RoutineId { get; set; }
        
        [NotNull]
        public string Type { get; set; }
        
        public string Code { get; set; }
        
        [NotNull]
        public int Value { get; set; }
    }
}