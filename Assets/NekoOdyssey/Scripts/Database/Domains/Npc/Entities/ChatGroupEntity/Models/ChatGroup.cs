﻿using System;
using System.Collections.Generic;
using NekoOdyssey.Scripts.Database.Commons.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.ChatEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.ChatGroupConditionEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.ChatGroupEntity.Models
{
    [Serializable]
    public class ChatGroup : EntityBase<int>, IAggregateRoot
    {
        [NotNull]
        [Indexed]
        public string Code { get; set; }

        public string TitleOriginal { get; set; }
        public string TitleTh { get; set; }
        public string TitleEn { get; set; }
        public string TitleZhCn { get; set; }
        public string TitleZhTw { get; set; }
        public string TitleJa { get; set; }

        [Ignore]
        public LocalizedText LocalizedTitle => new()
        {
            Original = TitleOriginal,
            Th = TitleTh,
            En = TitleEn,
            ZhCn = TitleZhCn,
            ZhTw = TitleZhTw,
            Ja = TitleJa,
        };

        public string DescriptionOriginal { get; set; }
        public string DescriptionTh { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionZhCn { get; set; }
        public string DescriptionZhTw { get; set; }
        public string DescriptionJa { get; set; }
        
        [Ignore]
        public LocalizedText LocalizedDescription => new()
        {
            Original = DescriptionOriginal,
            Th = DescriptionTh,
            En = DescriptionEn,
            ZhCn = DescriptionZhCn,
            ZhTw = DescriptionZhTw,
            Ja = DescriptionJa,
        };

        [Ignore]
        public virtual ICollection<Chat> Chats { get; set; }
        [Ignore]
        public virtual ICollection<ChatGroupCondition> Conditions { get; set; }
    }
}