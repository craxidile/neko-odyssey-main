using System;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestEntity.Models;
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
    }
}