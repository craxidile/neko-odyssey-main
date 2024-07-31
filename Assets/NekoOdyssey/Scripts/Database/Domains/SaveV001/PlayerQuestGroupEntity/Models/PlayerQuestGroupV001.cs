using System;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestGroupEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerQuestGroupEntity.Models
{
    [Serializable]
    public class PlayerQuestGroupV001 : EntityBase<int>, IAggregateRoot
    {
        [NotNull] [Unique] public string QuestGroupCode { get; set; }

        [Ignore] public QuestGroup QuestGroup { get; set; }

        public DateTime ReceivedAt { get; set; }

        public PlayerQuestGroupV001()
        {
        }

        public PlayerQuestGroupV001(string questGroupCode)
        {
            QuestGroupCode = questGroupCode;
            ReceivedAt = DateTime.Now;
        }
    }
}