using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Metadata.MetadataEntity.Models
{
    public class Metadata: EntityBase<int>, IAggregateRoot
    {
        [NotNull] [Indexed] public string Key { get; set; }
        
        public string Value { get; set; }
    }
}