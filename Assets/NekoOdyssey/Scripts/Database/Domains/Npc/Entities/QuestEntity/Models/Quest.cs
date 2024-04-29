using System;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestEntity.Models
{
    [Serializable]
    public class Quest : EntityBase<int>, IAggregateRoot
    {
        [NotNull] [Indexed] public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string NameTh { get; set; }
        public string NameEn { get; set; }
        public string NameZhCn { get; set; }
        public string NameZhTw { get; set; }
        public string NameJa { get; set; }

        public string DescriptionTh { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionZhCn { get; set; }
        public string DescriptionZhTw { get; set; }
        public string DescriptionJa { get; set; }
    }
}