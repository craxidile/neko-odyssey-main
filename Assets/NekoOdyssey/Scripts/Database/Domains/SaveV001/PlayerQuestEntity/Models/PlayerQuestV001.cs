﻿using System;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerQuestEntity.Models
{
    [Serializable]
    public class PlayerQuestV001 : EntityBase<int>, IAggregateRoot
    {
        [NotNull] [Unique] public string QuestCode { get; set; }

        [Ignore] public Quest Quest { get; set; }

        public DateTime ReceivedAt { get; set; }

        public PlayerQuestV001()
        {
        }

        public PlayerQuestV001(string questCode)
        {
            QuestCode = questCode;
            ReceivedAt = DateTime.Now;
        }
    }
}