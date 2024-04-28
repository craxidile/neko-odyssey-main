using System;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogLineEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogChoiceEntity.Models
{
    [Serializable]
    public class DialogChoice : EntityBase<int>, IAggregateRoot
    {
        [Indexed]
        [ForeignKey(typeof(DialogLine))]
        public int DialogLineId { get; set; }
        
        [Indexed]
        [ForeignKey(typeof(DialogLine))]
        public int NextDialogLineId { get; set; }
        
        public string Actor { get; set; }

        public string TextTh { get; set; }
        public string TextEn { get; set; }
        public string TextZhCn { get; set; }
        public string TextZhTw { get; set; }
        public string TextJa { get; set; }
    }
}