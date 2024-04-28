using System;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogLineEntity.Models
{
    [Serializable]
    public class DialogLine : EntityBase<int>, IAggregateRoot
    {
        [NotNull]
        [Indexed]
        [ForeignKey(typeof(Dialog))]
        public int DialogId { get; set; }

        public string DialogCode { get; set; }
        
        [NotNull] public string Actor { get; set; }

        public string TextTh { get; set; }
        public string TextEn { get; set; }
        public string TextZhCn { get; set; }
        public string TextZhTw { get; set; }
        public string TextJa { get; set; }
    }
}