using System;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerPropertiesEntity.Models
{
    [Serializable]
    public class PlayerPropertiesV001 : EntityBase<int>, IAggregateRoot
    {
        [NotNull] public int DayCount { get; set; }
        
        [NotNull] public int CurrentHour { get; set; }
        
        [NotNull] public int CurrentMinute { get; set; }
        
        [NotNull] public int Stamina { get; set; }
        
        [NotNull] public int PocketMoney { get; set; }
        
        [NotNull] public int FollowerCount { get; set; }
        
        [NotNull] public int LikeCount { get; set; }
    }
}