using System;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.RoutineEntity.Models
{
    [Serializable]
    public class Routine : EntityBase<int>, IAggregateRoot
    {
       [NotNull]
       [Indexed]
       public string Code { get; set; }
               
       [NotNull]
       public string TargetEventPoint { get; set; }
       [NotNull]
       public string TargetActors { get; set; }

       [NotNull]
       public string ActiveDaysOfWeek { get; set; }
       [NotNull]
       public int StartingHour { get; set; }
       [NotNull]
       public int StartingMinute { get; set; }
       [NotNull]
       public int EndingHour { get; set; }
       [NotNull]
       public int EndingMinute { get; set; }
       
       [Indexed]
       [ForeignKey(typeof(Dialog))]
       public int DialogId { get; set; }
    }
}