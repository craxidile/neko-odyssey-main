using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.SaveV001.DailySummaryEntity.Models
{
    public class DailySummaryV001 : EntityBase<int>, IAggregateRoot
    {
        [NotNull]
        public int LikeCount { get; set; }
        
        [NotNull]
        public int FollowCount { get; set; }
        
        [NotNull]
        public int CommentCount { get; set; }
        
        [NotNull]
        public int CatInteractionCount { get; set; }
        
        [NotNull]
        public int PhotoCount { get; set; }
        
        [NotNull]
        public int Money { get; set; }
    }
}