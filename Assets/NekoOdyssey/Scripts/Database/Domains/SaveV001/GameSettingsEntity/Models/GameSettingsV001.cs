using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.SaveV001.GameSettingsEntity.Models
{
    public class GameSettingsV001 : EntityBase<int>, IAggregateRoot
    {
        [NotNull] public string Locale { get; set; }
        
        [NotNull] public float MasterVolume { get; set; }
        
        [NotNull] public float BgmVolume { get; set; }
        
        [NotNull] public float EffectVolume { get; set; }
        
        [NotNull] public int WindowMode { get; set; }
    }
}