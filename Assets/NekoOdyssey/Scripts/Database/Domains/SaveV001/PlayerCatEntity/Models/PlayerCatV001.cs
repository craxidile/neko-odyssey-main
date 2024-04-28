using System;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerCatEntity.Models
{
    [Serializable]
    public class PlayerCatV001 : EntityBase<int>, IAggregateRoot
    {
        [NotNull] [Unique] public string CatCode { get; set; }
        
        [NotNull] public int DailyFeedCount { get; set; }
        
        [NotNull] public int DailyPlayCount { get; set; }
        
        [NotNull] public float Friendship { get; set; }
    }
}