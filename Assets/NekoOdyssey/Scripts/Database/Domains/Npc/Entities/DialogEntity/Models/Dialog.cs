using System;
using NekoOdyssey.Scripts.Database.Domains.Npc.Commons;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogConditionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogQuestionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.SubDialogEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogEntity.Models
{
    [Serializable]
    public class Dialog : EntityBase<int>, IAggregateRoot
    {
        [NotNull]
        [Indexed]
        public string Code { get; set; }
        
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