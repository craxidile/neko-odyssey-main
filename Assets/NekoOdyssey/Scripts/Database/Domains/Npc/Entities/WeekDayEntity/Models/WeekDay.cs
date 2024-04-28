using System;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.WeekDayEntity.Models
{
    [Serializable]
    public class WeekDay: EntityBase<int>, IAggregateRoot
    {
        [NotNull] [Indexed] public string Name { get; set; }
        
        [NotNull] public string ShortName { get; set; }
        
        public string NameTh { get; set; }
        public string NameEn { get; set; }
        public string NameZhCn { get; set; }
        public string NameZhTw { get; set; }
        public string NameJa { get; set; }

        public WeekDay()
        {
        }

        public WeekDay(string name, string shortName)
        {
            Name = name;
            ShortName = shortName;
        }
    }
}