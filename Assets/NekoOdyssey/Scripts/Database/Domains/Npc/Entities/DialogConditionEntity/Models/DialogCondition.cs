using System;
using System.Collections.Generic;
using NekoOdyssey.Scripts.Database.Domains.Npc.Commons;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogAnswerEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogConditionCaseEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogConditionOptionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.SubDialogEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogConditionEntity.Models
{
    [Serializable]
    public class DialogCondition : EntityBase<int>, IAggregateRoot, IDialogNextEntity
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
        
        [Ignore]
        public virtual ICollection<DialogConditionCase> ConditionCases { get; set; }
        
        [Ignore]
        public virtual ICollection<DialogConditionOption> Options { get; set; }
    }
}