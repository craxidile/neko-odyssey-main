using System;
using NekoOdyssey.Scripts.Database.Domains.Npc.Commons;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogConditionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogQuestionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.SubDialogEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogConditionOptionEntity.Models
{
    [Serializable]
    public class DialogConditionOption : EntityBase<int>, IAggregateRoot
    {
        // [Indexed]
        // [ForeignKey(typeof(DialogCondition))]
        public int ConditionId { get; set; }

        [NotNull]
        public bool Valid { get; set; }

        [NotNull]
        public int ChildFlag { get; set; }
        [Ignore]
        public DialogChildFlag DialogChildFlag => (DialogChildFlag)ChildFlag;
        
        [Ignore]
        public virtual SubDialog SubDialog { get; set; }
        
        [Ignore]
        public virtual DialogQuestion Question { get; set; }
        
        [Ignore]
        public virtual DialogCondition Condition { get; set; }
        
        [Ignore]
        public virtual IDialogNextEntity NextEntity { get; set; }
    }
}