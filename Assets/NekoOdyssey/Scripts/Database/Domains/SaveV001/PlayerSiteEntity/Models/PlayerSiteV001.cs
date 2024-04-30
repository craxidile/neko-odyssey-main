using System;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerSiteEntity.Models
{
    [Serializable]
    public class PlayerSiteV001 : EntityBase<int>, IAggregateRoot
    {
        [NotNull] [Unique] public string SiteCode { get; set; }
        
        [NotNull] [Indexed] public DateTime LastVisit { get; set; }
    }
}