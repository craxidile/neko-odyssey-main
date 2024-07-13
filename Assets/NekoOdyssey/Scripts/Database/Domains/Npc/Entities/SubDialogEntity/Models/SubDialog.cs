using System;
using System.Collections.Generic;
using NekoOdyssey.Scripts.Database.Domains.Npc.Commons;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogAnswerEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogConditionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogConditionOptionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogLineEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogQuestionEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.SubDialogEntity.Models
{
    [Serializable]
    public class SubDialog : EntityBase<int>, IAggregateRoot, IDialogNextEntity
    {
        // [Indexed]
        // [ForeignKey(typeof(Dialog))]
        public int DialogId { get; set; }
        
        // [Indexed]
        // [ForeignKey(typeof(DialogAnswer))]
        public int AnswerId { get; set; }
        
        // [Indexed]
        // [ForeignKey(typeof(DialogConditionOption))]
        public int OptionId { get; set; }
        
        public int ChildFlag { get; set; }
        [Ignore]
        public DialogChildFlag DialogChildFlag => (DialogChildFlag)ChildFlag;
        
        [Ignore]
        public virtual ICollection<DialogLine> Lines { get; set; }
        
        [Ignore]
        public virtual DialogQuestion Question { get; set; }
        
        [Ignore]
        public virtual DialogCondition Condition { get; set; }
        
        [Ignore]
        public virtual IDialogNextEntity NextEntity { get; set; }
    }
}