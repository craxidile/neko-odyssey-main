using System;
using System.Collections.Generic;
using NekoOdyssey.Scripts.Database.Domains.Npc.Commons;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogAnswerEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogConditionOptionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.SubDialogEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogQuestionEntity.Models
{
    [Serializable]
    public class DialogQuestion : EntityBase<int>, IAggregateRoot, IDialogNextEntity
    {
        // [Indexed]
        // [ForeignKey(typeof(Dialog))]
        public int DialogId { get; set; }
        
        // [Indexed]
        // [ForeignKey(typeof(SubDialog))]
        public int SubDialogId { get; set; }
        
        // [Indexed]
        // [ForeignKey(typeof(DialogAnswer))]
        public int AnswerId { get; set; }
        
        // [Indexed]
        // [ForeignKey(typeof(DialogConditionOption))]
        public int OptionId { get; set; }
        
        public string Actor { get; set; }
        
        public string Original { get; set; }
        public string TextTh { get; set; }
        public string TextEn { get; set; }
        public string TextZhCn { get; set; }
        public string TextZhTw { get; set; }
        public string TextJa { get; set; }
        
        [Ignore]
        public virtual ICollection<DialogAnswer> Answers { get; set; }
    }
}