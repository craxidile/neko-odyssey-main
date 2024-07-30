using System;
using System.Collections;
using System.Collections.Generic;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.ChatConditionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.ChatGroupEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.ChatEntity.Models
{
    [Serializable]
    public class Chat : EntityBase<int>, IAggregateRoot
    {
       [Indexed]
       [ForeignKey(typeof(ChatGroup))]
       public int ChatGroupId { get; set; }
               
       [NotNull]
       [Indexed]
       public string Code { get; set; }
       
       public string TitleOriginal { get; set; }
       public string TitleTh { get; set; }
       public string TitleEn { get; set; }
       public string TitleZhCn { get; set; }
       public string TitleZhTw { get; set; }
       public string TitleJa { get; set; }
       
       public string DescriptionOriginal { get; set; }
       public string DescriptionTh { get; set; }
       public string DescriptionEn { get; set; }
       public string DescriptionZhCn { get; set; }
       public string DescriptionZhTw { get; set; }
       public string DescriptionJa { get; set; }

       [Indexed]
       [ForeignKey(typeof(Dialog))]
       public int? DialogId { get; set; }
       
       [Ignore]
       public virtual Dialog Dialog { get; set; }
       
       [Ignore]
       public virtual ChatGroup ChatGroup { get; set; }
       
       [Ignore]
       public virtual ICollection<ChatCondition> Conditions { get; set; }
    }
}