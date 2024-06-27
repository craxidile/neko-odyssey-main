using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Ui.Entities.LocalisationEntity.Models
{
    public class Localisation : EntityBase<int>, IAggregateRoot
    {
        [NotNull] [Unique] [Indexed] public string Original { get; set; }

        public string TextTh { get; set; }
        public string TextEn { get; set; }
        public string TextZhCn { get; set; }
        public string TextZhTw { get; set; }
        public string TextJa { get; set; }
    }
}