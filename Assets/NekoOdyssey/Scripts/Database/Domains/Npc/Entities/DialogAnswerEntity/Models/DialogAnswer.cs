using System;
using NekoOdyssey.Scripts.Database.Domains.Npc.Commons;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogConditionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogQuestionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.SubDialogEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogAnswerEntity.Models
{
    [Serializable]
    public class DialogAnswer : EntityBase<int>, IAggregateRoot
    {
        // [Indexed]
        // [ForeignKey(typeof(DialogQuestion))]
        public int QuestionId { get; set; }
        
        public string Actor { get; set; }
        
        public string Original { get; set; }
        public string TextTh { get; set; }
        public string TextEn { get; set; }
        public string TextZhCn { get; set; }
        public string TextZhTw { get; set; }
        public string TextJa { get; set; }
        
        public string AnimatorParam { get; set; }
        public string AnimatorParamValue { get; set; }
        
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